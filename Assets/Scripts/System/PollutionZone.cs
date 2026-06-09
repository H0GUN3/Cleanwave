using CleanWave.Player;
using UnityEngine;

namespace CleanWave.Systems
{
    [RequireComponent(typeof(Collider2D))]
    public class PollutionZone : MonoBehaviour
    {
        [Range(0.2f, 1f)]
        [SerializeField] private float speedMultiplier = 0.65f;
        [SerializeField] private float batteryDrainPerSecond = 1.5f;

        private PlayerController _playerInZone;
        private StageManager _stageManager;

        private void Start()
        {
            _stageManager = FindObjectOfType<StageManager>();
        }

        private void Update()
        {
            if (_playerInZone != null && _stageManager != null)
            {
                _stageManager.ConsumeBattery(batteryDrainPerSecond * Time.deltaTime);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<PlayerController>(out var controller))
            {
                return;
            }

            _playerInZone = controller;
            controller.SetMovementPenalty(speedMultiplier);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.TryGetComponent<PlayerController>(out var controller))
            {
                return;
            }

            controller.SetMovementPenalty(1f);
            if (_playerInZone == controller)
            {
                _playerInZone = null;
            }
        }
    }
}
