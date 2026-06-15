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
    [SerializeField] string gameplaySceneName = "MainScene";
    [SerializeField] Button gameStartButton;
    [SerializeField] Button exitButton;

    void Awake()
    {
        if (gameStartButton == null)
            gameStartButton = transform.Find("MenuRoot/GameStartButton")?.GetComponent<Button>()
                ?? transform.Find("GameStartButton")?.GetComponent<Button>();
        if (exitButton == null)
            exitButton = transform.Find("MenuRoot/ExitButton")?.GetComponent<Button>()
                ?? transform.Find("ExitButton")?.GetComponent<Button>();

        if (gameStartButton != null)
        {
            gameStartButton.onClick.RemoveListener(StartGame);
            gameStartButton.onClick.AddListener(StartGame);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(ExitGame);
            exitButton.onClick.AddListener(ExitGame);
        }
    }

    public void StartGame()
    {
        if (string.IsNullOrWhiteSpace(gameplaySceneName))
        {
            Debug.LogError("[MainMenuController] gameplaySceneName이 비어 있습니다.");
            return;
        }

        SceneManager.LoadScene(gameplaySceneName);
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
