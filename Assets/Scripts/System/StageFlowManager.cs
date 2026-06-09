using CleanWave.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CleanWave.Systems
{
    public class StageFlowManager : MonoBehaviour
    {
        [SerializeField] private StageCatalog stageCatalog;
        [SerializeField] private SaveManager saveManager;

        public void LoadStageByIndex(int index)
        {
            if (stageCatalog == null || stageCatalog.Stages == null || index < 0 || index >= stageCatalog.Stages.Length)
            {
                return;
            }

            if (saveManager != null && saveManager.CurrentState != null)
            {
                var unlockIndex = Mathf.Max(1, saveManager.CurrentState.UnlockedStageIndex) - 1;
                if (index > unlockIndex)
                {
                    return;
                }
            }

            var stage = stageCatalog.Stages[index];
            if (!string.IsNullOrWhiteSpace(stage.SceneName))
            {
                SceneManager.LoadScene(stage.SceneName);
            }
        }

        public void LoadNextStage(string currentStageId)
        {
            if (stageCatalog == null || stageCatalog.Stages == null)
            {
                return;
            }

            for (int i = 0; i < stageCatalog.Stages.Length; i++)
            {
                if (stageCatalog.Stages[i].StageId != currentStageId)
                {
                    continue;
                }

                var next = i + 1;
                if (next < stageCatalog.Stages.Length)
                {
                    LoadStageByIndex(next);
                }
                return;
            }
        }
    }
}
