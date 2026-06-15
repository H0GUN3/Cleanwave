using CleanWave.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class TrashSortingBootstrapper
{
    static bool sceneLoadedSubscribed;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AttachSortingBins()
    {
        SubscribeSceneLoaded();

        if (!GameplaySceneUtility.IsGameplayScene())
            return;

        CleanWaveRunState.EnsureInstance();
        SortingFeedbackPopup.EnsureInstance();

        Transform[] sceneObjects = Object.FindObjectsByType<Transform>(FindObjectsSortMode.None);
        foreach (Transform sceneObject in sceneObjects)
        {
            if (!sceneObject.name.StartsWith("Bin_", System.StringComparison.OrdinalIgnoreCase))
                continue;

            if (sceneObject.GetComponent<Collider2D>() == null)
                continue;

            if (sceneObject.GetComponent<TrashSortingBin>() == null)
                sceneObject.gameObject.AddComponent<TrashSortingBin>();
        }
    }

    static void SubscribeSceneLoaded()
    {
        if (sceneLoadedSubscribed)
            return;

        SceneManager.sceneLoaded += HandleSceneLoaded;
        sceneLoadedSubscribed = true;
    }

    static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AttachSortingBins();
    }
}
