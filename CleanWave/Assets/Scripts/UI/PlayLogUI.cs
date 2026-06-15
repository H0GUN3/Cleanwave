using System.Collections.Generic;
using CleanWave.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayLogUI : MonoBehaviour
{
    struct LogEntry
    {
        public readonly string Text;
        public readonly Color Color;

        public LogEntry(string text, Color color)
        {
            Text = text;
            Color = color;
        }
    }

    const string CanvasName = "CleanWave_PlayLog";
    const int MaxLines = 5;
    const float VisibleDuration = 4f;
    const float FadeDuration = 0.8f;
    const float PanelWidth = 360f;
    const float PanelHeight = 156f;
    const float Margin = 24f;
    const float LineHeight = 24f;

    static readonly Color PanelColor = new Color(0f, 0f, 0f, 0.55f);
    static readonly Color DefaultLogColor = Color.white;
    static readonly Color SuccessLogColor = new Color(0.62f, 1f, 0.82f, 1f);

    static PlayLogUI instance;
    static bool sceneLoadedSubscribed;

    readonly Queue<LogEntry> lines = new Queue<LogEntry>(MaxLines);
    readonly List<Text> lineTexts = new List<Text>(MaxLines);

    CanvasGroup canvasGroup;
    float visibleUntil;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void EnsureRuntimeUI()
    {
        SubscribeSceneLoaded();

        if (!GameplaySceneUtility.IsGameplayScene())
            return;

        EnsureInstance();
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

    public static void AddLog(string text)
    {
        AddLog(text, DefaultLogColor);
    }

    public static void AddSuccessLog(string text)
    {
        AddLog(text, SuccessLogColor);
    }

    public static void AddLog(string text, Color color)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        if (!GameplaySceneUtility.IsGameplayScene())
            return;

        EnsureInstance().Append(text, color);
    }

    static PlayLogUI EnsureInstance()
    {
        if (instance != null)
            return instance;

        PlayLogUI found = FindFirstObjectByType<PlayLogUI>();
        if (found != null)
        {
            instance = found;
            return instance;
        }

        GameObject uiObject = new GameObject(CanvasName);
        return uiObject.AddComponent<PlayLogUI>();
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        BuildUI();
        SetVisible(0f);
    }

    void Update()
    {
        if (canvasGroup == null)
            return;

        float fadeStart = visibleUntil;
        float fadeEnd = fadeStart + FadeDuration;

        if (Time.time <= fadeStart)
        {
            SetVisible(1f);
            return;
        }

        if (Time.time >= fadeEnd)
        {
            SetVisible(0f);
            return;
        }

        float alpha = 1f - ((Time.time - fadeStart) / FadeDuration);
        SetVisible(alpha);
    }

    void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    void Append(string text, Color color)
    {
        while (lines.Count >= MaxLines)
            lines.Dequeue();

        lines.Enqueue(new LogEntry(text, color));
        visibleUntil = Time.time + VisibleDuration;
        SetVisible(1f);
        RefreshLines();
    }

    void BuildUI()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 90;

        gameObject.AddComponent<GraphicRaycaster>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        RectTransform canvasRect = GetComponent<RectTransform>();
        canvasRect.anchorMin = Vector2.zero;
        canvasRect.anchorMax = Vector2.one;
        canvasRect.offsetMin = Vector2.zero;
        canvasRect.offsetMax = Vector2.zero;

        GameObject panel = new GameObject("PlayLogPanel");
        panel.transform.SetParent(transform, false);

        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(1f, 1f);
        panelRect.anchorMax = new Vector2(1f, 1f);
        panelRect.pivot = new Vector2(1f, 1f);
        panelRect.anchoredPosition = new Vector2(-Margin, -Margin);
        panelRect.sizeDelta = new Vector2(PanelWidth, PanelHeight);

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = PanelColor;

        for (int i = 0; i < MaxLines; i++)
        {
            GameObject lineObject = new GameObject($"Line_{i + 1}");
            lineObject.transform.SetParent(panel.transform, false);

            RectTransform lineRect = lineObject.AddComponent<RectTransform>();
            lineRect.anchorMin = new Vector2(0f, 1f);
            lineRect.anchorMax = new Vector2(1f, 1f);
            lineRect.pivot = new Vector2(0.5f, 1f);
            lineRect.anchoredPosition = new Vector2(0f, -14f - i * LineHeight);
            lineRect.sizeDelta = new Vector2(-24f, LineHeight);

            Text text = lineObject.AddComponent<Text>();
            text.alignment = TextAnchor.MiddleLeft;
            text.color = Color.white;
            text.fontSize = 18;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.font = CreateKoreanFont();
            text.text = string.Empty;
            lineTexts.Add(text);
        }
    }

    void RefreshLines()
    {
        LogEntry[] snapshot = lines.ToArray();
        int emptyCount = MaxLines - snapshot.Length;

        for (int i = 0; i < MaxLines; i++)
        {
            if (i < emptyCount)
            {
                lineTexts[i].text = string.Empty;
                lineTexts[i].color = DefaultLogColor;
            }
            else
            {
                LogEntry entry = snapshot[i - emptyCount];
                lineTexts[i].text = entry.Text;
                lineTexts[i].color = entry.Color;
            }
        }
    }

    void SetVisible(float alpha)
    {
        if (canvasGroup != null)
            canvasGroup.alpha = Mathf.Clamp01(alpha);
    }

    static Font CreateKoreanFont()
    {
        Font resourceFont = Resources.Load<Font>("Fonts/TerrarumSansBitmap");
        if (resourceFont != null)
            return resourceFont;

        Font arial = Resources.GetBuiltinResource<Font>("Arial.ttf");
        return arial != null ? arial : Font.CreateDynamicFontFromOSFont("Malgun Gothic", 18);
    }
}
