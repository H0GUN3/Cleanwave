using UnityEngine;

namespace CleanWave.Systems
{
    public class MapVisualController : MonoBehaviour
    {
        [SerializeField] private StageManager stageManager;
        [SerializeField] private SpriteRenderer[] pollutedRenderers;
        [SerializeField] private SpriteRenderer[] cleanRenderers;
        [SerializeField] private Color pollutedTint = new(0.45f, 0.45f, 0.45f, 1f);
        [SerializeField] private Color cleanTint = Color.white;

        private void OnEnable()
        {
            if (stageManager != null)
            {
                stageManager.StageUpdated += RefreshVisual;
            }
        }

        private void OnDisable()
        {
            if (stageManager != null)
            {
                stageManager.StageUpdated -= RefreshVisual;
            }
        }

        private void Start()
        {
            RefreshVisual();
        }

        private void RefreshVisual()
        {
            if (stageManager == null)
            {
                return;
            }

            var t = stageManager.ProgressPercent / 100f;
            var blend = Color.Lerp(pollutedTint, cleanTint, t);

            if (pollutedRenderers != null)
            {
                foreach (var renderer in pollutedRenderers)
                {
                    if (renderer != null)
                    {
                        renderer.color = blend;
                    }
                }
            }

            if (cleanRenderers != null)
            {
                foreach (var renderer in cleanRenderers)
                {
                    if (renderer != null)
                    {
                        var baseColor = renderer.color;
                        baseColor.a = Mathf.Clamp01(t);
                        renderer.color = baseColor;
                    }
                }
            }
        }
    }
}
