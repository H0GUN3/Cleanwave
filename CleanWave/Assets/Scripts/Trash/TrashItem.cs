using UnityEngine;

namespace CleanWave
{
    public class TrashItem : MonoBehaviour
    {
        [SerializeField] private TrashType trashType;
        [SerializeField] private ZoneType zoneType;

        public TrashType TrashType => trashType;
        public ZoneType ZoneType => zoneType;
        public float PollutionValue => TrashBinMapping.GetPollutionValue(trashType);
        public int CoinValue => TrashBinMapping.GetCoinValue(trashType);
        public bool IsPickedUp { get; private set; }

        private ZoneManager myZone;

        private void Start()
        {
            RegisterWithZone();
        }

        private void RegisterWithZone()
        {
            ZoneManager[] zones = FindObjectsByType<ZoneManager>(FindObjectsSortMode.None);
            foreach (var zone in zones)
            {
                if (zone.ZoneType == zoneType)
                {
                    myZone = zone;
                    zone.RegisterTrash(this);
                    break;
                }
            }

            if (myZone == null)
                Debug.LogWarning($"[TrashItem] ZoneManager({zoneType})를 찾을 수 없습니다: {name}");
        }

        public void OnPickedUp()
        {
            IsPickedUp = true;
            gameObject.SetActive(false);
        }

        public void OnDeposited(bool correct)
        {
            if (correct)
                myZone?.OnTrashDeposited(PollutionValue);
        }
    }
}
