using CleanWave.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// CleanWave 메인 메뉴. 게임 시작 / 게임 종료 버튼만 처리한다.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    const string StartClipPath = "Audio/Game/start_explosion";
    const string StartHoverPath = "UI/MainMenu/menu_hover_start";
    const string ExitHoverPath = "UI/MainMenu/menu_hover_exit";

    [SerializeField] string gameplaySceneName = "MainScene";
    [SerializeField] Button gameStartButton;
    [SerializeField] Button exitButton;

    static AudioClip startClip;

    void Awake()
    {
        if (gameStartButton == null)
            gameStartButton = transform.Find("StartButton")?.GetComponent<Button>();
        if (exitButton == null)
            exitButton = transform.Find("ExitButton")?.GetComponent<Button>();

        if (gameStartButton != null)
        {
            EnsureHoverFeedback(gameStartButton, StartHoverPath);
            gameStartButton.onClick.RemoveListener(StartGame);
            gameStartButton.onClick.AddListener(StartGame);
        }

        if (exitButton != null)
        {
            EnsureHoverFeedback(exitButton, ExitHoverPath);
            exitButton.onClick.RemoveListener(ExitGame);
            exitButton.onClick.AddListener(ExitGame);
        }
    }

    void EnsureHoverFeedback(Button button, string hoverOverlayPath)
    {
        button.transition = Selectable.Transition.None;
        DisableScaleFeedback(button);

        UIHoverFeedback feedback = button.GetComponent<UIHoverFeedback>();
        if (feedback == null)
            feedback = button.gameObject.AddComponent<UIHoverFeedback>();

        feedback.ConfigureHoverOverlay(hoverOverlayPath);
    }

    static void DisableScaleFeedback(Button button)
    {
        ButtonHoverScale scaleFeedback = button.GetComponent<ButtonHoverScale>();
        if (scaleFeedback != null)
            scaleFeedback.enabled = false;
    }

    public void StartGame()
    {
        if (string.IsNullOrWhiteSpace(gameplaySceneName))
        {
            Debug.LogError("[MainMenuController] gameplaySceneName이 비어 있습니다.");
            return;
        }

        PlayStartSound();
        SceneManager.LoadScene(gameplaySceneName);
    }

    static void PlayStartSound()
    {
        if (startClip == null)
            startClip = Resources.Load<AudioClip>(StartClipPath);

        if (startClip == null)
            return;

        GameObject audioObject = new GameObject("CleanWave_StartClickAudio");
        DontDestroyOnLoad(audioObject);

        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        audioSource.PlayOneShot(startClip);

        Destroy(audioObject, startClip.length + 0.1f);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
