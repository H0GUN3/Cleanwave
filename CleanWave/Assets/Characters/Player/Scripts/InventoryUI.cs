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

            slot.enabled = true;
            if (!insideCapacity)
            {
                slot.sprite = emptySlotSprite;
                slot.color = lockedSlotColor;
            }
            else if (filled)
            {
                slot.sprite = trashSlotSprite != null ? trashSlotSprite : emptySlotSprite;
                slot.color = Color.white;
            }
            else if (markSelected && i == 0)
            {
                slot.sprite = selectedSlotSprite != null ? selectedSlotSprite : emptySlotSprite;
                slot.color = Color.white;
            }
            else
            {
                slot.sprite = emptySlotSprite;
                slot.color = Color.white;
            }
        }
    }
}
