using UnityEngine.SceneManagement;

namespace CleanWave.Core
{
    public static class GameplaySceneUtility
    {
        public const string GameplaySceneName = "MainScene";

        public static bool IsGameplayScene()
        {
            return SceneManager.GetActiveScene().name == GameplaySceneName;
        }
    }
}
