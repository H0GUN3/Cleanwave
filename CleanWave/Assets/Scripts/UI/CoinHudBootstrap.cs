using CleanWave.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CleanWave.UI
{
    /// <summary>
    /// Creates a fallback coin HUD at runtime so the feature can be tested without scene edits.
    /// Remove this bootstrap when a hand-authored HUD Canvas is wired in the scene.
    /// </summary>
    public class CoinHudBootstrap : MonoBehaviour
    {
        private const string CanvasName = "CleanWave_RuntimeCoinHud";
        private const string CoinPanelPath = "MiryeongUI/HUD_coin";
        private const string DigitPathPrefix = "MiryeongUI/num_";
        private const float PanelHeight = 84f;
        private const float PanelLeftMargin = 24f;
        private const float PanelTopMargin = 20f;
        private const float DigitHeight = 34f;
        private const float DigitSpacing = 1f;
        private static bool sceneLoadedSubscribed;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateRuntimeHud()
        {
            SubscribeSceneLoaded();

            if (!GameplaySceneUtility.IsGameplayScene())
                return;

            EnsureWallet();

            if (FindFirstObjectByType<CoinHudView>() != null)
                return;

            GameObject canvasObject = new GameObject(CanvasName);
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 120;
            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObject.AddComponent<GraphicRaycaster>();

            Sprite panelSprite = LoadSprite(CoinPanelPath);
            Sprite[] digitSprites = LoadDigitSprites();

            GameObject panel = new GameObject("CoinPanel");
            panel.transform.SetParent(canvasObject.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0f, 1f);
            panelRect.anchorMax = new Vector2(0f, 1f);
            panelRect.pivot = new Vector2(0f, 1f);
            panelRect.anchoredPosition = new Vector2(PanelLeftMargin, -PanelTopMargin);
            panelRect.sizeDelta = GetPanelSize(panelSprite);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.sprite = panelSprite;
            panelImage.preserveAspect = true;
            panelImage.color = panelSprite == null ? new Color(1f, 1f, 1f, 0.85f) : Color.white;
            panelImage.raycastTarget = false;

            Transform digitRoot = CreateDigitRoot(panel.transform);

            CoinHudView view = panel.AddComponent<CoinHudView>();
            view.Initialize(digitRoot, digitSprites, DigitHeight, DigitSpacing);
            view.Bind(CoinWallet.Instance);
        }

        private static void SubscribeSceneLoaded()
        {
            if (sceneLoadedSubscribed)
                return;

            SceneManager.sceneLoaded += HandleSceneLoaded;
            sceneLoadedSubscribed = true;
        }

        private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            CreateRuntimeHud();
        }

        private static void EnsureWallet()
        {
            CoinWallet.EnsureInstance();
        }

        private static Transform CreateDigitRoot(Transform parent)
        {
            GameObject digitRootObject = new GameObject("CoinDigits");
            digitRootObject.transform.SetParent(parent, false);

            RectTransform rect = digitRootObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.56f, 0.5f);
            rect.anchorMax = new Vector2(0.86f, 0.5f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.sizeDelta = new Vector2(0f, DigitHeight);

            HorizontalLayoutGroup layout = digitRootObject.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleRight;
            layout.spacing = DigitSpacing;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            ContentSizeFitter fitter = digitRootObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            return digitRootObject.transform;
        }

        private static Vector2 GetPanelSize(Sprite sprite)
        {
            if (sprite == null || sprite.rect.height <= 0f)
                return new Vector2(190f, PanelHeight);

            return new Vector2(PanelHeight * (sprite.rect.width / sprite.rect.height), PanelHeight);
        }

        private static Sprite[] LoadDigitSprites()
        {
            Sprite[] sprites = new Sprite[10];
            for (int i = 0; i < sprites.Length; i++)
                sprites[i] = LoadSprite(DigitPathPrefix + i);

            return sprites;
        }

        private static Sprite LoadSprite(string resourcePath)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>(resourcePath);
            if (sprites != null && sprites.Length > 0)
                return sprites[0];

            Sprite sprite = Resources.Load<Sprite>(resourcePath);
            if (sprite != null)
                return sprite;

            Texture2D texture = Resources.Load<Texture2D>(resourcePath);
            if (texture == null)
                return null;

            Rect rect = new Rect(0f, 0f, texture.width, texture.height);
            return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100f);
        }
    }
}
