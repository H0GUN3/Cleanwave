using CleanWave.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CleanWave.UI
{
    /// <summary>
    /// Runtime quiz popup bootstrap. Each zone opens its assigned sample quiz once
    /// when that zone reaches 50% purification.
    /// </summary>
    public class QuizHudTestBootstrap : MonoBehaviour
    {
        private const string CanvasName = "CleanWave_RuntimeQuizHud";
        private const string BackgroundPath = "MiryeongUI/quiz_bg";
        private const string OptionPath = "MiryeongUI/quiz_option";
        private const string OptionHoverPath = "MiryeongUI/quiz_option_hover";
        private const int TriggerPercent = 50;

        private static readonly QuizSample[] Samples =
        {
            new QuizSample(
                "도심에서 주운 페트병은 어떻게 버려야 할까요?",
                new[] { "비우고 라벨을 떼어 압착한다", "음식물과 함께 버린다", "내용물이 남아도 그대로 버린다", "길가에 모아 둔다" },
                0,
                "페트병은 비우고 라벨을 제거해야 깨끗한 플라스틱으로 재활용돼요."),
            new QuizSample(
                "하천에 버려진 비닐봉투가 위험한 이유는?",
                new[] { "물고기가 먹이로 착각할 수 있다", "물이 더 맑아진다", "식물이 더 빨리 자란다", "하천 냄새가 사라진다" },
                0,
                "비닐은 잘 썩지 않고 생물이 삼키거나 얽히게 만들어 하천 생태계를 해쳐요."),
            new QuizSample(
                "해안의 폐어망을 수거해야 하는 가장 큰 이유는?",
                new[] { "바다 생물이 걸려 다칠 수 있다", "모래가 더 단단해진다", "파도가 조용해진다", "바닷물이 바로 깨끗해진다" },
                0,
                "버려진 그물은 계속 생물을 붙잡는 유령어업을 일으킬 수 있어요."),
        };

        private QuizFeedbackPopup popup;
        private ZoneTrashProgress progress;
        private bool cityQuizShown;
        private bool riverQuizShown;
        private bool coastQuizShown;
        private static bool sceneLoadedSubscribed;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateRuntimeQuizHud()
        {
            SubscribeSceneLoaded();

            if (!GameplaySceneUtility.IsGameplayScene())
                return;

            if (FindFirstObjectByType<QuizHudTestBootstrap>() != null)
                return;

            GameObject host = new GameObject(CanvasName);
            host.AddComponent<QuizHudTestBootstrap>().Build(host);
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
            CreateRuntimeQuizHud();
        }

        private void OnEnable()
        {
            BindProgress();
        }

        private void Start()
        {
            BindProgress();
            EvaluateExistingProgress();
        }

        private void OnDisable()
        {
            if (progress != null)
            {
                progress.ProgressChanged -= HandleProgressChanged;
                progress = null;
            }
        }

        private void Build(GameObject host)
        {
            EnsureEventSystem();

            Canvas canvas = host.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 200;
            CanvasScaler scaler = host.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            host.AddComponent<GraphicRaycaster>();

            GameObject panel = CreatePanel(host.transform);
            Text question = CreateQuestion(panel.transform);
            Text feedback = CreateFeedback(panel.transform);
            Button close = CreateCloseButton(panel.transform);

            Button[] buttons = new Button[4];
            Text[] texts = new Text[4];
            for (int i = 0; i < buttons.Length; i++)
                buttons[i] = CreateOption(panel.transform, i, out texts[i]);

            popup = panel.AddComponent<QuizFeedbackPopup>();
            popup.Configure(panel, question, buttons, texts, feedback, close);
            popup.Hide();
        }

        private static void EnsureEventSystem()
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

        private void BindProgress()
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

        private void EvaluateExistingProgress()
        {
            if (progress == null)
                return;

            TryShowZoneQuiz(CleanWaveZoneType.City, progress.GetCollectedPercent(CleanWaveZoneType.City));
            TryShowZoneQuiz(CleanWaveZoneType.River, progress.GetCollectedPercent(CleanWaveZoneType.River));
            TryShowZoneQuiz(CleanWaveZoneType.Coast, progress.GetCollectedPercent(CleanWaveZoneType.Coast));
        }

        private void HandleProgressChanged(CleanWaveZoneType zone, int collected, int total, int percent)
        {
            TryShowZoneQuiz(zone, percent);
        }

        private void TryShowZoneQuiz(CleanWaveZoneType zone, int percent)
        {
            if (popup == null || percent < TriggerPercent || HasShownQuiz(zone))
                return;

            int sampleIndex = GetSampleIndex(zone);
            if (sampleIndex < 0 || sampleIndex >= Samples.Length)
                return;

            SetShownQuiz(zone);
            QuizSample sample = Samples[sampleIndex];
            popup.Show(sample.Question, sample.Options, sample.CorrectIndex, sample.WrongExplanation);
        }

        private bool HasShownQuiz(CleanWaveZoneType zone)
        {
            switch (zone)
            {
                case CleanWaveZoneType.City:
                    return cityQuizShown;
                case CleanWaveZoneType.River:
                    return riverQuizShown;
                case CleanWaveZoneType.Coast:
                    return coastQuizShown;
                default:
                    return true;
            }
        }

        private void SetShownQuiz(CleanWaveZoneType zone)
        {
            switch (zone)
            {
                case CleanWaveZoneType.City:
                    cityQuizShown = true;
                    break;
                case CleanWaveZoneType.River:
                    riverQuizShown = true;
                    break;
                case CleanWaveZoneType.Coast:
                    coastQuizShown = true;
                    break;
            }
        }

        private static int GetSampleIndex(CleanWaveZoneType zone)
        {
            switch (zone)
            {
                case CleanWaveZoneType.City:
                    return 0;
                case CleanWaveZoneType.River:
                    return 1;
                case CleanWaveZoneType.Coast:
                    return 2;
                default:
                    return -1;
            }
        }

        private static GameObject CreatePanel(Transform parent)
        {
            GameObject panel = new GameObject("QuizFeedbackPanel");
            panel.transform.SetParent(parent, false);
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(760f, 520f);

            Image image = panel.AddComponent<Image>();
            image.sprite = LoadSprite(BackgroundPath);
            image.color = image.sprite == null ? new Color(0.08f, 0.24f, 0.24f, 0.96f) : Color.white;
            return panel;
        }

        private static Text CreateQuestion(Transform parent)
        {
            Text text = CreateText(parent, "QuestionText", new Vector2(0f, 142f), new Vector2(650f, 92f), 30);
            text.fontStyle = FontStyle.Bold;
            text.color = new Color(0.04f, 0.12f, 0.08f);
            return text;
        }

        private static Button CreateOption(Transform parent, int index, out Text label)
        {
            GameObject option = new GameObject($"Option_{index + 1}");
            option.transform.SetParent(parent, false);
            RectTransform rect = option.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = GetOptionPosition(index);
            rect.sizeDelta = new Vector2(318f, 82f);

            Image image = option.AddComponent<Image>();
            image.sprite = LoadSprite(OptionPath);
            image.color = image.sprite == null ? new Color(0.9f, 0.98f, 0.9f, 0.95f) : Color.white;

            Button button = option.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.None;
            option.AddComponent<UIHoverFeedback>();
            Sprite hover = LoadSprite(OptionHoverPath);
            if (hover != null)
            {
                SpriteState state = button.spriteState;
                state.highlightedSprite = hover;
                state.pressedSprite = hover;
                button.spriteState = state;
            }

            label = CreateText(option.transform, "Label", Vector2.zero, new Vector2(282f, 68f), 20);
            label.color = new Color(0.04f, 0.1f, 0.1f);
            label.fontStyle = FontStyle.Bold;
            return button;
        }

        private static Vector2 GetOptionPosition(int index)
        {
            float x = index % 2 == 0 ? -175f : 175f;
            float y = index < 2 ? 30f : -72f;
            return new Vector2(x, y);
        }

        private static Text CreateFeedback(Transform parent)
        {
            Text text = CreateText(parent, "FeedbackText", new Vector2(0f, -176f), new Vector2(630f, 76f), 22);
            text.fontStyle = FontStyle.Bold;
            text.gameObject.SetActive(false);
            return text;
        }

        private static Button CreateCloseButton(Transform parent)
        {
            GameObject close = new GameObject("CloseButton");
            close.transform.SetParent(parent, false);
            RectTransform rect = close.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-28f, -24f);
            rect.sizeDelta = new Vector2(48f, 48f);

            Image image = close.AddComponent<Image>();
            image.color = new Color(0.72f, 0.08f, 0.08f, 0.96f);
            Button button = close.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.None;
            close.AddComponent<UIHoverFeedback>();

            Text label = CreateText(close.transform, "Label", Vector2.zero, new Vector2(48f, 48f), 24);
            label.text = "X";
            label.fontStyle = FontStyle.Bold;
            label.color = Color.white;
            return button;
        }

        private static Text CreateText(Transform parent, string name, Vector2 position, Vector2 size, int fontSize)
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
            text.color = Color.white;
            text.raycastTarget = false;
            return text;
        }

        private static Font CreateKoreanFont(int size)
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

        private readonly struct QuizSample
        {
            public readonly string Question;
            public readonly string[] Options;
            public readonly int CorrectIndex;
            public readonly string WrongExplanation;

            public QuizSample(string question, string[] options, int correctIndex, string wrongExplanation)
            {
                Question = question;
                Options = options;
                CorrectIndex = correctIndex;
                WrongExplanation = wrongExplanation;
            }
        }
    }
}
