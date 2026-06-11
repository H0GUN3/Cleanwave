using UnityEngine;

namespace CleanWave
{
    public class TrashBin : MonoBehaviour
    {
        [SerializeField] private BinType binType;
        [SerializeField] private ZoneType zoneType;

        public BinType BinType => binType;

        public bool TryDeposit(TrashType trashType)
        {
            BinType correct = TrashBinMapping.GetCorrectBin(trashType);
            bool isCorrect = (correct == binType);
            return isCorrect;
        }
    }
}
