using System;
using System.Collections.Generic;
using CleanWave.Trash;
using UnityEngine;

namespace CleanWave.Systems
{
    public class InventoryManager : MonoBehaviour
    {
        [SerializeField] private int maxCapacity = 20;

        private readonly Dictionary<TrashType, int> _trashCounts = new();
        private int _currentCount;

        public int CurrentCount => _currentCount;
        public int MaxCapacity => maxCapacity;
        public IReadOnlyDictionary<TrashType, int> TrashCounts => _trashCounts;

        public event Action InventoryChanged;

        private void Awake()
        {
            foreach (TrashType type in Enum.GetValues(typeof(TrashType)))
            {
                _trashCounts[type] = 0;
            }
        }

        public bool TryAddTrash(TrashType type)
        {
            if (_currentCount >= maxCapacity)
            {
                return false;
            }

            _trashCounts[type]++;
            _currentCount++;
            InventoryChanged?.Invoke();
            return true;
        }

        public bool TryRemoveTrash(TrashType type)
        {
            if (_trashCounts[type] <= 0)
            {
                return false;
            }

            _trashCounts[type]--;
            _currentCount--;
            InventoryChanged?.Invoke();
            return true;
        }

        public int GetCount(TrashType type)
        {
            return _trashCounts.TryGetValue(type, out var count) ? count : 0;
        }

        public void ClearAll()
        {
            foreach (TrashType type in Enum.GetValues(typeof(TrashType)))
            {
                _trashCounts[type] = 0;
            }

            _currentCount = 0;
            InventoryChanged?.Invoke();
        }
    }
}
