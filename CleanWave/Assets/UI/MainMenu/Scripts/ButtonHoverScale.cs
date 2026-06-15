using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI 버튼 hover / press 시 부드럽게 스케일한다.
/// </summary>
[DisallowMultipleComponent]
public class ButtonHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] float hoverScale = 1.05f;
    [SerializeField] float pressedScale = 0.97f;
    [SerializeField] float duration = 0.1f;

    Vector3 _baseScale = Vector3.one;
    bool _hovering;
    Coroutine _scaleRoutine;

    void Awake()
    {
        _baseScale = transform.localScale;
    }

    void OnEnable()
    {
        transform.localScale = _baseScale;
        _hovering = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hovering = true;
        AnimateTo(hoverScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hovering = false;
        AnimateTo(1f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        AnimateTo(pressedScale);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        AnimateTo(_hovering ? hoverScale : 1f);
    }

    void AnimateTo(float factor)
    {
        if (_scaleRoutine != null)
            StopCoroutine(_scaleRoutine);

        _scaleRoutine = StartCoroutine(ScaleRoutine(_baseScale * factor));
    }

    IEnumerator ScaleRoutine(Vector3 target)
    {
        Vector3 start = transform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.localScale = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.localScale = target;
        _scaleRoutine = null;
    }
}
