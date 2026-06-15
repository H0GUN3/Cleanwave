using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 퀴즈 패널 하나를 담당. 정답 시 "정답!" 표시 후 닫기,
/// 오답 시 "오답! 다시 시도하세요." 표시 후 재시도 허용.
/// </summary>
public class QuizPanel : MonoBehaviour
{
    [SerializeField] Text questionText;
    [SerializeField] Button[] optionButtons;   // 4개
    [SerializeField] Text[] optionTexts;       // 각 버튼 안의 Text
    [SerializeField] Text feedbackText;
    [SerializeField] Button closeButton;

    string question;
    string[] options;
    int correctIndex;
    bool answered;

    public void Setup(string q, string[] opts, int correct)
    {
        question = q;
        options = opts;
        correctIndex = correct;
    }

    void OnEnable()
    {
        answered = false;
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(false);
            feedbackText.text = "";
        }

        if (questionText != null) questionText.text = question;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (optionButtons[i] == null) continue;
            if (i < options.Length && optionTexts != null && i < optionTexts.Length && optionTexts[i] != null)
                optionTexts[i].text = options[i];

            optionButtons[i].interactable = true;
            int captured = i;
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => OnAnswer(captured));
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() => gameObject.SetActive(false));
        }
    }

    void OnAnswer(int idx)
    {
        if (answered) return;

        if (idx == correctIndex)
        {
            answered = true;
            foreach (var btn in optionButtons)
                if (btn != null) btn.interactable = false;
            ShowFeedback("정답!", new Color(0.1f, 0.7f, 0.1f));
        }
        else
        {
            ShowFeedback("오답! 다시 시도하세요.", new Color(0.85f, 0.1f, 0.1f));
        }
    }

    void ShowFeedback(string msg, Color color)
    {
        if (feedbackText == null) return;
        feedbackText.gameObject.SetActive(true);
        feedbackText.text = msg;
        feedbackText.color = color;
    }
}
