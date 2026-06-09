using UnityEngine;

namespace CleanWave.Data
{
    [CreateAssetMenu(fileName = "StageConfig", menuName = "CleanWave/Stage Config")]
    public class StageConfig : ScriptableObject
    {
        [Header("Identity")]
        public string StageId = "Stage1_City";
        public string StageDisplayName = "도심";

        [Header("Clear")]
        [Range(0, 100)] public float ClearThresholdPercent = 80f;
        public float TargetPollution = 100f;
        public float TimeLimitSeconds = 360f;
        public int MaxWrongSortCount = 5;

        [Header("Battery")]
        public float MaxBattery = 100f;
        public float MoveBatteryCostPerSecond = 0.5f;
        public float BoostBatteryCostPerSecond = 2.5f;
        public float PickupBatteryCost = 1f;

        [Header("Score")]
        public int ClearBonusScore = 100;
    }
}
