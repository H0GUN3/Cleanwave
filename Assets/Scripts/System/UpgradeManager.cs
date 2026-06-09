using CleanWave.Data;
using CleanWave.Player;
using UnityEngine;

namespace CleanWave.Systems
{
    public class UpgradeManager : MonoBehaviour
    {
        [SerializeField] private UpgradeData upgradeData;
        [SerializeField] private SaveManager saveManager;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private StageManager stageManager;

        public int Coins => saveManager != null && saveManager.CurrentState != null ? saveManager.CurrentState.Coins : 0;

        public bool TryUpgradeMoveSpeed() => TryUpgrade(
            () => saveManager.CurrentState.MoveSpeedLevel,
            level => saveManager.CurrentState.MoveSpeedLevel = level,
            upgradeData != null ? upgradeData.MoveSpeedLevels : null);

        public bool TryUpgradePickupRange() => TryUpgrade(
            () => saveManager.CurrentState.PickupRangeLevel,
            level => saveManager.CurrentState.PickupRangeLevel = level,
            upgradeData != null ? upgradeData.PickupRangeLevels : null);

        public bool TryUpgradeBattery() => TryUpgrade(
            () => saveManager.CurrentState.BatteryLevel,
            level => saveManager.CurrentState.BatteryLevel = level,
            upgradeData != null ? upgradeData.BatteryLevels : null);

        public bool TryUpgradeAutoSortAssist() => TryUpgrade(
            () => saveManager.CurrentState.AutoSortAssistLevel,
            level => saveManager.CurrentState.AutoSortAssistLevel = level,
            upgradeData != null ? upgradeData.AutoSortAssistLevels : null);

        private bool TryUpgrade(
            System.Func<int> getLevel,
            System.Action<int> setLevel,
            UpgradeData.UpgradeLevel[] levels)
        {
            if (saveManager == null || saveManager.CurrentState == null || levels == null)
            {
                return false;
            }

            var currentLevel = getLevel();
            var nextLevel = currentLevel + 1;
            if (nextLevel >= levels.Length)
            {
                return false;
            }

            var cost = levels[nextLevel].cost;
            if (saveManager.CurrentState.Coins < cost)
            {
                return false;
            }

            saveManager.CurrentState.Coins -= cost;
            setLevel(nextLevel);
            saveManager.Save();
            ApplyUpgrades();
            return true;
        }

        public void ApplyUpgrades()
        {
            if (saveManager == null || saveManager.CurrentState == null || upgradeData == null)
            {
                return;
            }

            var state = saveManager.CurrentState;
            if (playerController != null && upgradeData.PickupRangeLevels != null && state.PickupRangeLevel < upgradeData.PickupRangeLevels.Length)
            {
                // Reflection-free adjustment via serialized backing fields is not possible here,
                // so pickup range boost is expected to be handled through a dedicated public API if needed.
            }

            if (stageManager != null && upgradeData.BatteryLevels != null && state.BatteryLevel < upgradeData.BatteryLevels.Length)
            {
                stageManager.RecoverBattery(upgradeData.BatteryLevels[state.BatteryLevel].value);
            }
        }
    }
}
