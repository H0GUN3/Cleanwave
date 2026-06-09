using System;
using CleanWave.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CleanWave.Systems
{
    public class StageManager : MonoBehaviour
    {
        [SerializeField] private StageConfig stageConfig;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private InventoryManager inventoryManager;
        [Header("Fallback (used when StageConfig is missing)")]
        [SerializeField] private float defaultTargetPollution = 15f;
        [SerializeField] private float defaultMaxBattery = 100f;
        [SerializeField] private float defaultTimeLimitSeconds = 300f;
        [SerializeField] private float defaultClearThresholdPercent = 80f;
        [SerializeField] private int defaultWrongSortLimit = 5;

        private float _timeLeft;
        private float _battery;
        private float _pollutionCleaned;
        private bool _isStageActive;

        private float TargetPollution => stageConfig != null ? stageConfig.TargetPollution : defaultTargetPollution;
        private float MaxBattery => stageConfig != null ? stageConfig.MaxBattery : defaultMaxBattery;
        private float TimeLimitSeconds => stageConfig != null ? stageConfig.TimeLimitSeconds : defaultTimeLimitSeconds;
        private float ClearThresholdPercent => stageConfig != null ? stageConfig.ClearThresholdPercent : defaultClearThresholdPercent;
        public float ProgressPercent => TargetPollution <= 0
            ? 0f
            : Mathf.Clamp01(_pollutionCleaned / TargetPollution) * 100f;
        public float BatteryPercent => MaxBattery <= 0
            ? 0f
            : Mathf.Clamp01(_battery / MaxBattery) * 100f;
        public float TimeLeft => _timeLeft;
        public int WrongSortLimit => stageConfig != null ? stageConfig.MaxWrongSortCount : defaultWrongSortLimit;

        public event Action StageUpdated;
        public event Action<bool> StageEnded;

        private void Start()
        {
            BeginStage();
        }

        private void Update()
        {
            if (!_isStageActive)
            {
                return;
            }

            _timeLeft -= Time.deltaTime;
            if (_timeLeft <= 0f)
            {
                _timeLeft = 0f;
                EndStage(false);
                return;
            }

            if (_battery <= 0f)
            {
                EndStage(false);
                return;
            }

            if (scoreManager != null && scoreManager.WrongSortCount >= WrongSortLimit)
            {
                EndStage(false);
                return;
            }

            if (ProgressPercent >= ClearThresholdPercent)
            {
                EndStage(true);
                return;
            }

            StageUpdated?.Invoke();
        }

        public void BeginStage()
        {
            if (stageConfig == null)
            {
                Debug.LogWarning("StageConfig is missing. Using fallback defaults.");
            }

            _timeLeft = TimeLimitSeconds;
            _battery = MaxBattery;
            _pollutionCleaned = 0f;
            _isStageActive = true;
            scoreManager?.ResetAll();
            inventoryManager?.ClearAll();
            StageUpdated?.Invoke();
        }

        public void AddCleanProgress(float amount)
        {
            _pollutionCleaned = Mathf.Max(0f, _pollutionCleaned + amount);
            StageUpdated?.Invoke();
        }

        public void ConsumeBattery(float amount)
        {
            _battery = Mathf.Max(0f, _battery - Mathf.Abs(amount));
            StageUpdated?.Invoke();
        }

        public void RecoverBattery(float amount)
        {
            _battery = Mathf.Clamp(_battery + Mathf.Abs(amount), 0f, MaxBattery);
            StageUpdated?.Invoke();
        }

        public void EndStage(bool isSuccess)
        {
            if (!_isStageActive)
            {
                return;
            }

            _isStageActive = false;
            StageEnded?.Invoke(isSuccess);
        }

        public void GoToResultScene(string resultSceneName)
        {
            if (!string.IsNullOrWhiteSpace(resultSceneName))
            {
                SceneManager.LoadScene(resultSceneName);
            }
        }
    }
}
