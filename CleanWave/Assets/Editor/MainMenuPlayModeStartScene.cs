#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class MainMenuPlayModeStartScene
{
    const string MainMenuScenePath = "Assets/Scenes/MainMenu.unity";

    static MainMenuPlayModeStartScene()
    {
        ApplyStartScene();
        EditorApplication.delayCall += ApplyStartScene;
    }

    [InitializeOnLoadMethod]
    static void Initialize()
    {
        ApplyStartScene();
        EditorApplication.delayCall += ApplyStartScene;
    }

    public static void ApplyStartScene()
    {
        SceneAsset mainMenuScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(MainMenuScenePath);
        if (mainMenuScene == null)
        {
            Debug.LogWarning($"[MainMenuPlayModeStartScene] Main menu scene not found: {MainMenuScenePath}");
            return;
        }

        if (EditorSceneManager.playModeStartScene != mainMenuScene)
            EditorSceneManager.playModeStartScene = mainMenuScene;
    }
}
#endif
