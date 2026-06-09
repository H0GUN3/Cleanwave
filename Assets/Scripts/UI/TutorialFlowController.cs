using TMPro;
using UnityEngine;

namespace CleanWave.UI
{
    public class TutorialFlowController : MonoBehaviour
    {
        [SerializeField] private TMP_Text tutorialText;
        [SerializeField] private float[] timings = { 0f, 15f, 30f, 45f };
        [SerializeField] private string[] messages =
        {
            "WASD로 이동하세요.",
            "쓰레기 근처에서 E를 눌러 수거하세요.",
            "수거함 앞에서 E를 눌러 분리수거하세요.",
            "Shift로 부스트 이동이 가능합니다."
        };

        private float _startTime;
        private int _index;

        private void Start()
        {
            _startTime = Time.time;
            _index = 0;
            RefreshMessage();
        }

        private void Update()
        {
            if (_index + 1 >= timings.Length)
            {
                return;
            }

            var elapsed = Time.time - _startTime;
            if (elapsed >= timings[_index + 1])
            {
                _index++;
                RefreshMessage();
            }
        }

        private void RefreshMessage()
        {
            if (tutorialText != null && _index >= 0 && _index < messages.Length)
            {
                tutorialText.text = messages[_index];
            }
        }
    }
}
