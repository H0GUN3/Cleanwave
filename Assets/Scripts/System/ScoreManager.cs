using System;
using UnityEngine;

namespace CleanWave.Systems
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private int basePickupScore = 10;
        [SerializeField] private int correctSortScore = 20;
        [SerializeField] private int wrongSortPenalty = -15;
        [SerializeField] private float comboWindowSeconds = 5f;

        private readonly int[] _comboSteps = { 1, 2, 3, 5 };
        private int _comboIndex;
        private float _comboExpiryTime;

        public int CurrentScore { get; private set; }
        public int BestComboMultiplier { get; private set; } = 1;
        public int CorrectSortCount { get; private set; }
        public int WrongSortCount { get; private set; }
        public int PickupCount { get; private set; }

        public float ComboRemainSeconds => Mathf.Max(0, _comboExpiryTime - Time.time);
        public int CurrentComboMultiplier => _comboSteps[_comboIndex];

        public event Action ScoreChanged;
        public event Action ComboChanged;

        public void AddPickupScore(int amount = -1)
        {
            var score = amount > 0 ? amount : basePickupScore;
            CurrentScore += score;
            PickupCount++;
            ScoreChanged?.Invoke();
        }

        public void AddCorrectSort()
        {
            ExtendCombo();
            CurrentScore += correctSortScore * CurrentComboMultiplier;
            CorrectSortCount++;
            ScoreChanged?.Invoke();
            ComboChanged?.Invoke();
        }

        public void AddWrongSort()
        {
            CurrentScore += wrongSortPenalty;
            WrongSortCount++;
            ResetCombo();
            ScoreChanged?.Invoke();
            ComboChanged?.Invoke();
        }

        private void Update()
        {
            if (_comboIndex > 0 && Time.time > _comboExpiryTime)
            {
                ResetCombo();
                ComboChanged?.Invoke();
            }
        }

        public void ResetAll()
        {
            CurrentScore = 0;
            CorrectSortCount = 0;
            WrongSortCount = 0;
            PickupCount = 0;
            ResetCombo();
            ScoreChanged?.Invoke();
            ComboChanged?.Invoke();
        }

        private void ExtendCombo()
        {
            _comboExpiryTime = Time.time + comboWindowSeconds;
            _comboIndex = Mathf.Clamp(_comboIndex + 1, 0, _comboSteps.Length - 1);
            BestComboMultiplier = Mathf.Max(BestComboMultiplier, CurrentComboMultiplier);
        }

        private void ResetCombo()
        {
            _comboIndex = 0;
            _comboExpiryTime = 0f;
        }
    }
}
