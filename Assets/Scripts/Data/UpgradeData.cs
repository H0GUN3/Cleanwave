using UnityEngine;

namespace CleanWave.Data
{
    [CreateAssetMenu(fileName = "UpgradeData", menuName = "CleanWave/Upgrade Data")]
    public class UpgradeData : ScriptableObject
    {
        [System.Serializable]
        public class UpgradeLevel
        {
            public int cost;
            public float value;
        }

        public UpgradeLevel[] MoveSpeedLevels;
        public UpgradeLevel[] PickupRangeLevels;
        public UpgradeLevel[] BatteryLevels;
        public UpgradeLevel[] AutoSortAssistLevels;
    }
}
