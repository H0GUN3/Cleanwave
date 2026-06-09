using CleanWave.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CleanWave.Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private StageManager stageManager;
        [SerializeField] private SaveManager saveManager;
        [SerializeField] private string resultSceneName = SceneNames.Result;
        [SerializeField] private string mainMenuSceneName = SceneNames.MainMenu;

        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            if (stageManager != null)
            {
                stageManager.StageEnded += HandleStageEnded;
            }
        }

        private void OnDisable()
        {
            if (stageManager != null)
            {
                stageManager.StageEnded -= HandleStageEnded;
            }
        }

        private void HandleStageEnded(bool success)
        {
            saveManager?.SaveLastStageResult(success);
            if (!string.IsNullOrWhiteSpace(resultSceneName))
            {
                SceneManager.LoadScene(resultSceneName);
            }
        }

        public void LoadMainMenu()
        {
            if (!string.IsNullOrWhiteSpace(mainMenuSceneName))
            {
                SceneManager.LoadScene(mainMenuSceneName);
            }
        }
    }
}
