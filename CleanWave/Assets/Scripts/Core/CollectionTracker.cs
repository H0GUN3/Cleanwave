using UnityEngine;
using UnityEngine.Events;

public class CollectionTracker : MonoBehaviour
{
    public static CollectionTracker Instance { get; private set; }

    public UnityEvent<float> OnProgressChanged = new UnityEvent<float>();

    int totalTrash;
    int pickedCount;

    public static CollectionTracker EnsureInstance()
    {
        if (Instance != null)
            return Instance;

        CollectionTracker found = FindFirstObjectByType<CollectionTracker>();
        if (found != null)
        {
            Instance = found;
            return Instance;
        }

        GameObject trackerObject = new GameObject("CollectionTracker");
        return trackerObject.AddComponent<CollectionTracker>();
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void SetTotalTrash(int total)
    {
        totalTrash = total;
        pickedCount = 0;
        OnProgressChanged.Invoke(0f);
    }

    public void RegisterPick()
    {
        if (totalTrash <= 0) return;
        pickedCount++;
        float progress = Mathf.Clamp01((float)pickedCount / totalTrash);
        OnProgressChanged.Invoke(progress);
    }

    public void SetProgress(float progress)
    {
        OnProgressChanged.Invoke(Mathf.Clamp01(progress));
    }
}
