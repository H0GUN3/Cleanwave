using System;
using CleanWave.Systems;
using UnityEngine;

namespace CleanWave.Trash
{
    [RequireComponent(typeof(Collider2D))]
    public class TrashObject : MonoBehaviour
    {
        [SerializeField] private TrashType trashType;
        [SerializeField] private int pickupScore = 10;
        [SerializeField] private float interactionRadius = 1.2f;

        public TrashType Type => trashType;
        public int PickupScore => pickupScore;
        public float InteractionRadius => interactionRadius;
        public bool IsCollected { get; private set; }

        public event Action<TrashObject> PickedUp;

        public bool TryPickup(Transform collector, InventoryManager inventoryManager)
        {
            if (IsCollected || inventoryManager == null || collector == null)
            {
                return false;
            }

            if (!inventoryManager.TryAddTrash(trashType))
            {
                return false;
            }

            IsCollected = true;
            PickedUp?.Invoke(this);
            gameObject.SetActive(false);
            return true;
        }

        public void ResetForReuse()
        {
            IsCollected = false;
            gameObject.SetActive(true);
        }
    }
}
