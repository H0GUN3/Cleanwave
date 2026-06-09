using System;
using UnityEngine;

namespace CleanWave.Systems
{
    [Serializable]
    public class SaveState
    {
        public int Coins;
        public int HighestScore;
        public int UnlockedStageIndex;
        public int MoveSpeedLevel;
        public int PickupRangeLevel;
        public int BatteryLevel;
        public int AutoSortAssistLevel;
        public bool LastStageSuccess;
    }

    public class SaveManager : MonoBehaviour
    {
        private const string SaveKey = "CleanWave.SaveState.v1";

        public SaveState CurrentState { get; private set; }

        private void Awake()
        {
            Load();
        }

        public void Load()
        {
            if (!PlayerPrefs.HasKey(SaveKey))
            {
                CurrentState = new SaveState
                {
                    Coins = 0,
                    HighestScore = 0,
                    UnlockedStageIndex = 1
                };
                Save();
                return;
            }

            var json = PlayerPrefs.GetString(SaveKey, string.Empty);
            CurrentState = string.IsNullOrEmpty(json) ? new SaveState() : JsonUtility.FromJson<SaveState>(json);
            CurrentState ??= new SaveState();
        }

        public void Save()
        {
            if (CurrentState == null)
            {
                CurrentState = new SaveState();
            }

            var json = JsonUtility.ToJson(CurrentState);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }

        public void SaveStageRewards(int score, int coins, int clearedStageIndex)
        {
            CurrentState.HighestScore = Mathf.Max(CurrentState.HighestScore, score);
            CurrentState.Coins += Mathf.Max(0, coins);
            CurrentState.UnlockedStageIndex = Mathf.Max(CurrentState.UnlockedStageIndex, clearedStageIndex + 1);
            Save();
        }

        public void SaveLastStageResult(bool success)
        {
            CurrentState.LastStageSuccess = success;
            Save();
        }
    }
}
