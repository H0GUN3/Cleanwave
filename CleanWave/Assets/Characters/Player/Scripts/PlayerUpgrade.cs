using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 진화 단계에 따라 Animator Controller / 이동속도 / 가방용량을 변경한다.
/// 0 = Base, 1 = Upgrade, 2 = Final
/// </summary>
public class PlayerUpgrade : MonoBehaviour
{
    [System.Serializable]
    public class EvolutionStage
    {
        public string name = "Base";
        public RuntimeAnimatorController animatorController;
        public float moveSpeed = 3.0f;
        public int bagCapacity = 5;
    }

    [SerializeField] EvolutionStage[] stages = new EvolutionStage[3];
    [SerializeField] int currentLevel = 0;

    [Header("테스트용: Play Mode에서 1/2/3 키로 단계 전환")]
    [SerializeField] bool enableDebugKeys = true;

    PlayerMovement movement;
    PlayerBag bag;
    Animator animator;

    public int CurrentLevel => currentLevel;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        bag = GetComponent<PlayerBag>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        ApplyLevel(currentLevel);
    }

    void Update()
    {
        if (!enableDebugKeys) return;

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.digit1Key.wasPressedThisFrame) SetEvolutionLevel(0);
        if (keyboard.digit2Key.wasPressedThisFrame) SetEvolutionLevel(1);
        if (keyboard.digit3Key.wasPressedThisFrame) SetEvolutionLevel(2);
    }

    /// <summary>진화 단계 설정 (0~2). 외형/속도/용량을 모두 갱신한다.</summary>
    public void SetEvolutionLevel(int level)
    {
        if (level < 0 || level >= stages.Length)
        {
            Debug.LogWarning($"[PlayerUpgrade] 잘못된 진화 단계: {level}");
            return;
        }
        currentLevel = level;
        ApplyLevel(level);
    }

    /// <summary>다음 단계로 진화. 마지막 단계면 무시.</summary>
    public void EvolveNext()
    {
        if (currentLevel < stages.Length - 1)
            SetEvolutionLevel(currentLevel + 1);
    }

    void ApplyLevel(int level)
    {
        var stage = stages[level];
        if (stage == null)
        {
            Debug.LogWarning($"[PlayerUpgrade] 단계 {level} 데이터가 비어 있습니다.");
            return;
        }

        if (stage.animatorController != null && animator != null)
            animator.runtimeAnimatorController = stage.animatorController;

        if (movement != null)
            movement.SetMoveSpeed(stage.moveSpeed);

        if (bag != null)
            bag.SetCapacity(stage.bagCapacity);

        Debug.Log($"[PlayerUpgrade] 진화 단계 적용: {level} ({stage.name}) speed={stage.moveSpeed} bag={stage.bagCapacity}");
    }
}
