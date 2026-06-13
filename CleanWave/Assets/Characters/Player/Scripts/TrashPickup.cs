using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 쓰레기 오브젝트. 플레이어가 범위 안에서 E를 누르면 줍는다.
/// PlayerBag.TryAddTrash()가 true일 때만 제거된다.
/// </summary>
public class TrashPickup : MonoBehaviour
{
    [SerializeField] float pickupRange = 1.2f;

    static PlayerBag cachedBag;

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null || !keyboard.eKey.wasPressedThisFrame)
            return;

        PlayerBag bag = FindPlayerBag();
        if (bag == null)
            return;

        float dist = Vector2.Distance(transform.position, bag.transform.position);
        if (dist > pickupRange)
            return;

        if (bag.TryAddTrash())
        {
            Destroy(gameObject);
        }
        // 가방이 가득 차면 TryAddTrash가 false → 쓰레기를 제거하지 않는다.
    }

    PlayerBag FindPlayerBag()
    {
        if (cachedBag == null)
            cachedBag = FindFirstObjectByType<PlayerBag>();
        return cachedBag;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
