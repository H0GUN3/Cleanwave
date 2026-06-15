using CleanWave.Core;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 쓰레기 오브젝트. 플레이어가 범위 안에서 E를 누르면 줍는다.
/// PlayerBag.TryAddTrash()가 true일 때만 제거된다.
/// </summary>
public class TrashPickup : MonoBehaviour
{
    const float SfxVolumeBoost = 1.3f;

    [SerializeField] float pickupRange = 1.2f;
    [SerializeField] TrashType trashType = TrashType.Paper;
    [SerializeField] bool inferTrashTypeFromName = true;
    [SerializeField] SpriteRenderer outlineRenderer;
    [SerializeField] Color outlineColor = new Color(1f, 0.45f, 0f, 1f);
    [SerializeField] AudioClip pickupSfx;
    [SerializeField, Range(0f, 1f)] float pickupSfxVolume = 1f;

    static PlayerBag cachedBag;
    static CurrentZoneTracker cachedZoneTracker;

    void Awake()
    {
        ApplyOutlineColor();
    }

    void OnValidate()
    {
        ApplyOutlineColor();
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null || !keyboard.eKey.wasPressedThisFrame)
            return;

        PlayerBag bag = FindPlayerBag();
        if (bag == null)
            return;

        float dist = Vector2.Distance(transform.position, bag.transform.position);
        if (dist > pickupRange)
            return;

        CleanWaveZoneType zoneType = GetCurrentZone();
        CleanWaveZoneType trashFamilyZone = TrashSortingUtility.InferTrashFamilyZone(name, zoneType);
        if (bag.TryAddTrash(GetEffectiveTrashType(trashFamilyZone), zoneType, GetInventoryIcon()))
        {
            if (pickupSfx != null)
                AudioSource.PlayClipAtPoint(pickupSfx, transform.position, Mathf.Clamp01(pickupSfxVolume * SfxVolumeBoost));

            Destroy(gameObject);
        }
        // 가방이 가득 차면 TryAddTrash가 false → 쓰레기를 제거하지 않는다.
    }

    PlayerBag FindPlayerBag()
    {
        if (cachedBag == null)
            cachedBag = FindFirstObjectByType<PlayerBag>();
        return cachedBag;
    }

    CleanWaveZoneType GetCurrentZone()
    {
        if (cachedZoneTracker == null)
            cachedZoneTracker = FindFirstObjectByType<CurrentZoneTracker>();

        return cachedZoneTracker != null ? cachedZoneTracker.CurrentZone : CleanWaveZoneType.City;
    }

    TrashType GetEffectiveTrashType(CleanWaveZoneType zoneType)
    {
        if (!inferTrashTypeFromName)
            return trashType;

        return TrashSortingUtility.InferTrashType(name, zoneType, trashType);
    }

    Sprite GetInventoryIcon()
    {
        foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>(true))
        {
            if (renderer == outlineRenderer || renderer.name == "Outline")
                continue;

            if (renderer.sprite != null)
                return renderer.sprite;
        }

        return null;
    }

    void ApplyOutlineColor()
    {
        SpriteRenderer renderer = FindOutlineRenderer();
        if (renderer != null)
            renderer.color = outlineColor;
    }

    SpriteRenderer FindOutlineRenderer()
    {
        if (outlineRenderer != null)
            return outlineRenderer;

        foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>(true))
        {
            if (renderer.name == "Outline")
            {
                outlineRenderer = renderer;
                return outlineRenderer;
            }
        }

        return null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
