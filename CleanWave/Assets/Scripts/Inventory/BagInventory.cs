using UnityEngine;
using System.Collections.Generic;
using System;

namespace CleanWave
{
    public class BagInventory : MonoBehaviour
    {
        [SerializeField] private int capacity = 5;

        private List<TrashItem> items = new List<TrashItem>();
        private int selectedIndex = 0;

        public int Capacity
        {
            get => capacity;
            set { capacity = value; OnInventoryChanged?.Invoke(); }
        }

        public int Count => items.Count;
        public int SelectedIndex => selectedIndex;

        public event Action OnInventoryChanged;
        public event Action OnSelectionChanged;

        private void Update()
        {
            if (BinSelectionUI.IsPopupOpen) return;

            if (Input.GetKeyDown(KeyCode.Q))
                ShiftSelection(-1);
            else if (Input.GetKeyDown(KeyCode.E))
                ShiftSelection(1);
        }

        public bool CanAdd() => items.Count < capacity;

        public void AddTrash(TrashItem item)
        {
            if (!CanAdd()) return;
            items.Add(item);
            OnInventoryChanged?.Invoke();
        }

        public TrashItem GetSelectedItem()
        {
            if (items.Count == 0) return null;
            selectedIndex = Mathf.Clamp(selectedIndex, 0, items.Count - 1);
            return items[selectedIndex];
        }

        public void RemoveSelected()
        {
            if (items.Count == 0) return;
            selectedIndex = Mathf.Clamp(selectedIndex, 0, items.Count - 1);
            items.RemoveAt(selectedIndex);
            if (selectedIndex >= items.Count && selectedIndex > 0)
                selectedIndex--;
            OnInventoryChanged?.Invoke();
        }

        public TrashItem GetItemAt(int index)
        {
            if (index < 0 || index >= items.Count) return null;
            return items[index];
        }

        public void SetSelectedIndex(int index)
        {
            if (index < 0 || index >= items.Count) return;
            selectedIndex = index;
            OnSelectionChanged?.Invoke();
        }

        private void ShiftSelection(int dir)
        {
            if (items.Count == 0) return;
            selectedIndex = (selectedIndex + dir + items.Count) % items.Count;
            OnSelectionChanged?.Invoke();
        }
    }
}
