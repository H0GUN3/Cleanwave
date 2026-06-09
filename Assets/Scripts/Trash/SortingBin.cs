using CleanWave.Systems;
using UnityEngine;

namespace CleanWave.Trash
{
    [RequireComponent(typeof(Collider2D))]
    public class SortingBin : MonoBehaviour
    {
        [SerializeField] private TrashType acceptedType = TrashType.Plastic; // backward compatibility for old scenes
        [SerializeField] private System.Collections.Generic.List<TrashType> acceptedTypes = new();
        [SerializeField] private InventoryManager inventoryManager;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private StageManager stageManager;
        [SerializeField] private float correctCleanProgress = 1.5f;
        [SerializeField] private float wrongCleanPenalty = 0.5f;

        private bool _playerInRange;

        private void Update()
        {
            if (_playerInRange && Input.GetKeyDown(KeyCode.E))
            {
                ProcessSort();
            }
        }

        private void ProcessSort()
        {
            if (inventoryManager == null || scoreManager == null)
            {
                return;
            }

            var candidateTypes = acceptedTypes != null && acceptedTypes.Count > 0
                ? acceptedTypes
                : new System.Collections.Generic.List<TrashType> { acceptedType };

            foreach (var type in candidateTypes)
            {
                if (inventoryManager.GetCount(type) > 0 && inventoryManager.TryRemoveTrash(type))
                {
                    scoreManager.AddCorrectSort();
                    stageManager?.AddCleanProgress(correctCleanProgress);
                    return;
                }
            }

            if (HasAnyTrashInInventory())
            {
                scoreManager.AddWrongSort();
                stageManager?.AddCleanProgress(-wrongCleanPenalty);
            }
        }

        private bool HasAnyTrashInInventory()
        {
            foreach (var pair in inventoryManager.TrashCounts)
            {
                if (pair.Value > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = false;
            }
        }
    }
}
