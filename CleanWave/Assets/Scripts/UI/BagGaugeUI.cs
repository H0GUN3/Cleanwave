using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CleanWave
{
    public class BagGaugeUI : MonoBehaviour
    {
        [SerializeField] private Image fillBar;
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private Image coinIcon;
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private BagInventory bagInventory;

        private static readonly Color colorEmpty  = new Color(0.3f, 0.85f, 0.3f);
        private static readonly Color colorHalf   = new Color(0.95f, 0.8f, 0.1f);
        private static readonly Color colorFull   = new Color(0.95f, 0.25f, 0.2f);

        private BagInventory bag;

        private void Start()
        {
            if (bag == null && bagInventory != null)
                Setup(bagInventory);
        }

        public void Setup(BagInventory bagInventory)
        {
            bag = bagInventory;
            if (bag != null)
            {
                bag.OnInventoryChanged += Refresh;
                bag.OnSelectionChanged += Refresh;
            }
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.OnCoinsChanged += UpdateCoins;
            Refresh();
        }

        private void OnDestroy()
        {
            if (bag != null)
            {
                bag.OnInventoryChanged -= Refresh;
                bag.OnSelectionChanged -= Refresh;
            }
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.OnCoinsChanged -= UpdateCoins;
        }

        public void Refresh()
        {
            if (bag == null) return;
            float ratio = bag.Capacity > 0 ? (float)bag.Count / bag.Capacity : 0f;

            if (fillBar != null)
            {
                fillBar.fillAmount = ratio;
                fillBar.color = Color.Lerp(
                    ratio < 0.5f ? colorEmpty : colorHalf,
                    ratio < 0.5f ? colorHalf  : colorFull,
                    ratio < 0.5f ? ratio * 2f : (ratio - 0.5f) * 2f
                );
            }

            if (countText != null)
                countText.text = $"{bag.Count} / {bag.Capacity}";
        }

        private void UpdateCoins(int coins)
        {
            if (coinText != null)
                coinText.text = coins.ToString();
        }
    }
}
