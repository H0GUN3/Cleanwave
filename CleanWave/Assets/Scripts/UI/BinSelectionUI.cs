using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

namespace CleanWave
{
    public class BinSelectionUI : MonoBehaviour
    {
        [SerializeField] private GameObject       panel;
        [SerializeField] private TextMeshProUGUI  trashNameText;
        [SerializeField] private Image            trashIconImage;
        [SerializeField] private Transform        binButtonContainer;
        [SerializeField] private Button           closeButton;

        private BagInventory bagInventory;
        private readonly List<GameObject> builtButtons = new();

        public bool IsOpen => panel != null && panel.activeSelf;

        public static bool IsPopupOpen { get; private set; }

        // (bagItemIndex=0, chosenBinType)
        public event Action<int, BinType> OnDepositRequested;
        public event Action OnClosed;

        private static readonly (BinType type, string label, Color color)[] BIN_DEFS =
        {
            (BinType.Paper,   "Paper",   new Color(0.95f, 0.88f, 0.15f)),
            (BinType.Can,     "Can",     new Color(0.20f, 0.50f, 1.00f)),
            (BinType.Plastic, "Plastic", new Color(0.10f, 0.88f, 0.88f)),
            (BinType.General, "General", new Color(0.55f, 0.55f, 0.55f)),
            (BinType.Special, "Special", new Color(0.90f, 0.20f, 0.20f)),
        };

        private void Awake()
        {
            if (closeButton) closeButton.onClick.AddListener(Close);
            if (panel != null) panel.SetActive(false);
        }

        // called lazily on first Open() so binButtonContainer is guaranteed wired
        private void BuildBinButtons()
        {
            if (builtButtons.Count > 0) return;
            if (binButtonContainer == null) return;

            foreach (var (binType, label, color) in BIN_DEFS)
            {
                var bt    = binType;
                var btnGo = CreateBinButton(label, color);
                btnGo.transform.SetParent(binButtonContainer, false);
                builtButtons.Add(btnGo);

                var btn = btnGo.GetComponent<Button>();
                if (btn) btn.onClick.AddListener(() => OnBinButtonClicked(bt));
            }

            // Force layout rebuild so buttons are positioned and clickable immediately
            if (binButtonContainer is RectTransform bcRect)
                LayoutRebuilder.ForceRebuildLayoutImmediate(bcRect);
        }

        private static GameObject CreateBinButton(string label, Color bgColor)
        {
            var root = new GameObject(label);
            var rect = root.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(130, 160);

            var img = root.AddComponent<Image>();
            img.color = bgColor;

            var btn = root.AddComponent<Button>();
            btn.targetGraphic = img;

            // Color block – darken on hover/press so button feels interactive
            var cb = ColorBlock.defaultColorBlock;
            cb.normalColor      = Color.white;
            cb.highlightedColor = new Color(1f, 1f, 1f, 0.85f);
            cb.pressedColor     = new Color(0.7f, 0.7f, 0.7f);
            btn.colors          = cb;

            // Bin label (bottom 35%)
            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(root.transform, false);
            var lr = labelGo.AddComponent<RectTransform>();
            lr.anchorMin = new Vector2(0f, 0f);
            lr.anchorMax = new Vector2(1f, 0.38f);
            lr.offsetMin = lr.offsetMax = Vector2.zero;
            var tmp = labelGo.AddComponent<TextMeshProUGUI>();
            tmp.text      = label;
            tmp.fontSize  = 20;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color     = Color.white;
            tmp.enableWordWrapping = false;

            // Bin icon area (top 60%) — plain solid color block as placeholder
            var iconGo = new GameObject("BinIcon");
            iconGo.transform.SetParent(root.transform, false);
            var ir = iconGo.AddComponent<RectTransform>();
            ir.anchorMin = new Vector2(0.1f, 0.38f);
            ir.anchorMax = new Vector2(0.9f, 0.92f);
            ir.offsetMin = ir.offsetMax = Vector2.zero;
            var iconImg = iconGo.AddComponent<Image>();
            iconImg.color = new Color(bgColor.r * 0.6f, bgColor.g * 0.6f, bgColor.b * 0.6f);
            iconImg.raycastTarget = false;

            return root;
        }

        public void Setup(BagInventory bag)
        {
            bagInventory = bag;
        }

        public void Open(TrashBin _bin)
        {
            BuildBinButtons();
            RefreshTrashDisplay();
            if (panel != null) panel.SetActive(true);
            IsPopupOpen = true;
        }

        public void Close()
        {
            if (panel != null) panel.SetActive(false);
            IsPopupOpen = false;
            OnClosed?.Invoke();
        }

        private void RefreshTrashDisplay()
        {
            if (bagInventory == null || bagInventory.Count == 0) { Close(); return; }

            var item = bagInventory.GetItemAt(0);
            if (item == null) { Close(); return; }

            if (trashNameText)
                trashNameText.text = $"Trash: {TrashLabel(item.TrashType)}";

            if (trashIconImage)
            {
                if (item.TryGetComponent<SpriteRenderer>(out var sr) && sr.sprite != null)
                {
                    trashIconImage.sprite  = sr.sprite;
                    trashIconImage.enabled = true;
                }
                else
                {
                    trashIconImage.enabled = false;
                }
            }
        }

        private void OnBinButtonClicked(BinType binType)
        {
            // always deposit the first item in the bag (index 0)
            OnDepositRequested?.Invoke(0, binType);

            // after the deposit handler runs (synchronous), bag may be shorter
            if (bagInventory == null || bagInventory.Count == 0)
                Close();
            else
                RefreshTrashDisplay();
        }

        private static string TrashLabel(TrashType t) => t switch
        {
            TrashType.Paper   => "Paper",
            TrashType.Can     => "Can",
            TrashType.Plastic => "Plastic",
            TrashType.Vinyl   => "Vinyl",
            TrashType.Food    => "Food",
            TrashType.Net     => "Net",
            TrashType.Oil     => "Oil",
            _                 => t.ToString()
        };
    }
}
