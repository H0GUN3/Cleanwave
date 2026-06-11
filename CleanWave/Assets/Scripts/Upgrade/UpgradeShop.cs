using UnityEngine;

namespace CleanWave
{
    public class UpgradeShop : MonoBehaviour
    {
        [SerializeField] private UpgradeType upgradeType;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private PlayerInteractor playerInteractor;
        [SerializeField] private BagInventory bagInventory;
        [SerializeField] private FeedbackUI feedbackUI;

        private static readonly int[] costs = { 50, 100 };
        private int currentLevel = 0;
        private const int maxLevel = 2;

        public void TryUpgrade()
        {
            if (currentLevel >= maxLevel)
            {
                feedbackUI?.ShowMessage("Already max level.");
                return;
            }

            int cost = costs[currentLevel];
            if (!ScoreManager.Instance.TrySpendCoins(cost))
            {
                feedbackUI?.ShowMessage("Not enough coins.");
                return;
            }

            currentLevel++;
            ApplyUpgrade();
            feedbackUI?.ShowMessage($"{GetUpgradeName()} upgraded! (Lv.{currentLevel})");
        }

        private void ApplyUpgrade()
        {
            switch (upgradeType)
            {
                case UpgradeType.Bag:
                    if (bagInventory != null)
                        bagInventory.Capacity = currentLevel == 1 ? 8 : 12;
                    break;

                case UpgradeType.Shoes:
                    if (playerController != null)
                    {
                        float baseSpeed = 3f;
                        float multiplier = currentLevel == 1 ? 1.15f : 1.30f;
                        playerController.MoveSpeed = baseSpeed * multiplier;
                    }
                    break;

                case UpgradeType.Tongs:
                    if (playerInteractor != null)
                        playerInteractor.InteractRange = currentLevel == 1 ? 1.5f : 2.0f;
                    break;
            }
        }

        private string GetUpgradeName()
        {
            switch (upgradeType)
            {
                case UpgradeType.Bag:   return "Bag";
                case UpgradeType.Shoes: return "Shoes";
                case UpgradeType.Tongs: return "Tongs";
                default: return "Gear";
            }
        }
    }
}
