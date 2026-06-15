using CleanWave.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class ZoneBgmController : MonoBehaviour
{
    const string CityClipPath = "Audio/Game/bgm_city";
    const string RiverClipPath = "Audio/Game/bgm_river";
    const string CoastClipPath = "Audio/Game/bgm_coast";

    [SerializeField, Range(0f, 1f)] float volume = 0.45f;

    AudioSource audioSource;
    CurrentZoneTracker zoneTracker;
    AudioClip cityClip;
    AudioClip riverClip;
    AudioClip coastClip;
    static bool sceneLoadedSubscribed;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void EnsureRuntimeController()
    {
        SubscribeSceneLoaded();

        if (!GameplaySceneUtility.IsGameplayScene())
            return;

        if (Object.FindFirstObjectByType<ZoneBgmController>() != null)
            return;

        GameObject controller = new GameObject("ZoneBgmController");
        controller.AddComponent<ZoneBgmController>();
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
        EnsureRuntimeController();
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f;

        cityClip = Resources.Load<AudioClip>(CityClipPath);
        riverClip = Resources.Load<AudioClip>(RiverClipPath);
        coastClip = Resources.Load<AudioClip>(CoastClipPath);
    }

    void OnEnable()
    {
        BindZoneTracker();
    }

    void Start()
    {
        BindZoneTracker();
        PlayZone(zoneTracker != null ? zoneTracker.CurrentZone : CleanWaveZoneType.City);
    }

    void OnDisable()
    {
        if (zoneTracker != null)
            zoneTracker.OnZoneChanged.RemoveListener(PlayZone);
    }

    void BindZoneTracker()
    {
        CurrentZoneTracker found = Object.FindFirstObjectByType<CurrentZoneTracker>();
        if (found == null || found == zoneTracker)
            return;

        if (zoneTracker != null)
            zoneTracker.OnZoneChanged.RemoveListener(PlayZone);

        zoneTracker = found;
        zoneTracker.OnZoneChanged.AddListener(PlayZone);
    }

    void PlayZone(CleanWaveZoneType zoneType)
    {
        AudioClip clip = GetClip(zoneType);
        if (clip == null || audioSource == null)
            return;

        if (audioSource.clip == clip && audioSource.isPlaying)
            return;

        if (clip.loadState == AudioDataLoadState.Unloaded)
            clip.LoadAudioData();

        audioSource.clip = clip;
        audioSource.Play();
    }

    AudioClip GetClip(CleanWaveZoneType zoneType)
    {
        switch (zoneType)
        {
            case CleanWaveZoneType.River:
                return riverClip;
            case CleanWaveZoneType.Coast:
                return coastClip;
            default:
                return cityClip;
        }
    }
}
