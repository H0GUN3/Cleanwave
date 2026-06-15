using UnityEngine;
using UnityEngine.InputSystem;

public class TrashBin : MonoBehaviour
{
    [SerializeField] TrashType acceptedType;
    [SerializeField] float interactRange = 2f;

    static PlayerBag cachedBag;
    static ScoreUI cachedScore;

    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null || !kb.eKey.wasPressedThisFrame) return;

        PlayerBag bag = GetBag();
        if (bag == null || bag.CurrentCount == 0) return;

        float dist = Vector2.Distance(transform.position, bag.transform.position);
        if (dist > interactRange) return;

        if (!bag.RemoveFirst(out TrashItemInfo item)) return;

        if (item.type == acceptedType)
        {
            GetScore()?.AddCoins(10);
            TrashSortUI.Instance?.ShowMessage($"{item.displayName} 분리수거 완료! +10 코인", Color.green);
        }
        else
        {
            TrashSortUI.Instance?.ShowMessage("잘못된 쓰레기통입니다. 다시 시도하세요!", Color.red);
            bag.TryAddTrash(item);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }

    PlayerBag GetBag()
    {
        if (cachedBag == null) cachedBag = FindFirstObjectByType<PlayerBag>();
        return cachedBag;
    }

    ScoreUI GetScore()
    {
        if (cachedScore == null) cachedScore = FindFirstObjectByType<ScoreUI>();
        return cachedScore;
    }
}
