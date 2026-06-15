using CleanWave.Core;
using UnityEngine;

public class ZoneProgressGate : MonoBehaviour
{
    const string ZoneUnlockClipPath = "Audio/Game/zone_unlock";

    [SerializeField] CleanWaveZoneType requiredZone = CleanWaveZoneType.City;
    [SerializeField, Range(0, 100)] int requiredCollectedPercent = CleanWaveGameConstants.PurificationPercentToUnlockNextZone;
    [SerializeField] Collider2D blockingCollider;
    [SerializeField] GameObject closedVisual;
    [SerializeField] GateSpriteAnimator gateAnimator;
    [SerializeField] bool logStateChanges = true;
    [SerializeField, Range(0f, 1f)] float unlockVolume = 1f;

    ZoneTrashProgress progress;
    bool isOpen;
    AudioClip zoneUnlockClip;

    public bool IsOpen => isOpen;

    void Awake()
    {
        if (blockingCollider == null)
            blockingCollider = GetComponent<Collider2D>();

        if (gateAnimator == null)
            gateAnimator = GetComponent<GateSpriteAnimator>();
    }

    void OnEnable()
    {
        BindProgressTracker();
    }

    void Start()
    {
        BindProgressTracker();
        EvaluateGate();
    }

    void OnDisable()
    {
        if (progress != null)
            progress.ProgressChanged -= HandleProgressChanged;
    }

    public void EvaluateGate()
    {
        if (progress == null)
        {
            SetOpen(false);
            return;
        }

        SetOpen(progress.GetCollectedPercent(requiredZone) >= requiredCollectedPercent);
    }

    void BindProgressTracker()
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

    void HandleProgressChanged(CleanWaveZoneType zoneType, int collected, int total, int percent)
    {
        if (zoneType != requiredZone)
            return;

        SetOpen(percent >= requiredCollectedPercent);
    }

    void SetOpen(bool open)
    {
        bool changed = isOpen != open;
        if (!changed)
        {
            if (isOpen)
                DisableColliderIfStillOpen();
            return;
        }

        isOpen = open;

        if (isOpen)
        {
            SortingFeedbackPopup.ShowMessage("다음 구역이 개방되었습니다.");
            PlayZoneUnlockSound();

            if (gateAnimator != null)
            {
                gateAnimator.PlayOpen(DisableColliderIfStillOpen);
            }
            else
            {
                DisableColliderIfStillOpen();

                if (closedVisual != null)
                    closedVisual.SetActive(false);
            }
        }
        else
        {
            if (blockingCollider != null)
                blockingCollider.enabled = true;

            if (gateAnimator != null)
                gateAnimator.ShowClosed();
            else if (closedVisual != null)
                closedVisual.SetActive(true);
        }

        if (changed && logStateChanges)
            Debug.Log($"[ZoneProgressGate] {name} {(isOpen ? "opened" : "closed")}: {requiredZone} >= {requiredCollectedPercent}%", this);
    }

    void DisableColliderIfStillOpen()
    {
        if (!isOpen || blockingCollider == null)
            return;

        blockingCollider.enabled = false;
    }

    void PlayZoneUnlockSound()
    {
        if (zoneUnlockClip == null)
            zoneUnlockClip = Resources.Load<AudioClip>(ZoneUnlockClipPath);

        if (zoneUnlockClip != null)
            AudioSource.PlayClipAtPoint(zoneUnlockClip, transform.position, unlockVolume);
    }
}
