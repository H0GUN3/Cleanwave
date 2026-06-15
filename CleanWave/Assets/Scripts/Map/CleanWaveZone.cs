using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CleanWaveZone : MonoBehaviour
{
    [SerializeField] CleanWaveZoneType zoneType;

    public CleanWaveZoneType ZoneType => zoneType;

    void Reset()
    {
        Collider2D zoneCollider = GetComponent<Collider2D>();
        zoneCollider.isTrigger = true;
    }

    void OnValidate()
    {
        Collider2D zoneCollider = GetComponent<Collider2D>();
        if (zoneCollider != null)
            zoneCollider.isTrigger = true;
    }
}
