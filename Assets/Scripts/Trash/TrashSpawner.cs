using System.Collections.Generic;
using CleanWave.Systems;
using UnityEngine;

namespace CleanWave.Trash
{
    public class TrashSpawner : MonoBehaviour
    {
        [System.Serializable]
        public class SpawnEntry
        {
            public TrashObject Prefab;
            [Range(0f, 1f)] public float Weight = 0.2f;
        }

        [SerializeField] private List<SpawnEntry> spawnEntries = new();
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private int initialSpawnCount = 30;
        [SerializeField] private bool enableRespawn;
        [SerializeField] private float respawnInterval = 10f;
        [SerializeField] private bool useObjectPool = true;
        [SerializeField] private ObjectPool objectPool;

        private float _nextRespawnTime;

        private void Start()
        {
            for (int i = 0; i < initialSpawnCount; i++)
            {
                SpawnSingle();
            }

            _nextRespawnTime = Time.time + respawnInterval;
        }

        private void Update()
        {
            if (!enableRespawn || Time.time < _nextRespawnTime)
            {
                return;
            }

            SpawnSingle();
            _nextRespawnTime = Time.time + respawnInterval;
        }

        private void SpawnSingle()
        {
            var prefab = PickWeightedPrefab();
            if (prefab == null || spawnPoints == null || spawnPoints.Length == 0)
            {
                return;
            }

            var point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            if (point == null)
            {
                return;
            }

            if (useObjectPool && objectPool != null)
            {
                var poolKey = prefab.name;
                var pooled = objectPool.Spawn(poolKey, point.position, Quaternion.identity);
                if (pooled != null)
                {
                    pooled.transform.SetParent(transform);
                    PrepareSpawnedTrash(pooled, prefab);
                    return;
                }
            }

            var spawned = Instantiate(prefab, point.position, Quaternion.identity, transform);
            PrepareSpawnedTrash(spawned.gameObject, prefab);
        }

        private static void PrepareSpawnedTrash(GameObject instance, TrashObject sourcePrefab)
        {
            if (instance == null || sourcePrefab == null)
            {
                return;
            }

            // Keep layer/tag consistent even if source scene objects were misconfigured.
            instance.layer = sourcePrefab.gameObject.layer;
            instance.tag = sourcePrefab.gameObject.tag;

            if (instance.TryGetComponent<TrashObject>(out var trashObject))
            {
                trashObject.ResetForReuse();
            }
        }

        private TrashObject PickWeightedPrefab()
        {
            if (spawnEntries == null || spawnEntries.Count == 0)
            {
                return null;
            }

            float totalWeight = 0f;
            foreach (var entry in spawnEntries)
            {
                if (entry.Prefab != null && entry.Weight > 0f)
                {
                    totalWeight += entry.Weight;
                }
            }

            if (totalWeight <= 0f)
            {
                return null;
            }

            float random = Random.Range(0f, totalWeight);
            float cumulative = 0f;
            foreach (var entry in spawnEntries)
            {
                if (entry.Prefab == null || entry.Weight <= 0f)
                {
                    continue;
                }

                cumulative += entry.Weight;
                if (random <= cumulative)
                {
                    return entry.Prefab;
                }
            }

            return spawnEntries[spawnEntries.Count - 1].Prefab;
        }
    }
}
