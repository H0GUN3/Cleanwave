using UnityEngine;
using UnityEngine.Events;

public class CurrentZoneTracker : MonoBehaviour
{
    [System.Serializable]
    public class ZoneChangedEvent : UnityEvent<CleanWaveZoneType> { }

    [SerializeField] CleanWaveZoneType currentZone = CleanWaveZoneType.City;
    [SerializeField] bool logZoneChanges = true;

    public ZoneChangedEvent OnZoneChanged = new ZoneChangedEvent();

    public CleanWaveZoneType CurrentZone => currentZone;

    void OnTriggerEnter2D(Collider2D other)
    {
        CleanWaveZone zone = other.GetComponent<CleanWaveZone>();
        if (zone == null)
            return;

        SetCurrentZone(zone.ZoneType);
    }

    void SetCurrentZone(CleanWaveZoneType zoneType)
    {
        if (currentZone == zoneType)
            return;

        currentZone = zoneType;
        OnZoneChanged.Invoke(currentZone);

        if (logZoneChanges)
            Debug.Log($"[CurrentZoneTracker] Current zone: {currentZone}");
    }
}
