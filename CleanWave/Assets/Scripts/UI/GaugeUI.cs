using UnityEngine;
using UnityEngine.UI;

public class GaugeUI : MonoBehaviour
{
    [SerializeField] Image fillImage;   // gage.png, type=Filled Vertical Bottom

    // gage 내용물이 fill RectTransform 안에서 차지하는 세로 범위(0~1).
    // 텍스처 자체의 투명 여백 때문에 fillAmount를 이 범위로 매핑한다.
    [SerializeField] float fillMin = 0f;
    [SerializeField] float fillMax = 1f;

    CollectionTracker tracker;
    float progress;

    void Start()
    {
        SetProgress(0f);

        tracker = CollectionTracker.Instance ?? FindFirstObjectByType<CollectionTracker>();
        if (tracker != null)
            tracker.OnProgressChanged.AddListener(SetProgress);
    }

    void OnDestroy()
    {
        if (tracker != null)
            tracker.OnProgressChanged.RemoveListener(SetProgress);
    }

    void SetProgress(float p)
    {
        progress = Mathf.Clamp01(p);
        if (fillImage != null)
            fillImage.fillAmount = Mathf.Lerp(fillMin, fillMax, progress);
    }
}
