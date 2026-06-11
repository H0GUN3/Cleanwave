using UnityEngine;
using TMPro;

namespace CleanWave
{
    public class HudController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private TextMeshProUGUI purificationText;
        [SerializeField] private BagSlotUI[] bagSlots;
        [SerializeField] private BagInventory bagInventory;
        [SerializeField] private ZoneManager[] zoneManagers;

        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI cityPurText;
        [SerializeField] private TextMeshProUGUI riverPurText;
        [SerializeField] private TextMeshProUGUI beachPurText;

        private void Start()
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnScoreChanged += UpdateScore;
                ScoreManager.Instance.OnCoinsChanged += UpdateCoins;
            }

            if (GameManager.Instance != null)
                GameManager.Instance.OnTimerTick += UpdateTimer;

            if (bagInventory != null)
            {
                bagInventory.OnInventoryChanged += RefreshBagUI;
                bagInventory.OnSelectionChanged += RefreshBagUI;
            }

            SetupBagSlots();
            RefreshAll();
        }

        private void OnDestroy()
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnScoreChanged -= UpdateScore;
                ScoreManager.Instance.OnCoinsChanged -= UpdateCoins;
            }

            if (GameManager.Instance != null)
                GameManager.Instance.OnTimerTick -= UpdateTimer;

            if (bagInventory != null)
            {
                bagInventory.OnInventoryChanged -= RefreshBagUI;
                bagInventory.OnSelectionChanged -= RefreshBagUI;
            }
        }

        private void Update()
        {
            UpdatePurification();
            UpdateZonePurifications();
        }

        private void SetupBagSlots()
        {
            for (int i = 0; i < bagSlots.Length; i++)
            {
                int idx = i;
                bagSlots[i].Setup(i);
                bagSlots[i].OnClicked += (index) => bagInventory?.SetSelectedIndex(index);
            }
        }

        private void UpdateScore(int score)
        {
            if (scoreText != null)
                scoreText.text = $"Score: {score}";
        }

        private void UpdateCoins(int coins)
        {
            if (coinText != null)
                coinText.text = $"Coins: {coins}";
        }

        private void UpdateTimer(float seconds)
        {
            if (timerText == null) return;
            int m = Mathf.FloorToInt(seconds / 60f);
            int s = Mathf.FloorToInt(seconds % 60f);
            timerText.text = $"{m:D2}:{s:D2}";
            timerText.color = seconds < 30f ? new Color(1f, 0.2f, 0.2f) : Color.white;
        }

        private void UpdatePurification()
        {
            if (purificationText == null || zoneManagers == null || zoneManagers.Length == 0) return;
            float total = 0f;
            int count = 0;
            foreach (var zm in zoneManagers)
            {
                if (zm != null) { total += zm.PurificationRate; count++; }
            }
            float avg = count > 0 ? total / count : 0f;
            purificationText.text = $"Clean: {avg:F0}%";
        }

        private void UpdateZonePurifications()
        {
            if (zoneManagers == null) return;
            foreach (var zm in zoneManagers)
            {
                if (zm == null) continue;
                string pct = $"{zm.PurificationRate:F0}%";
                switch (zm.ZoneType)
                {
                    case ZoneType.City:  if (cityPurText)  cityPurText.text  = $"City: {pct}";  break;
                    case ZoneType.River: if (riverPurText) riverPurText.text = $"River: {pct}"; break;
                    case ZoneType.Beach: if (beachPurText) beachPurText.text = $"Beach: {pct}"; break;
                }
            }
        }

        private void RefreshBagUI()
        {
            if (bagSlots == null || bagInventory == null) return;
            for (int i = 0; i < bagSlots.Length; i++)
            {
                if (i < bagInventory.Capacity)
                {
                    TrashItem item = bagInventory.GetItemAt(i);
                    bagSlots[i].UpdateSlot(item, i == bagInventory.SelectedIndex);
                    bagSlots[i].gameObject.SetActive(true);
                }
                else
                {
                    bagSlots[i].gameObject.SetActive(false);
                }
            }
        }

        private void RefreshAll()
        {
            UpdateScore(ScoreManager.Instance != null ? ScoreManager.Instance.Score : 0);
            UpdateCoins(ScoreManager.Instance != null ? ScoreManager.Instance.Coins : 0);
            RefreshBagUI();
        }
    }
}
