using UnityEngine;
using System.Collections.Generic;

namespace CleanWave
{
    public class ZoneManager : MonoBehaviour
    {
        [SerializeField] private ZoneType zoneType;
        [SerializeField] private ZoneGate gateToNextZone;

        public ZoneType ZoneType => zoneType;

        private float totalPollution = 0f;
        private float removedPollution = 0f;
        private bool isCompleted = false;

        private List<TrashItem> registeredTrash = new List<TrashItem>();

        public float PurificationRate
        {
            get
            {
                if (totalPollution <= 0f) return 0f;
                return Mathf.Clamp01(removedPollution / totalPollution) * 100f;
            }
        }

        private void Start()
        {
            GameManager.Instance?.RegisterZone(this);
        }

        public void RegisterTrash(TrashItem trash)
        {
            if (!registeredTrash.Contains(trash))
            {
                registeredTrash.Add(trash);
                totalPollution += trash.PollutionValue;
            }
        }

        public void OnTrashDeposited(float pollutionValue)
        {
            removedPollution += pollutionValue;
            removedPollution = Mathf.Min(removedPollution, totalPollution);
            CheckCompletion();
        }

        private void CheckCompletion()
        {
            if (isCompleted) return;
            if (PurificationRate >= 80f)  // 80% is enough to advance (was 100f)
            {
                isCompleted = true;
                gateToNextZone?.OpenGate();
                GameManager.Instance?.OnZoneCompleted(this);
            }
        }
    }
}
