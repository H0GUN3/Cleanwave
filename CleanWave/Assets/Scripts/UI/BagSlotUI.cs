using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace CleanWave
{
    public class BagSlotUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI labelText;
        [SerializeField] private Image selectionHighlight;
        [SerializeField] private Button slotButton;

        private int slotIndex;
        public event Action<int> OnClicked;

        private void Awake()
        {
            if (slotButton != null)
                slotButton.onClick.AddListener(() => OnClicked?.Invoke(slotIndex));
        }

        public void Setup(int index)
        {
            slotIndex = index;
        }

        public void UpdateSlot(TrashItem item, bool isSelected)
        {
            bool hasItem = item != null;

            if (iconImage != null)
            {
                iconImage.gameObject.SetActive(hasItem);
                if (hasItem && item.TryGetComponent<SpriteRenderer>(out var sr))
                    iconImage.sprite = sr.sprite;
            }

            if (labelText != null)
                labelText.text = hasItem ? item.TrashType.ToString() : "";

            if (selectionHighlight != null)
                selectionHighlight.enabled = isSelected;
        }

        public void SetEmpty(bool isSelected)
        {
            if (iconImage != null) iconImage.gameObject.SetActive(false);
            if (labelText != null) labelText.text = "";
            if (selectionHighlight != null) selectionHighlight.enabled = isSelected;
        }
    }
}
