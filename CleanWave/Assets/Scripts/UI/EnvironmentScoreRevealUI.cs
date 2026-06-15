using CleanWave.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnvironmentScoreRevealUI : MonoBehaviour
{
    const string CanvasName = "CleanWave_EnvironmentScoreResult";
    const string BackgroundPath = "MiryeongUI/quiz_bg";
    const string OptionPath = "MiryeongUI/quiz_option";
    const string DigitPathPrefix = "MiryeongUI/num_";
    const string MainMenuSceneName = "MainMenu";
    const float PanelWidth = 760f;
    const float PanelHeight = 520f;
    const float DigitHeight = 88f;
    const float DigitSpacing = 4f;

    static readonly Color FallbackPanelColor = new Color(0.08f, 0.24f, 0.24f, 0.96f);
    static readonly Color TextColor = new Color(0.04f, 0.12f, 0.08f);
    static readonly Color FallbackButtonColor = new Color(0.9f, 0.98f, 0.9f, 0.95f);

    static EnvironmentScoreRevealUI instance;
    static bool sceneLoadedSubscribed;

    readonly Sprite[] digitSprites = new Sprite[10];

    GameObject panelRoot;
    Transform digitRoot;
    ZoneTrashProgress progress;
    CleanWaveRunState runState;
    bool isVisible;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void EnsureRuntimeUI()
    {
        SubscribeSceneLoaded();

        if (!GameplaySceneUtility.IsGameplayScene())
            return;

        if (FindFirstObjectByType<EnvironmentScoreRevealUI>() != null)
            return;

        GameObject uiObject = new GameObject(CanvasName);
        uiObject.AddComponent<EnvironmentScoreRevealUI>();
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
        EnsureRuntimeUI();
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        runState = CleanWaveRunState.EnsureInstance();
        BuildUI();
    }

    void OnEnable()
    {
        BindProgressTracker();
    }

    void Start()
    {
        BindProgressTracker();
        EvaluateVisibility();
    }

    void OnDisable()
    {
        if (progress != null)
            progress.ProgressChanged -= HandleProgressChanged;
    }

    void OnDestroy()
    {
        if (instance == this)
            instance = null;
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
        if (zoneType == CleanWaveZoneType.Coast && percent >= CleanWaveGameConstants.PurificationPercentToRevealEnvironmentScore)
            ShowPanel();
    }

    void EvaluateVisibility()
    {
        if (progress != null && progress.GetCollectedPercent(CleanWaveZoneType.Coast) >= CleanWaveGameConstants.PurificationPercentToRevealEnvironmentScore)
            ShowPanel();
    }

    void BuildUI()
    {
        if (panelRoot != null)
            return;

        EnsureEventSystem();
        LoadDigitSprites();

        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 240;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        gameObject.AddComponent<GraphicRaycaster>();

        panelRoot = CreatePanel(transform);
        CreateTitle(panelRoot.transform);
        digitRoot = CreateDigitRoot(panelRoot.transform);
        CreateButton(panelRoot.transform, "RestartButton", new Vector2(-178f, -172f), "다시하기", RestartActiveScene);
        CreateButton(panelRoot.transform, "QuitButton", new Vector2(178f, -172f), "나가기", LoadMainMenu);
        panelRoot.SetActive(false);
    }

    int GetScore()
    {
        if (runState == null)
            runState = CleanWaveRunState.EnsureInstance();

        return runState != null ? runState.Score : CleanWaveGameConstants.StartingGameScore;
    }

    void ShowPanel()
    {
        if (isVisible)
            return;

        isVisible = true;
        BuildUI();
        SetScoreDigits(GetScore());

        if (panelRoot != null)
            panelRoot.SetActive(true);
    }

    void SetScoreDigits(int score)
    {
        if (digitRoot == null)
            return;

        LoadDigitSprites();
        string scoreString = Mathf.Max(0, score).ToString();

        for (int i = digitRoot.childCount - 1; i >= 0; i--)
            Destroy(digitRoot.GetChild(i).gameObject);

        for (int i = 0; i < scoreString.Length; i++)
        {
            int digit = scoreString[i] - '0';
            GameObject digitObject = new GameObject($"ScoreDigit_{i}");
            digitObject.transform.SetParent(digitRoot, false);

            Image image = digitObject.AddComponent<Image>();
            image.sprite = digitSprites[digit];
            image.preserveAspect = true;
            image.raycastTarget = false;

            SetDigitSize(image.rectTransform, image.sprite);
        }
    }

    static GameObject CreatePanel(Transform parent)
    {
        GameObject panel = new GameObject("EnvironmentScorePanel");
        panel.transform.SetParent(parent, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(PanelWidth, PanelHeight);

        Image image = panel.AddComponent<Image>();
        image.sprite = LoadSprite(BackgroundPath);
        image.color = image.sprite == null ? FallbackPanelColor : Color.white;
        return panel;
    }

    static void CreateTitle(Transform parent)
    {
        Text title = CreateText(parent, "Title", new Vector2(0f, 152f), new Vector2(560f, 86f), 54);
        title.text = "SCORE!";
        title.fontStyle = FontStyle.Bold;
        title.color = TextColor;
    }

    static Transform CreateDigitRoot(Transform parent)
    {
        GameObject digitRootObject = new GameObject("ScoreDigits");
        digitRootObject.transform.SetParent(parent, false);

        RectTransform rect = digitRootObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, 16f);
        rect.sizeDelta = new Vector2(500f, DigitHeight);

        HorizontalLayoutGroup layout = digitRootObject.AddComponent<HorizontalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
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

    static void CreateButton(Transform parent, string name, Vector2 position, string labelText, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = buttonObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(318f, 82f);

        Image image = buttonObject.AddComponent<Image>();
        image.sprite = LoadSprite(OptionPath);
        image.color = image.sprite == null ? FallbackButtonColor : Color.white;

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);

        Text label = CreateText(buttonObject.transform, "Label", Vector2.zero, new Vector2(282f, 68f), 24);
        label.text = labelText;
        label.fontStyle = FontStyle.Bold;
        label.color = TextColor;
    }

    static Text CreateText(Transform parent, string name, Vector2 position, Vector2 size, int fontSize)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);

        RectTransform rect = textObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Text text = textObject.AddComponent<Text>();
        text.font = CreateKoreanFont(fontSize);
        text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.raycastTarget = false;
        return text;
    }

    static Font CreateKoreanFont(int size)
    {
        Font projectFont = Resources.Load<Font>("Fonts/TerrarumSansBitmap");
        if (projectFont != null)
            return projectFont;

        string[] preferredFonts =
        {
            "Malgun Gothic",
            "맑은 고딕",
            "Noto Sans CJK KR",
            "Noto Sans KR",
            "Apple SD Gothic Neo",
            "Arial Unicode MS",
        };

        Font font = Font.CreateDynamicFontFromOSFont(preferredFonts, size);
        if (font != null)
            return font;

        return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
    }

    void LoadDigitSprites()
    {
        for (int i = 0; i < digitSprites.Length; i++)
        {
            if (digitSprites[i] != null)
                continue;

            digitSprites[i] = LoadSprite(DigitPathPrefix + i);
        }
    }

    static Sprite LoadSprite(string resourcePath)
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

    static void SetDigitSize(RectTransform rect, Sprite sprite)
    {
        float width = DigitHeight;
        if (sprite != null && sprite.rect.height > 0f)
            width = DigitHeight * (sprite.rect.width / sprite.rect.height);

        rect.sizeDelta = new Vector2(width, DigitHeight);

        LayoutElement layout = rect.GetComponent<LayoutElement>();
        if (layout == null)
            layout = rect.gameObject.AddComponent<LayoutElement>();

        layout.preferredWidth = width;
        layout.preferredHeight = DigitHeight;
        layout.minWidth = width;
        layout.minHeight = DigitHeight;
        layout.flexibleWidth = 0f;
        layout.flexibleHeight = 0f;
    }

    static void EnsureEventSystem()
    {
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            GameObject eventSystemObject = new GameObject("CleanWave_RuntimeEventSystem");
            eventSystem = eventSystemObject.AddComponent<EventSystem>();
        }

        if (eventSystem.GetComponent<BaseInputModule>() == null)
            eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
    }

    static void RestartActiveScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.buildIndex >= 0)
            SceneManager.LoadScene(activeScene.buildIndex);
        else
            SceneManager.LoadScene(activeScene.name);
    }

    static void LoadMainMenu()
    {
        SceneManager.LoadScene(MainMenuSceneName);
    }
}
