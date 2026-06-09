using CleanWave.Systems;
using CleanWave.Trash;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace CleanWave.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float boostMultiplier = 1.5f;
        [SerializeField] private float accelerationTime = 0.05f;
        [SerializeField] private float decelerationTime = 0.05f;

        [Header("Interaction Settings")]
        [SerializeField] private float pickupRadius = 1.5f;
        [SerializeField] private LayerMask trashLayer;
        [SerializeField] private InventoryManager inventoryManager;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private StageManager stageManager;

        private Rigidbody2D _rb;
        private Animator _animator;
        private SpriteRenderer _sr;

        private Vector2 _moveInput;
        private Vector2 _currentVelocity;
        private Vector2 _velocityRef;
        private Vector2 _facing = Vector2.down;
        private float _speedPenaltyMultiplier = 1f;

        private readonly Collider2D[] _interactionResults = new Collider2D[64];

        // Animator Hashes
        private static readonly int MoveXHash = Animator.StringToHash("MoveX");
        private static readonly int MoveYHash = Animator.StringToHash("MoveY");
        private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");

        public bool IsBoosting { get; private set; }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _sr = GetComponent<SpriteRenderer>();

            // Auto-wire runtime references if scene bindings are missing.
            inventoryManager ??= FindAnyObjectByType<InventoryManager>();
            scoreManager ??= FindAnyObjectByType<ScoreManager>();
            stageManager ??= FindAnyObjectByType<StageManager>();

            // Ensure physical consistency for top-down
            _rb.gravityScale = 0f;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            _rb.interpolation = RigidbodyInterpolation2D.None;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        private void Update()
        {
            HandleInput();
            UpdateAnimation();

            if (IsInteractPressed())
            {
                TryPickupNearestTrash();
            }
        }

        private void FixedUpdate()
        {
            ApplyMovement();

            if (_moveInput.sqrMagnitude > 0.01f && stageManager != null)
            {
                stageManager.ConsumeBattery(Time.fixedDeltaTime * (IsBoosting ? 2.0f : 0.5f));
            }
        }

        private void HandleInput()
        {
            _moveInput = ReadMoveInput();
            IsBoosting = IsBoostPressed();
        }

        private static Vector2 ReadMoveInput()
        {
            float x = 0f;
            float y = 0f;

            // 1) Legacy input manager path (when enabled).
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) x -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) x += 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) y -= 1f;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) y += 1f;
#endif

            var direct = new Vector2(x, y);
            if (direct.sqrMagnitude > 0f)
            {
                return direct.normalized;
            }

#if ENABLE_INPUT_SYSTEM
            // 2) Input System keyboard/gamepad path.
            if (Keyboard.current != null)
            {
                float nx = 0f;
                float ny = 0f;

                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) nx -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) nx += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) ny -= 1f;
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) ny += 1f;

                var fromInputSystem = new Vector2(nx, ny);
                if (fromInputSystem.sqrMagnitude > 0f)
                {
                    return fromInputSystem.normalized;
                }
            }

            if (Gamepad.current != null)
            {
                var leftStick = Gamepad.current.leftStick.ReadValue();
                if (leftStick.sqrMagnitude > 0.0001f)
                {
                    return leftStick.normalized;
                }
            }
#endif

            // 3) Axis fallback (legacy only).
#if ENABLE_LEGACY_INPUT_MANAGER
            var axis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (axis.sqrMagnitude < 0.0001f)
            {
                return Vector2.zero;
            }

            return axis.normalized;
#else
            return Vector2.zero;
#endif
        }

        private static bool IsBoostPressed()
        {
            bool pressed = false;
#if ENABLE_LEGACY_INPUT_MANAGER
            pressed |= Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
#endif
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                pressed |= Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
            }
#endif
            return pressed;
        }

        private static bool IsInteractPressed()
        {
            bool pressed = false;
#if ENABLE_LEGACY_INPUT_MANAGER
            pressed |= Input.GetKeyDown(KeyCode.E);
#endif
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                pressed |= Keyboard.current.eKey.wasPressedThisFrame;
            }
#endif
            return pressed;
        }

        private void ApplyMovement()
        {
            float targetSpeed = moveSpeed * _speedPenaltyMultiplier;
            if (IsBoosting) targetSpeed *= boostMultiplier;

            Vector2 targetVelocity = _moveInput * targetSpeed;
            float smoothTime = _moveInput.sqrMagnitude > 0.01f ? accelerationTime : decelerationTime;

            _currentVelocity = Vector2.SmoothDamp(_currentVelocity, targetVelocity, ref _velocityRef, smoothTime);
            _rb.linearVelocity = _currentVelocity;
        }

        private void UpdateAnimation()
        {
            bool isMoving = _moveInput.sqrMagnitude > 0.01f;

            if (isMoving)
            {
                float absX = Mathf.Abs(_moveInput.x);
                float absY = Mathf.Abs(_moveInput.y);
                float axisSwitchThreshold = 0.08f;
                bool currentlyVertical = Mathf.Abs(_facing.y) > 0.5f;

                // Keep previous axis when nearly diagonal to avoid oscillation/flicker.
                if (Mathf.Abs(absX - absY) <= axisSwitchThreshold)
                {
                    if (currentlyVertical)
                    {
                        _facing = new Vector2(0f, Mathf.Sign(_moveInput.y));
                    }
                    else
                    {
                        _facing = new Vector2(Mathf.Sign(_moveInput.x), 0f);
                    }
                }
                else if (absX > absY)
                {
                    _facing = new Vector2(Mathf.Sign(_moveInput.x), 0f);
                }
                else
                {
                    _facing = new Vector2(0f, Mathf.Sign(_moveInput.y));
                }
            }

            if (_sr != null)
            {
                _sr.flipX = (_facing.x > 0.5f);
            }

            if (_animator != null)
            {
                _animator.SetFloat(MoveXHash, _facing.x);
                _animator.SetFloat(MoveYHash, _facing.y);
                _animator.SetBool(IsMovingHash, isMoving);
            }
        }

        public void SetMovementPenalty(float multiplier)
        {
            _speedPenaltyMultiplier = Mathf.Clamp(multiplier, 0.2f, 1f);
        }

        private void TryPickupNearestTrash()
        {
            if (inventoryManager == null)
            {
                return;
            }

            float queryRadius = Mathf.Max(pickupRadius, 2.25f);
            int count = Physics2D.OverlapCircleNonAlloc(transform.position, queryRadius, _interactionResults, trashLayer);
            if (count == 0)
            {
                count = Physics2D.OverlapCircleNonAlloc(transform.position, queryRadius, _interactionResults);
            }

            TrashObject nearest = null;
            float nearestDist = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                var hit = _interactionResults[i];
                if (hit == null) continue;

                if (hit.TryGetComponent<TrashObject>(out var trash))
                {
                    if (trash.IsCollected) continue;

                    float d = Vector2.Distance(transform.position, hit.transform.position);
                    if (d < nearestDist)
                    {
                        nearestDist = d;
                        nearest = trash;
                    }
                }
            }

            if (nearest != null)
            {
                if (nearest.TryPickup(transform, inventoryManager))
                {
                    scoreManager?.AddPickupScore(nearest.PickupScore);
                    stageManager?.AddCleanProgress(1f);
                    stageManager?.ConsumeBattery(1f);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, pickupRadius);
        }
    }
}
