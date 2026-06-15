using UnityEngine;
using UnityEngine.UI;

namespace CleanWave.UI
{
    public class QuizFeedbackPopup : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Text questionText;
        [SerializeField] private Button[] optionButtons;
        [SerializeField] private Text[] optionTexts;
        [SerializeField] private Text feedbackText;
        [SerializeField] private Button closeButton;

        private int correctIndex;
        private string wrongExplanation;
        private bool answered;

        private void Awake()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(Hide);
            Hide();
        }

        public void Configure(
            GameObject root,
            Text question,
            Button[] buttons,
            Text[] buttonTexts,
            Text feedback,
            Button close)
        {
            panelRoot = root;
            questionText = question;
            optionButtons = buttons;
            optionTexts = buttonTexts;
            feedbackText = feedback;
            closeButton = close;

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Hide);
                closeButton.onClick.AddListener(Hide);
            }

            ClearFeedback();
        }

        public void Show(string question, string[] options, int correct)
        {
            Show(question, options, correct, string.Empty);
        }

        public void Show(string question, string[] options, int correct, string explanation)
        {
            correctIndex = Mathf.Clamp(correct, 0, Mathf.Max(0, options.Length - 1));
            wrongExplanation = explanation;
            answered = false;

            if (panelRoot != null)
                panelRoot.SetActive(true);
            if (questionText != null)
                questionText.text = question;
            if (feedbackText != null)
                ClearFeedback();

            if (optionButtons == null)
                return;

            for (int i = 0; i < optionButtons.Length; i++)
            {
                int captured = i;
                bool hasOption = i < options.Length;

                if (optionTexts != null && i < optionTexts.Length && optionTexts[i] != null)
                    optionTexts[i].text = hasOption ? options[i] : string.Empty;

                if (optionButtons[i] == null)
                    continue;

                optionButtons[i].gameObject.SetActive(hasOption);
                optionButtons[i].interactable = hasOption;
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => Select(captured));
            }
        }

        public void Hide()
        {
            ClearFeedback();

            if (panelRoot != null)
                panelRoot.SetActive(false);
        }

        private void Select(int index)
        {
            if (answered)
                return;

            bool correct = index == correctIndex;
            if (correct)
                answered = true;

            if (feedbackText != null)
            {
                feedbackText.gameObject.SetActive(true);
                feedbackText.text = correct ? "정답!" : GetWrongFeedbackText();
                feedbackText.color = correct ? new Color(0.22f, 0.85f, 0.42f) : new Color(1f, 0.38f, 0.32f);
            }

            if (!correct)
                return;

            if (optionButtons == null)
                return;

            foreach (Button button in optionButtons)
            {
                if (button != null)
                    button.interactable = false;
            }
        }

        private string GetWrongFeedbackText()
        {
            if (string.IsNullOrWhiteSpace(wrongExplanation))
                return "오답! 다시 선택해보세요.";

            return wrongExplanation;
        }

        private void ClearFeedback()
        {
            if (feedbackText == null)
                return;

            feedbackText.text = string.Empty;
            feedbackText.gameObject.SetActive(false);
        }
    }
}
