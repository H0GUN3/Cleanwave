using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace CleanWave
{
    public class ResultScreen : MonoBehaviour
    {
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI purificationRateText;
        [SerializeField] private Image[] starImages;
        [SerializeField] private Sprite starFilled;
        [SerializeField] private Sprite starEmpty;
        [SerializeField] private Button retryButton;

        private void Start()
        {
            if (resultPanel != null) resultPanel.SetActive(false);
            if (retryButton != null)
                retryButton.onClick.AddListener(RetryGame);
        }

        public void Show(int score, float purificationRate, bool timeUp = false)
        {
            if (resultPanel != null) resultPanel.SetActive(true);

            if (titleText)
            {
                titleText.text  = timeUp ? "Time's Up!" : "All Clear!";
                titleText.color = timeUp ? new Color(1f, 0.3f, 0.3f) : new Color(1f, 0.9f, 0.3f);
            }

            if (finalScoreText != null)
                finalScoreText.text = $"Final Score: {score}";

            if (purificationRateText != null)
                purificationRateText.text = $"Purification: {purificationRate:F0}%";

            int stars = CalculateStars(purificationRate);
            SetStars(stars);

            Time.timeScale = 0f;
        }

        private int CalculateStars(float purificationRate)
        {
            if (purificationRate >= 90f) return 3;
            if (purificationRate >= 60f) return 2;
            return 1;
        }

        private void SetStars(int count)
        {
            if (starImages == null) return;
            for (int i = 0; i < starImages.Length; i++)
            {
                if (starImages[i] == null) continue;
                starImages[i].sprite = (i < count) ? starFilled : starEmpty;
            }
        }

        private void RetryGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
