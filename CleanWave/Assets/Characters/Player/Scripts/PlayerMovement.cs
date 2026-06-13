using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// WASD 기반 2D 탑다운 이동. Rigidbody2D를 사용한다.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3.0f;

    Rigidbody2D rb;
    Vector2 moveInput;

    public Vector2 MoveInput => moveInput;
    public float MoveSpeed => moveSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            moveInput = Vector2.zero;
            return;
        }

        float x = 0f;
        float y = 0f;
        if (keyboard.wKey.isPressed) y += 1f;
        if (keyboard.sKey.isPressed) y -= 1f;
        if (keyboard.dKey.isPressed) x += 1f;
        if (keyboard.aKey.isPressed) x -= 1f;

        moveInput = new Vector2(x, y).normalized;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = Mathf.Max(0f, speed);
    }
}
