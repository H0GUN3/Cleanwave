using System;
using CleanWave.Core;
using UnityEngine;

public class ZoneTrashProgress : MonoBehaviour
{
    [SerializeField] int cityTotalTrash = CleanWaveGameConstants.CityTrashCount;
    [SerializeField] int riverTotalTrash = CleanWaveGameConstants.RiverTrashCount;
    [SerializeField] int coastTotalTrash = CleanWaveGameConstants.CoastTrashCount;

    [SerializeField] int cityCollectedTrash;
    [SerializeField] int riverCollectedTrash;
    [SerializeField] int coastCollectedTrash;

    public static ZoneTrashProgress Instance { get; private set; }

    public event Action<CleanWaveZoneType, int, int, int> ProgressChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[ZoneTrashProgress] Multiple progress trackers found. Using the first active instance.", this);
            return;
        }

        ApplyConfiguredTotals();
        Instance = this;
    }

    void ApplyConfiguredTotals()
    {
        cityTotalTrash = CleanWaveGameConstants.CityTrashCount;
        riverTotalTrash = CleanWaveGameConstants.RiverTrashCount;
        coastTotalTrash = CleanWaveGameConstants.CoastTrashCount;
    }

    void Start()
    {
        Notify(CleanWaveZoneType.City);
        Notify(CleanWaveZoneType.River);
        Notify(CleanWaveZoneType.Coast);
    }

    public void RecordCollected(CleanWaveZoneType zoneType)
    {
        int total = GetTotalTrash(zoneType);
        if (total <= 0)
            return;

        int collected = Mathf.Min(GetCollectedTrash(zoneType) + 1, total);
        SetCollectedTrash(zoneType, collected);
        Notify(zoneType);
    }

    public int GetCollectedTrash(CleanWaveZoneType zoneType)
    {
        switch (zoneType)
        {
            case CleanWaveZoneType.City:
                return cityCollectedTrash;
            case CleanWaveZoneType.River:
                return riverCollectedTrash;
            case CleanWaveZoneType.Coast:
                return coastCollectedTrash;
            default:
                return 0;
        }
    }

    public int GetTotalTrash(CleanWaveZoneType zoneType)
    {
        switch (zoneType)
        {
            case CleanWaveZoneType.City:
                return cityTotalTrash;
            case CleanWaveZoneType.River:
                return riverTotalTrash;
            case CleanWaveZoneType.Coast:
                return coastTotalTrash;
            default:
                return 0;
        }
    }

    public int GetCollectedPercent(CleanWaveZoneType zoneType)
    {
        int total = GetTotalTrash(zoneType);
        if (total <= 0)
            return 0;

        return Mathf.FloorToInt(GetCollectedTrash(zoneType) * 100f / total);
    }

    void SetCollectedTrash(CleanWaveZoneType zoneType, int collected)
    {
        switch (zoneType)
        {
            case CleanWaveZoneType.City:
                cityCollectedTrash = collected;
                break;
            case CleanWaveZoneType.River:
                riverCollectedTrash = collected;
                break;
            case CleanWaveZoneType.Coast:
                coastCollectedTrash = collected;
                break;
        }
    }

    void Notify(CleanWaveZoneType zoneType)
    {
        ProgressChanged?.Invoke(
            zoneType,
            GetCollectedTrash(zoneType),
            GetTotalTrash(zoneType),
            GetCollectedPercent(zoneType));
    }
}
