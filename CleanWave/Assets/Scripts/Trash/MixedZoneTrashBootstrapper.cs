using System.Collections.Generic;
using CleanWave.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MixedZoneTrashBootstrapper
{
    const string RuntimeTrashPrefix = "Runtime_Mixed_Trash";
    const float SpawnJitterRadius = 0.45f;
    static bool sceneLoadedSubscribed;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void EnsureMixedZoneTrash()
    {
        SubscribeSceneLoaded();

        if (!GameplaySceneUtility.IsGameplayScene())
            return;

        Transform cityGroup = FindGroup("City_Trash");
        Transform riverGroup = FindGroup("River_Trash");
        Transform coastGroup = FindGroup("Coast_Trash");

        if (cityGroup == null || riverGroup == null || coastGroup == null)
            return;

        List<TrashPickup> cityTrash = GetAuthoredTrash(cityGroup);
        List<TrashPickup> riverTrash = GetAuthoredTrash(riverGroup);
        List<TrashPickup> coastTrash = GetAuthoredTrash(coastGroup);

        SpawnUntilTarget(
            riverGroup,
            CleanWaveGameConstants.RiverTrashCount,
            Combine(cityTrash, riverTrash),
            riverTrash);

        SpawnUntilTarget(
            coastGroup,
            CleanWaveGameConstants.CoastTrashCount,
            Combine(cityTrash, riverTrash, coastTrash),
            coastTrash);
    }

    static void SubscribeSceneLoaded()
    {
        if (sceneLoadedSubscribed)
            return;

        SceneManager.sceneLoaded += HandleSceneLoaded;
        sceneLoadedSubscribed = true;
    }

    static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureMixedZoneTrash();
    }

    static Transform FindGroup(string name)
    {
        GameObject found = GameObject.Find(name);
        return found != null ? found.transform : null;
    }

    static List<TrashPickup> GetAuthoredTrash(Transform group)
    {
        TrashPickup[] allTrash = group.GetComponentsInChildren<TrashPickup>(true);
        List<TrashPickup> authoredTrash = new List<TrashPickup>(allTrash.Length);
        foreach (TrashPickup trash in allTrash)
        {
            if (trash != null && !trash.name.StartsWith(RuntimeTrashPrefix, System.StringComparison.OrdinalIgnoreCase))
                authoredTrash.Add(trash);
        }

        return authoredTrash;
    }

    static List<TrashPickup> Combine(params List<TrashPickup>[] groups)
    {
        List<TrashPickup> combined = new List<TrashPickup>();
        foreach (List<TrashPickup> group in groups)
        {
            if (group == null)
                continue;

            combined.AddRange(group);
        }

        return combined;
    }

    static void SpawnUntilTarget(Transform targetGroup, int targetCount, List<TrashPickup> sourcePool, List<TrashPickup> positionPool)
    {
        if (targetGroup == null || sourcePool == null || sourcePool.Count == 0 || positionPool == null || positionPool.Count == 0)
            return;

        int existingCount = targetGroup.GetComponentsInChildren<TrashPickup>(true).Length;
        for (int index = existingCount; index < targetCount; index++)
        {
            TrashPickup source = sourcePool[Random.Range(0, sourcePool.Count)];
            TrashPickup positionSample = positionPool[Random.Range(0, positionPool.Count)];
            if (source == null || positionSample == null)
                continue;

            Vector2 jitter = Random.insideUnitCircle * SpawnJitterRadius;
            Vector3 spawnPosition = positionSample.transform.position + new Vector3(jitter.x, jitter.y, 0f);
            GameObject clone = Object.Instantiate(source.gameObject, spawnPosition, source.transform.rotation, targetGroup);
            clone.name = $"{RuntimeTrashPrefix}_{targetGroup.name}_{index:00}_{source.name}";
            clone.SetActive(source.gameObject.activeSelf);
        }
    }
}
