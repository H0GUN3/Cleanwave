using UnityEngine;

namespace CleanWave.Systems
{
    [RequireComponent(typeof(Collider2D))]
    public class ChargeStation : MonoBehaviour
    {
        [SerializeField] private StageManager stageManager;
        [SerializeField] private float chargePerSecond = 10f;
        private bool _isCharging;

        private void Update()
        {
            if (_isCharging && stageManager != null)
            {
                stageManager.RecoverBattery(chargePerSecond * Time.deltaTime);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isCharging = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isCharging = false;
            }
        }
    }
}
