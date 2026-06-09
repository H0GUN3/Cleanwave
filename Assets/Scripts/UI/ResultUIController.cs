using CleanWave.Data;
using CleanWave.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

namespace CleanWave.UI
{
    public class ResultUIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text stageNameText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text accuracyText;
        [SerializeField] private TMP_Text comboText;
        [SerializeField] private TMP_Text starText;
        [SerializeField] private StageConfig stageConfig;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private SaveManager saveManager;

        public void BuildResult()
        {
            if (scoreManager == null)
            {
                return;
            }

            var totalSort = scoreManager.CorrectSortCount + scoreManager.WrongSortCount;
            var accuracy = totalSort <= 0 ? 100f : (float)scoreManager.CorrectSortCount / totalSort * 100f;
            var stars = GetStars(accuracy, scoreManager.CurrentScore);

            if (stageNameText != null && stageConfig != null)
            {
                stageNameText.text = stageConfig.StageDisplayName;
            }

            if (scoreText != null)
            {
                scoreText.text = $"점수 {scoreManager.CurrentScore}";
            }

            if (accuracyText != null)
            {
                accuracyText.text = $"정확도 {accuracy:F0}%";
            }

            if (comboText != null)
            {
                comboText.text = $"최고 콤보 x{scoreManager.BestComboMultiplier}";
            }

            if (starText != null)
            {
                starText.text = new string('★', stars) + new string('☆', 3 - stars);
            }

            saveManager?.SaveStageRewards(scoreManager.CurrentScore, stars * 20, ResolveClearedStageIndex());
        }

        private int GetStars(float accuracy, int score)
        {
            if (accuracy >= 95f && score >= 600) return 3;
            if (accuracy >= 85f && score >= 350) return 2;
            return 1;
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        private int ResolveClearedStageIndex()
        {
            if (stageConfig == null || string.IsNullOrWhiteSpace(stageConfig.StageId))
            {
                return 1;
            }

            var match = Regex.Match(stageConfig.StageId, @"\d+");
            if (match.Success && int.TryParse(match.Value, out var parsed))
            {
                return Mathf.Max(1, parsed);
            }

            return 1;
        }
    }
}
