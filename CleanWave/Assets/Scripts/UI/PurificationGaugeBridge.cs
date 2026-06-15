using CleanWave.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PurificationGaugeBridge : MonoBehaviour
{
    static PurificationGaugeBridge instance;
    static bool sceneLoadedSubscribed;

    ZoneTrashProgress progress;
    CurrentZoneTracker zoneTracker;
    CollectionTracker collectionTracker;
    CleanWaveZoneType currentZone = CleanWaveZoneType.City;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void EnsureRuntimeBridge()
    {
        SubscribeSceneLoaded();

        if (!GameplaySceneUtility.IsGameplayScene())
            return;

        if (FindFirstObjectByType<PurificationGaugeBridge>() != null)
            return;

        GameObject bridgeObject = new GameObject("PurificationGaugeBridge");
        bridgeObject.AddComponent<PurificationGaugeBridge>();
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
        EnsureRuntimeBridge();
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        collectionTracker = CollectionTracker.EnsureInstance();
    }

    void OnEnable()
    {
        BindProgress();
        BindZoneTracker();
    }

    void Start()
    {
        BindProgress();
        BindZoneTracker();
        PushCurrentZoneProgress();
    }

    void OnDisable()
    {
        if (progress != null)
            progress.ProgressChanged -= HandleProgressChanged;
        if (zoneTracker != null)
            zoneTracker.OnZoneChanged.RemoveListener(HandleZoneChanged);
    }

    void BindProgress()
    {
        ZoneTrashProgress found = ZoneTrashProgress.Instance;
        if (found == null)
            found = FindFirstObjectByType<ZoneTrashProgress>();

        if (found == null || found == progress)
            return;

        if (progress != null)
            progress.ProgressChanged -= HandleProgressChanged;

        progress = found;
        progress.ProgressChanged += HandleProgressChanged;
    }

    void BindZoneTracker()
    {
        CurrentZoneTracker found = FindFirstObjectByType<CurrentZoneTracker>();
        if (found == null || found == zoneTracker)
            return;

        if (zoneTracker != null)
            zoneTracker.OnZoneChanged.RemoveListener(HandleZoneChanged);

        zoneTracker = found;
        currentZone = zoneTracker.CurrentZone;
        zoneTracker.OnZoneChanged.AddListener(HandleZoneChanged);
    }

    void HandleProgressChanged(CleanWaveZoneType zoneType, int collected, int total, int percent)
    {
        if (zoneType == currentZone)
            PushProgress(percent);
    }

    void HandleZoneChanged(CleanWaveZoneType zoneType)
    {
        currentZone = zoneType;
        PushCurrentZoneProgress();
    }

    void PushCurrentZoneProgress()
    {
        if (progress == null)
            return;

        PushProgress(progress.GetCollectedPercent(currentZone));
    }

    void PushProgress(int percent)
    {
        if (collectionTracker == null)
            collectionTracker = CollectionTracker.EnsureInstance();

        collectionTracker.SetProgress(percent / 100f);
    }
}
