using UnityEngine;
using System;

namespace CleanWave
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        public int Score { get; private set; }
        public int Coins { get; private set; }

        public event Action<int> OnScoreChanged;
        public event Action<int> OnCoinsChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void AddScore(int amount)
        {
            Score += amount;
            OnScoreChanged?.Invoke(Score);
        }

        public void SubtractScore(int amount)
        {
            Score = Mathf.Max(0, Score - amount);
            OnScoreChanged?.Invoke(Score);
        }

        public bool TrySpendCoins(int amount)
        {
            if (Coins < amount) return false;
            Coins -= amount;
            OnCoinsChanged?.Invoke(Coins);
            return true;
        }

        public void AddCoins(int amount)
        {
            Coins += amount;
            OnCoinsChanged?.Invoke(Coins);
        }
    }
}
