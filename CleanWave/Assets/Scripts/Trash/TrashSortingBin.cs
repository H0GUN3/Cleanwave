using CleanWave.Core;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class TrashSortingBin : MonoBehaviour
{
    const string SortSuccessClipPath = "Audio/Game/sort_success";
    const float SfxVolumeBoost = 1.3f;

    [SerializeField] BinType binType = BinType.General;
    [SerializeField] bool inferBinTypeFromName = true;
    [SerializeField] float fallbackInteractionRange = 1.35f;
    [SerializeField] bool logSorting = true;
    [SerializeField, Range(0f, 1f)] float sortSfxVolume = 1f;

    PlayerBag playerBagInRange;
    PlayerBag cachedPlayerBag;
    static AudioClip sortSuccessClip;

    public BinType CurrentBinType => inferBinTypeFromName ? TrashSortingUtility.InferBinType(name, binType) : binType;

    void Awake()
    {
        Collider2D binCollider = GetComponent<Collider2D>();
        if (binCollider != null)
            binCollider.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerBag bag = other.GetComponentInParent<PlayerBag>();
        if (bag != null)
            playerBagInRange = bag;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (playerBagInRange == null)
        {
            PlayerBag bag = other.GetComponentInParent<PlayerBag>();
            if (bag != null)
                playerBagInRange = bag;
        }
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null || !keyboard.eKey.wasPressedThisFrame)
            return;

        PlayerBag bag = GetInteractableBag();
        if (bag == null)
            return;

        playerBagInRange = bag;
        TrySortNextTrash();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        PlayerBag bag = other.GetComponentInParent<PlayerBag>();
        if (bag != null && bag == playerBagInRange)
            playerBagInRange = null;
    }

    public bool TrySortNextTrash()
    {
        if (playerBagInRange == null)
            return false;

        if (!playerBagInRange.TryPeekNextTrash(out PlayerBag.BagItem item))
        {
            SortingFeedbackPopup.ShowMessage("가방이 비어 있습니다.");
            return false;
        }

        BinType expectedBin = CleanWaveRules.GetExpectedBin(item.TrashType);
        BinType currentBin = CurrentBinType;
        if (expectedBin != currentBin)
        {
            CleanWaveRunState.EnsureInstance().ApplyWrongSortPenalty();
            SortingFeedbackPopup.ShowWrongSort(item.TrashType, expectedBin, currentBin);
            return false;
        }

        if (!playerBagInRange.TryRemoveNextTrash(out item))
            return false;

        PlaySortSuccessSound();

        CleanWaveRunState.EnsureInstance().RecordCorrectSort(item.TrashType);

        ZoneTrashProgress progress = ZoneTrashProgress.Instance;
        if (progress == null)
            progress = FindFirstObjectByType<ZoneTrashProgress>();
        if (progress != null)
            progress.RecordCollected(item.ZoneType);

        SortingFeedbackPopup.ShowCorrectSort(item.TrashType, currentBin);

        if (logSorting)
            Debug.Log($"[TrashSortingBin] Sorted {item.TrashType} into {currentBin} at {name}.", this);

        return true;
    }

    PlayerBag GetInteractableBag()
    {
        if (playerBagInRange != null)
            return playerBagInRange;

        if (cachedPlayerBag == null)
            cachedPlayerBag = FindFirstObjectByType<PlayerBag>();

        if (cachedPlayerBag == null)
            return null;

        float distance = Vector2.Distance(transform.position, cachedPlayerBag.transform.position);
        return distance <= fallbackInteractionRange ? cachedPlayerBag : null;
    }

    void PlaySortSuccessSound()
    {
        if (sortSuccessClip == null)
            sortSuccessClip = Resources.Load<AudioClip>(SortSuccessClipPath);

        if (sortSuccessClip == null)
            return;

        if (sortSuccessClip.loadState == AudioDataLoadState.Unloaded)
            sortSuccessClip.LoadAudioData();

        AudioSource.PlayClipAtPoint(sortSuccessClip, transform.position, Mathf.Clamp01(sortSfxVolume * SfxVolumeBoost));
    }
}
