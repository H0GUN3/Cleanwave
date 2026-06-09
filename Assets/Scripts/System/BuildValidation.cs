using CleanWave.Data;
using UnityEngine;

namespace CleanWave.Systems
{
    public class BuildValidation : MonoBehaviour
    {
        [SerializeField] private StageConfig stageConfig;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private InventoryManager inventoryManager;

        [ContextMenu("Validate Runtime References")]
        public void ValidateRuntimeReferences()
        {
            if (stageConfig == null) Debug.LogWarning("StageConfig is not assigned.");
            if (scoreManager == null) Debug.LogWarning("ScoreManager is not assigned.");
            if (inventoryManager == null) Debug.LogWarning("InventoryManager is not assigned.");
        }
    }
}
