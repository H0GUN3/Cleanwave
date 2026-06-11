using UnityEngine;
using TMPro;
using System.Collections;

namespace CleanWave
{
    public class FeedbackUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI feedbackText;
        [SerializeField] private float displayDuration = 2f;

        private Coroutine hideCoroutine;

        public void ShowMessage(string message)
        {
            if (feedbackText == null) return;
            feedbackText.text = message;
            feedbackText.gameObject.SetActive(true);

            if (hideCoroutine != null) StopCoroutine(hideCoroutine);
            hideCoroutine = StartCoroutine(HideAfterDelay());
        }

        private IEnumerator HideAfterDelay()
        {
            yield return new WaitForSecondsRealtime(displayDuration);
            if (feedbackText != null)
                feedbackText.gameObject.SetActive(false);
        }
    }
}
