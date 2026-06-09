using CleanWave.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CleanWave.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private StageManager stageManager;
        [SerializeField] private InventoryManager inventoryManager;

        [Header("HUD")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text comboText;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private TMP_Text inventoryText;
        [SerializeField] private Slider batterySlider;
        [SerializeField] private Slider cleanSlider;

        private void OnEnable()
        {
            if (scoreManager != null)
            {
                scoreManager.ScoreChanged += Refresh;
                scoreManager.ComboChanged += Refresh;
            }

            if (inventoryManager != null)
            {
                inventoryManager.InventoryChanged += Refresh;
            }

            if (stageManager != null)
            {
                stageManager.StageUpdated += Refresh;
            }
        }

        private void OnDisable()
        {
            if (scoreManager != null)
            {
                scoreManager.ScoreChanged -= Refresh;
                scoreManager.ComboChanged -= Refresh;
            }

            if (inventoryManager != null)
            {
                inventoryManager.InventoryChanged -= Refresh;
            }

            if (stageManager != null)
            {
                stageManager.StageUpdated -= Refresh;
            }
        }

        private void Start()
        {
            Refresh();
        }

        private void Update()
        {
            RefreshTimeOnly();
        }

        private void Refresh()
        {
            if (scoreText != null && scoreManager != null)
            {
                scoreText.text = $"Score {scoreManager.CurrentScore}";
            }

            if (comboText != null && scoreManager != null)
            {
                comboText.text = $"Combo x{scoreManager.CurrentComboMultiplier}";
            }

            if (inventoryText != null && inventoryManager != null)
            {
                inventoryText.text = $"Inventory {inventoryManager.CurrentCount}/{inventoryManager.MaxCapacity}";
            }

            if (batterySlider != null && stageManager != null)
            {
                batterySlider.value = stageManager.BatteryPercent / 100f;
            }

            if (cleanSlider != null && stageManager != null)
            {
                cleanSlider.value = stageManager.ProgressPercent / 100f;
            }

            RefreshTimeOnly();
        }

        private void RefreshTimeOnly()
        {
            if (timeText == null || stageManager == null)
            {
                return;
            }

            var remain = Mathf.Max(0f, stageManager.TimeLeft);
            var minutes = Mathf.FloorToInt(remain / 60f);
            var seconds = Mathf.FloorToInt(remain % 60f);
            timeText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}
