using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// PlayerBag을 단일 데이터 소스로 사용하는 인벤토리 UI.
/// Q 키로 전체 창을 열고 닫으며, 화면 전체를 덮는 dim/overlay는 만들지 않는다.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] PlayerBag playerBag;

    [Header("Always Visible Hotbar")]
    [SerializeField] GameObject hotbarRoot;
    [SerializeField] UnityEngine.UI.Image[] hotbarSlots;
    [SerializeField] UnityEngine.UI.Text hotbarCountText;

    [Header("Full Inventory")]
    [SerializeField] GameObject fullInventoryRoot;
    [SerializeField] UnityEngine.UI.Image[] fullInventorySlots;
    [SerializeField] UnityEngine.UI.Text fullInventoryCountText;

    [Header("Slot Sprites")]
    [SerializeField] Sprite emptySlotSprite;
    [SerializeField] Sprite selectedSlotSprite;
    [SerializeField] Sprite trashSlotSprite;

    [Header("Fallback Colors")]
    [SerializeField] Color lockedSlotColor = new Color(0f, 0f, 0f, 0.25f);

    [Header("Audio")]
    [SerializeField] AudioClip inventoryOpenSfx;
    [SerializeField, Range(0f, 1f)] float inventoryOpenSfxVolume = 0.8f;

    void Awake()
    {
        if (fullInventoryRoot != null)
            fullInventoryRoot.SetActive(false);
    }

    void OnEnable()
    {
        if (playerBag != null)
            playerBag.OnBagChanged.AddListener(Refresh);
    }

    void OnDisable()
    {
        if (playerBag != null)
            playerBag.OnBagChanged.RemoveListener(Refresh);
    }

    void Start()
    {
        if (playerBag == null)
            playerBag = FindFirstObjectByType<PlayerBag>();

        if (playerBag != null)
        {
            playerBag.OnBagChanged.RemoveListener(Refresh);
            playerBag.OnBagChanged.AddListener(Refresh);
            Refresh(playerBag.CurrentCount, playerBag.MaxCapacity);
        }
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null || fullInventoryRoot == null)
            return;

        if (keyboard.qKey.wasPressedThisFrame)
        {
            bool shouldOpen = !fullInventoryRoot.activeSelf;
            fullInventoryRoot.SetActive(shouldOpen);

            if (shouldOpen && inventoryOpenSfx != null)
                AudioSource.PlayClipAtPoint(inventoryOpenSfx, Vector3.zero, inventoryOpenSfxVolume);
        }
    }

    void Refresh(int currentCount, int maxCapacity)
    {
        RefreshSlots(hotbarSlots, currentCount, Mathf.Min(maxCapacity, 8), true, 8);
        RefreshSlots(fullInventorySlots, currentCount, maxCapacity, false, 24);

        string countText = $"쓰레기 {currentCount} / {maxCapacity}";
        if (hotbarCountText != null)
            hotbarCountText.text = countText;
        if (fullInventoryCountText != null)
            fullInventoryCountText.text = countText;
    }

    void RefreshSlots(UnityEngine.UI.Image[] slots, int currentCount, int maxCapacity, bool markSelected, int visibleSlotCount)
    {
        if (slots == null)
            return;

        for (int i = 0; i < slots.Length; i++)
        {
            UnityEngine.UI.Image slot = slots[i];
            if (slot == null)
                continue;

            if (i >= visibleSlotCount)
            {
                slot.enabled = false;
                continue;
            }

            bool insideCapacity = i < maxCapacity;
            bool filled = i < currentCount;
            UnityEngine.UI.Image itemIcon = GetItemIcon(slot);

            slot.enabled = true;
            slot.sprite = GetSlotFrameSprite(filled, markSelected && i == 0);
            if (!insideCapacity)
            {
                slot.color = lockedSlotColor;
                SetItemIcon(itemIcon, null, false);
            }
            else if (filled)
            {
                slot.color = Color.white;
                SetItemIcon(itemIcon, GetSlotSprite(i), true);
            }
            else if (markSelected && i == 0)
            {
                slot.color = Color.white;
                SetItemIcon(itemIcon, null, false);
            }
            else
            {
                slot.color = Color.white;
                SetItemIcon(itemIcon, null, false);
            }
        }
    }

    Sprite GetSlotFrameSprite(bool filled, bool selected)
    {
        if (selected && selectedSlotSprite != null)
            return selectedSlotSprite;

        return emptySlotSprite;
    }

    UnityEngine.UI.Image GetItemIcon(UnityEngine.UI.Image slot)
    {
        Transform existing = slot.transform.Find("ItemIcon");
        if (existing != null)
            return existing.GetComponent<UnityEngine.UI.Image>();

        GameObject iconObject = new GameObject("ItemIcon");
        iconObject.transform.SetParent(slot.transform, false);

        RectTransform rect = iconObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = slot.rectTransform.rect.size * 0.62f;

        UnityEngine.UI.Image icon = iconObject.AddComponent<UnityEngine.UI.Image>();
        icon.raycastTarget = false;
        icon.preserveAspect = true;
        return icon;
    }

    void SetItemIcon(UnityEngine.UI.Image itemIcon, Sprite sprite, bool visible)
    {
        if (itemIcon == null)
            return;

        itemIcon.enabled = visible && sprite != null;
        itemIcon.sprite = sprite;
        itemIcon.color = Color.white;
    }

    Sprite GetSlotSprite(int itemIndex)
    {
        if (playerBag != null && playerBag.TryGetTrashAt(itemIndex, out PlayerBag.BagItem item) && item.IconSprite != null)
            return item.IconSprite;

        return trashSlotSprite != null ? trashSlotSprite : emptySlotSprite;
    }
}
