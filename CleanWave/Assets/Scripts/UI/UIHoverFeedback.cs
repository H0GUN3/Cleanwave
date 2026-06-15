using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CleanWave.UI
{
    [DisallowMultipleComponent]
    public class UIHoverFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        private const string HoverClipPath = "Audio/Game/ui_hover";

        [SerializeField] private float hoverAlphaDelta = 0.16f;
        [SerializeField] private float pressedAlphaDelta = 0.32f;
        [SerializeField] private float hoverVolume = 0.9f;
        [SerializeField] private bool useCustomStateColors;
        [SerializeField] private Color hoverColor = Color.white;
        [SerializeField] private Color pressedColor = Color.white;
        [SerializeField] private string hoverOverlayTexturePath;
        [SerializeField] private float hoverOverlayAlpha = 1f;
        [SerializeField] private Vector2 hoverOverlayPadding = new Vector2(12f, 12f);

        private Graphic targetGraphic;
        private Image hoverOverlayImage;
        private Vector3 originalLocalScale = Vector3.one;
        private Color baseColor = Color.white;
        private bool hovering;
        private static AudioClip hoverClip;
        private static AudioSource uiAudioSource;

        private void Awake()
        {
            originalLocalScale = transform.localScale;
            EnsureTargetGraphic();
        }

        private void OnEnable()
        {
            RestoreOriginalScale();
            hovering = false;
            ApplyBaseColor();
        }

        public void ConfigureStateColors(Color hoverStateColor, Color pressedStateColor)
        {
            useCustomStateColors = true;
            hoverColor = hoverStateColor;
            pressedColor = pressedStateColor;
            hoverOverlayTexturePath = string.Empty;
            EnsureTargetGraphic();
            HideOverlay();
        }

        public void ConfigureHoverOverlay(string textureResourcePath, float overlayAlpha = 1f)
        {
            ConfigureHoverOverlay(textureResourcePath, overlayAlpha, hoverOverlayPadding);
        }

        public void ConfigureHoverOverlay(string textureResourcePath, float overlayAlpha, Vector2 overlayPadding)
        {
            hoverOverlayTexturePath = textureResourcePath;
            hoverOverlayAlpha = Mathf.Clamp01(overlayAlpha);
            hoverOverlayPadding = overlayPadding;
            useCustomStateColors = false;
            EnsureHoverOverlay();
            ApplyBaseColor();
            HideOverlay();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hovering = true;
            PlayHoverSound();
            RestoreOriginalScale();
            ApplyHoverState();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hovering = false;
            RestoreOriginalScale();
            ApplyBaseColor();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            RestoreOriginalScale();
            ApplyPressedState();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            RestoreOriginalScale();
            if (hovering)
                ApplyHoverState();
            else
                ApplyBaseColor();
        }

        private void EnsureTargetGraphic()
        {
            if (targetGraphic != null)
                return;

            targetGraphic = GetTargetGraphic();

            if (targetGraphic != null)
                baseColor = targetGraphic.color;
        }

        private Graphic GetTargetGraphic()
        {
            Selectable selectable = GetComponent<Selectable>();
            if (selectable != null && selectable.targetGraphic != null)
                return selectable.targetGraphic;

            return GetComponent<Graphic>() ?? GetComponentInChildren<Graphic>();
        }

        private void RestoreOriginalScale()
        {
            transform.localScale = originalLocalScale;
        }

        private float GetAdjustedAlpha(float delta)
        {
            float direction = baseColor.a >= 0.7f ? -1f : 1f;
            return Mathf.Clamp01(baseColor.a + direction * Mathf.Abs(delta));
        }

        private void ApplyBaseColor()
        {
            ApplyColor(baseColor);
            HideOverlay();
        }

        private void ApplyHoverState()
        {
            if (HasHoverOverlay())
                ShowOverlay(hoverOverlayAlpha);
            else if (useCustomStateColors)
                ApplyColor(hoverColor);
            else
                ApplyAlpha(GetAdjustedAlpha(hoverAlphaDelta));
        }

        private void ApplyPressedState()
        {
            if (HasHoverOverlay())
                ShowOverlay(Mathf.Clamp01(hoverOverlayAlpha * 0.9f));
            else if (useCustomStateColors)
                ApplyColor(pressedColor);
            else
                ApplyAlpha(GetAdjustedAlpha(pressedAlphaDelta));
        }

        private void ApplyAlpha(float alpha)
        {
            Color color = baseColor;
            color.a = alpha;
            ApplyColor(color);
        }

        private void ApplyColor(Color color)
        {
            EnsureTargetGraphic();

            if (targetGraphic == null)
                return;

            targetGraphic.color = color;
        }

        private bool HasHoverOverlay()
        {
            return !string.IsNullOrWhiteSpace(hoverOverlayTexturePath);
        }

        private void EnsureHoverOverlay()
        {
            if (!HasHoverOverlay() || hoverOverlayImage != null)
                return;

            Texture2D texture = Resources.Load<Texture2D>(hoverOverlayTexturePath);
            if (texture == null)
                return;

            GameObject overlay = new GameObject("HoverOverlay");
            overlay.transform.SetParent(transform, false);
            overlay.transform.SetAsLastSibling();

            RectTransform overlayRect = overlay.AddComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = new Vector2(-hoverOverlayPadding.x, -hoverOverlayPadding.y);
            overlayRect.offsetMax = new Vector2(hoverOverlayPadding.x, hoverOverlayPadding.y);
            overlayRect.localScale = Vector3.one;

            hoverOverlayImage = overlay.AddComponent<Image>();
            Rect rect = new Rect(0f, 0f, texture.width, texture.height);
            hoverOverlayImage.sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100f);
            hoverOverlayImage.type = Image.Type.Simple;
            hoverOverlayImage.preserveAspect = true;
            hoverOverlayImage.raycastTarget = false;
        }

        private void ShowOverlay(float alpha)
        {
            EnsureHoverOverlay();

            if (hoverOverlayImage == null)
                return;

            Color color = hoverOverlayImage.color;
            color.a = alpha;
            hoverOverlayImage.color = color;
            hoverOverlayImage.gameObject.SetActive(true);
        }

        private void HideOverlay()
        {
            if (hoverOverlayImage != null)
                hoverOverlayImage.gameObject.SetActive(false);
        }

        private void PlayHoverSound()
        {
            if (hoverClip == null)
                hoverClip = Resources.Load<AudioClip>(HoverClipPath);

            if (hoverClip == null)
                return;

            EnsureUiAudioSource().PlayOneShot(hoverClip, hoverVolume);
        }

        private static AudioSource EnsureUiAudioSource()
        {
            if (uiAudioSource != null)
                return uiAudioSource;

            GameObject audioObject = new GameObject("CleanWave_UIHoverAudio");
            DontDestroyOnLoad(audioObject);
            uiAudioSource = audioObject.AddComponent<AudioSource>();
            uiAudioSource.playOnAwake = false;
            uiAudioSource.spatialBlend = 0f;
            return uiAudioSource;
        }
    }
}
