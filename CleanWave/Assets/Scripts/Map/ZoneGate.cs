using UnityEngine;

namespace CleanWave
{
    public class ZoneGate : MonoBehaviour
    {
        [SerializeField] private GameObject gateVisual;
        [SerializeField] private Collider2D gateCollider;

        private void Start()
        {
            if (gateVisual == null)
                gateVisual = gameObject;
            if (gateCollider == null)
                gateCollider = GetComponent<Collider2D>();
        }

        public void OpenGate()
        {
            if (gateVisual != null) gateVisual.SetActive(false);
            if (gateCollider != null) gateCollider.enabled = false;
        }

        public void CloseGate()
        {
            if (gateVisual != null) gateVisual.SetActive(true);
            if (gateCollider != null) gateCollider.enabled = true;
        }
    }
}
