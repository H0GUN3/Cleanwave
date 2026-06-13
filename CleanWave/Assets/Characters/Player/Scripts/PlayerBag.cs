using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 쓰레기 가방. 가득 차면 더 이상 줍지 못한다.
/// HUD 텍스트 갱신을 위해 OnBagChanged 이벤트를 제공한다.
/// </summary>
public class PlayerBag : MonoBehaviour
{
    [SerializeField] int maxCapacity = 5;
    [SerializeField] int currentCount = 0;

    /// <summary>가방 내용 변경 시 (currentCount, maxCapacity) 전달.</summary>
    public UnityEvent<int, int> OnBagChanged = new UnityEvent<int, int>();

    public int CurrentCount => currentCount;
    public int MaxCapacity => maxCapacity;
    public bool IsFull => currentCount >= maxCapacity;

    void Start()
    {
        NotifyChanged();
    }

    /// <summary>쓰레기를 1개 추가. 가득 차 있으면 false.</summary>
    public bool TryAddTrash()
    {
        if (IsFull)
            return false;

        currentCount++;
        NotifyChanged();
        return true;
    }

    /// <summary>쓰레기를 1개 제거. 비어 있으면 false.</summary>
    public bool RemoveTrash()
    {
        if (currentCount <= 0)
            return false;

        currentCount--;
        NotifyChanged();
        return true;
    }

    /// <summary>가방 용량 변경. 현재 개수는 유지하되 새 용량을 넘으면 잘라낸다.</summary>
    public void SetCapacity(int capacity)
    {
        maxCapacity = Mathf.Max(0, capacity);
        currentCount = Mathf.Min(currentCount, maxCapacity);
        NotifyChanged();
    }

    void NotifyChanged()
    {
        OnBagChanged?.Invoke(currentCount, maxCapacity);
    }
}
