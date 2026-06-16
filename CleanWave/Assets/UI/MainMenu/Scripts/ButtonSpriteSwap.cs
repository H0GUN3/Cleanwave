using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 버튼에 마우스를 올리면 hover 스프라이트로, 벗어나면 일반 스프라이트로 교체한다.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Image))]
public class ButtonSpriteSwap : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image targetImage;
    [SerializeField] Sprite normalSprite;
    [SerializeField] Sprite hoverSprite;

    bool _hovering;

    void Reset()
    {
        targetImage = GetComponent<Image>();
        if (targetImage != null)
            normalSprite = targetImage.sprite;
    }

    void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();
        if (normalSprite == null && targetImage != null)
            normalSprite = targetImage.sprite;
    }

    void OnEnable()
    {
        _hovering = false;
        Apply(normalSprite);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hovering = true;
        Apply(hoverSprite);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hovering = false;
        Apply(normalSprite);
    }

    void Apply(Sprite sprite)
    {
        if (targetImage != null && sprite != null)
            targetImage.sprite = sprite;
    }
}
