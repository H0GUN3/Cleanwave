using System;
using System.Collections.Generic;
using CleanWave.Core;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 쓰레기 가방. 가득 차면 더 이상 줍지 못한다.
/// HUD 텍스트 갱신을 위해 OnBagChanged 이벤트를 제공한다.
/// </summary>
public class PlayerBag : MonoBehaviour
{
    [Serializable]
    public struct BagItem
    {
        public TrashType TrashType;
        public CleanWaveZoneType ZoneType;
        public Sprite IconSprite;

        public BagItem(TrashType trashType, CleanWaveZoneType zoneType)
            : this(trashType, zoneType, null)
        {
        }

        public BagItem(TrashType trashType, CleanWaveZoneType zoneType, Sprite iconSprite)
        {
            TrashType = trashType;
            ZoneType = zoneType;
            IconSprite = iconSprite;
        }
    }

    [SerializeField] int maxCapacity = 5;
    [SerializeField] List<BagItem> carriedTrash = new List<BagItem>();

    /// <summary>가방 내용 변경 시 (currentCount, maxCapacity) 전달.</summary>
    public UnityEvent<int, int> OnBagChanged = new UnityEvent<int, int>();

    public int CurrentCount => carriedTrash.Count;
    public int MaxCapacity => maxCapacity;
    public bool IsFull => CurrentCount >= maxCapacity;

    void Start()
    {
        NotifyChanged();
    }

    /// <summary>쓰레기를 1개 추가. 가득 차 있으면 false.</summary>
    public bool TryAddTrash()
    {
        return TryAddTrash(TrashType.Paper, CleanWaveZoneType.City);
    }

    public bool TryAddTrash(TrashType trashType, CleanWaveZoneType zoneType)
    {
        return TryAddTrash(trashType, zoneType, null);
    }

    public bool TryAddTrash(TrashType trashType, CleanWaveZoneType zoneType, Sprite iconSprite)
    {
        if (IsFull)
            return false;

        carriedTrash.Add(new BagItem(trashType, zoneType, iconSprite));
        NotifyChanged();
        return true;
    }

    public bool TryGetTrashAt(int index, out BagItem item)
    {
        if (index < 0 || index >= carriedTrash.Count)
        {
            item = default;
            return false;
        }

        item = carriedTrash[index];
        return true;
    }

    /// <summary>쓰레기를 1개 제거. 비어 있으면 false.</summary>
    public bool RemoveTrash()
    {
        return TryRemoveNextTrash(out _);
    }

    public bool TryPeekNextTrash(out BagItem item)
    {
        if (carriedTrash.Count <= 0)
        {
            item = default;
            return false;
        }

        item = carriedTrash[0];
        return true;
    }

    public bool TryRemoveNextTrash(out BagItem item)
    {
        if (!TryPeekNextTrash(out item))
            return false;

        carriedTrash.RemoveAt(0);
        NotifyChanged();
        return true;
    }

    /// <summary>가방 용량 변경. 현재 개수는 유지하되 새 용량을 넘으면 잘라낸다.</summary>
    public void SetCapacity(int capacity)
    {
        maxCapacity = Mathf.Max(0, capacity);
        if (carriedTrash.Count > maxCapacity)
            carriedTrash.RemoveRange(maxCapacity, carriedTrash.Count - maxCapacity);
        NotifyChanged();
    }

    void NotifyChanged()
    {
        OnBagChanged?.Invoke(CurrentCount, maxCapacity);
    }
}
