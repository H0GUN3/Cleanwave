using UnityEngine;
using CleanWave.Systems;

namespace CleanWave.NPC
{
    public class ShopNPC : MonoBehaviour
    {
        [SerializeField] private string npcName = "Recycle Shop";
        [SerializeField] private float interactionRadius = 2f;
        
        private bool _playerInRange;

        private void Update()
        {
            if (_playerInRange && Input.GetKeyDown(KeyCode.E))
            {
                OpenShop();
            }
        }

        private void OpenShop()
        {
            Debug.Log($"Opening shop for {npcName}");
            // In a real MVP, this would trigger a UI panel.
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = false;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }
}
