using UnityEngine;

/// <summary>
/// 이동 입력을 8방향 Direction int로 변환해 Animator에 전달한다.
/// Direction 행 순서: 0 UP, 1 UP_RIGHT, 2 RIGHT, 3 DOWN_RIGHT, 4 DOWN, 5 DOWN_LEFT, 6 LEFT, 7 UP_LEFT
/// 정지 시 Animator speed를 0으로 만들어 마지막 방향/프레임을 유지한다.
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerAnimation8Dir : MonoBehaviour
{
    static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    static readonly int DirectionHash = Animator.StringToHash("Direction");

    PlayerMovement movement;
    Animator animator;
    int lastDirection = 4; // 기본값: DOWN

    public int CurrentDirection => lastDirection;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector2 input = movement != null ? movement.MoveInput : Vector2.zero;
        bool isMoving = input.sqrMagnitude > 0.0001f;

        if (isMoving)
        {
            lastDirection = VectorToDirection(input);
            animator.speed = 1f;
        }
        else
        {
            // 마지막 방향의 마지막 프레임에서 정지
            animator.speed = 0f;
        }

        animator.SetBool(IsMovingHash, isMoving);
        animator.SetInteger(DirectionHash, lastDirection);
    }

    /// <summary>
    /// 입력 벡터를 8방향 인덱스로 변환. 행 순서와 일치해야 한다.
    /// </summary>
    public static int VectorToDirection(Vector2 v)
    {
        // 위쪽(+Y)이 0도, 시계 방향으로 45도 간격
        float angle = Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg; // UP 기준 시계방향 각도
        if (angle < 0f) angle += 360f;
        int index = Mathf.RoundToInt(angle / 45f) % 8;
        // index: 0=UP, 1=UP_RIGHT, 2=RIGHT, 3=DOWN_RIGHT, 4=DOWN, 5=DOWN_LEFT, 6=LEFT, 7=UP_LEFT
        return index;
    }
}
