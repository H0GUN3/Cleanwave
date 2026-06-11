using UnityEngine;
using System.Collections.Generic;
using System;

namespace CleanWave
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private ResultScreen resultScreen;
        [SerializeField] private float totalGameTime = 600f;  // 10 minutes

        private GameState gameState = GameState.Playing;
        private List<ZoneManager> zones = new List<ZoneManager>();
        private int completedZones = 0;

        private float timeRemaining;
        private bool timerRunning = false;

        public event Action<float> OnTimerTick;
        public event Action OnTimeUp;

        public float TimeRemaining => timeRemaining;
        public bool IsTimerRunning => timerRunning;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            timeRemaining = totalGameTime;
            timerRunning = true;
        }

        private void Update()
        {
            if (!timerRunning || gameState != GameState.Playing) return;
            timeRemaining -= Time.deltaTime;
            OnTimerTick?.Invoke(timeRemaining);
            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                timerRunning = false;
                OnTimeUp?.Invoke();
                TriggerGameComplete(true);
            }
        }

        public void RegisterZone(ZoneManager zone)
        {
            if (!zones.Contains(zone))
                zones.Add(zone);
        }

        public void OnZoneCompleted(ZoneManager zone)
        {
            completedZones++;
            if (completedZones >= zones.Count)
                TriggerGameComplete(false);
        }

        private void TriggerGameComplete(bool timeUp = false)
        {
            if (gameState == GameState.Result) return;
            timerRunning = false;
            gameState = GameState.Result;

            float totalPurification = CalculateTotalPurification();
            int finalScore = ScoreManager.Instance != null ? ScoreManager.Instance.Score : 0;

            if (resultScreen != null)
                resultScreen.Show(finalScore, totalPurification, timeUp);
        }

        private float CalculateTotalPurification()
        {
            if (zones.Count == 0) return 0f;
            float total = 0f;
            foreach (var zone in zones)
                total += zone.PurificationRate;
            return total / zones.Count;
        }

        public bool IsPlaying() => gameState == GameState.Playing;
    }
}
