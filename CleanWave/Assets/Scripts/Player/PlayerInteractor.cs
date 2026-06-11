using UnityEngine;

namespace CleanWave
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private float interactRange = 1.5f;
        [SerializeField] private BagInventory bagInventory;
        [SerializeField] private FeedbackUI feedbackUI;
        [SerializeField] private BinSelectionUI binSelectionUI;
        [SerializeField] private LayerMask interactLayer;

        private PlayerController controller;

        public float InteractRange
        {
            get => interactRange;
            set => interactRange = value;
        }

        private void Awake()
        {
            controller = GetComponent<PlayerController>();
            if (bagInventory == null)
                Debug.LogWarning("[PlayerInteractor] BagInventory 미연결");
            if (binSelectionUI == null)
                Debug.LogWarning("[PlayerInteractor] BinSelectionUI 미연결");
            else
            {
                binSelectionUI.Setup(bagInventory);
                binSelectionUI.OnDepositRequested += HandleDeposit;
            }
        }

        private void Update()
        {
            if (binSelectionUI != null && binSelectionUI.IsOpen) return;
            if (GameManager.Instance != null && !GameManager.Instance.IsPlaying()) return;
            if (Input.GetKeyDown(KeyCode.F)) TryInteract();
        }

        private void TryInteract()
        {
            // layerMask가 0(Nothing)이면 모든 레이어를 검사, 트리거 포함 강제 설정
            var filter = new ContactFilter2D();
            filter.useTriggers = true;
            filter.useLayerMask = interactLayer != 0;
            filter.layerMask   = interactLayer != 0 ? interactLayer : (LayerMask)~0;

            var results = new System.Collections.Generic.List<Collider2D>();
            Physics2D.OverlapCircle(transform.position, interactRange, filter, results);

            TrashItem nearestTrash = null;
            TrashBin  nearestBin  = null;
            UpgradeShop nearestShop = null;
            float minDist = float.MaxValue;

            foreach (var hit in results)
            {
                if (hit.gameObject == gameObject) continue;  // 자기 자신 제외
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist >= minDist) continue;

                var trash = hit.GetComponent<TrashItem>();
                var bin   = hit.GetComponent<TrashBin>();
                var shop  = hit.GetComponent<UpgradeShop>();

                if      (trash != null && !trash.IsPickedUp) { nearestTrash = trash; nearestBin = null; nearestShop = null; minDist = dist; }
                else if (bin   != null)                      { nearestBin   = bin;   nearestTrash = null; nearestShop = null; minDist = dist; }
                else if (shop  != null)                      { nearestShop  = shop;  nearestTrash = null; nearestBin = null; minDist = dist; }
            }

            if      (nearestTrash != null) TryPickupTrash(nearestTrash);
            else if (nearestBin   != null) OpenBinSelection(nearestBin);
            else if (nearestShop  != null) nearestShop.TryUpgrade();
            else Debug.Log($"[F키] 범위 내 오브젝트 없음. 감지된 콜라이더={results.Count}, 범위={interactRange}");
        }

        private void TryPickupTrash(TrashItem trash)
        {
            if (bagInventory == null) return;

            if (!bagInventory.CanAdd())
            {
                feedbackUI?.ShowMessage("Bag is full!");
                return;
            }

            bagInventory.AddTrash(trash);
            trash.OnPickedUp();
            controller.TriggerPickupAnimation();
            feedbackUI?.ShowMessage($"{GetTrashName(trash.TrashType)} collected!");
        }

        private void OpenBinSelection(TrashBin bin)
        {
            if (bagInventory == null || bagInventory.Count == 0)
            {
                feedbackUI?.ShowMessage("Bag is empty.");
                return;
            }
            binSelectionUI?.Open(bin);
        }

        // BinSelectionUI에서 호출됨 — 선택한 BinType이 맞는지 체크
        private void HandleDeposit(int itemIndex, BinType selectedBinType)
        {
            bagInventory.SetSelectedIndex(itemIndex);
            TrashItem selected = bagInventory.GetSelectedItem();
            if (selected == null) return;

            bool correct = TrashBinMapping.GetCorrectBin(selected.TrashType) == selectedBinType;

            selected.OnDeposited(correct);
            bagInventory.RemoveSelected();

            if (correct)
            {
                int coins = TrashBinMapping.GetCoinValue(selected.TrashType);
                ScoreManager.Instance?.AddScore(10);
                ScoreManager.Instance?.AddCoins(coins);
                feedbackUI?.ShowMessage($"Correct! +{coins} coins");
            }
            else
            {
                ScoreManager.Instance?.SubtractScore(5);
                feedbackUI?.ShowMessage("Wrong! -5 pts");
            }
        }

        private static string GetTrashName(TrashType t) => t switch
        {
            TrashType.Paper   => "Paper",
            TrashType.Can     => "Can",
            TrashType.Plastic => "Plastic",
            TrashType.Vinyl   => "Vinyl",
            TrashType.Food    => "Food",
            TrashType.Net     => "Net",
            TrashType.Oil     => "Oil",
            _                 => t.ToString()
        };

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactRange);
        }
    }
}
