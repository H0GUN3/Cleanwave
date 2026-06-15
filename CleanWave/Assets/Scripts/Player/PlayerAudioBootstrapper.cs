using CleanWave.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PlayerAudioBootstrapper
{
    const string FootstepClipPath = "Audio/Game/footstep";
    static bool sceneLoadedSubscribed;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AttachFootstepAudio()
    {
        SubscribeSceneLoaded();

        if (!GameplaySceneUtility.IsGameplayScene())
            return;

        PlayerMovement player = Object.FindFirstObjectByType<PlayerMovement>();
        if (player == null)
            return;

        PlayerFootstepAudio footstepAudio = player.GetComponent<PlayerFootstepAudio>();
        if (footstepAudio == null)
            footstepAudio = player.gameObject.AddComponent<PlayerFootstepAudio>();

        footstepAudio.Configure(Resources.Load<AudioClip>(FootstepClipPath));
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
        AttachFootstepAudio();
    }
}
