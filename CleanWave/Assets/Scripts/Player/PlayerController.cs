using UnityEngine;

namespace CleanWave
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 3f;

        private Rigidbody2D rb;
        private Animator anim;
        private SpriteRenderer spriteRenderer;
        private Vector2 moveInput;
        private string currentDirection = "down";

        public string CurrentDirection => currentDirection;
        public float MoveSpeed
        {
            get => moveSpeed;
            set => moveSpeed = value;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }

        private void Update()
        {
            if (BinSelectionUI.IsPopupOpen) { rb.linearVelocity = Vector2.zero; return; }

            if (GameManager.Instance != null && !GameManager.Instance.IsPlaying()) return;

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            moveInput = new Vector2(h, v).normalized;

            UpdateDirection(h, v);
            UpdateAnimator();
        }

        private void FixedUpdate()
        {
            if (BinSelectionUI.IsPopupOpen) return;

            rb.linearVelocity = moveInput * moveSpeed;
        }

        private void UpdateDirection(float h, float v)
        {
            if (Mathf.Abs(h) > Mathf.Abs(v))
            {
                currentDirection = "side";
                float faceX = h > 0 ? 1f : -1f;
                if (anim != null) anim.SetFloat("FaceX", faceX);
                // Flip sprite instead of needing mirrored animation clips
                if (spriteRenderer != null) spriteRenderer.flipX = faceX < 0f;
            }
            else if (v > 0.01f)
            {
                currentDirection = "up";
            }
            else if (v < -0.01f)
            {
                currentDirection = "down";
            }
        }

        private void UpdateAnimator()
        {
            if (anim == null) return;

            bool isMoving = moveInput.sqrMagnitude > 0.01f;
            anim.SetBool("IsMoving", isMoving);
            // 0=down, 1=up, 2=side
            int dirInt = currentDirection == "up" ? 1 : currentDirection == "side" ? 2 : 0;
            anim.SetInteger("Direction", dirInt);
        }

        public void TriggerPickupAnimation()
        {
            if (anim != null)
                anim.SetTrigger("Pickup");
        }
    }
}
