using System.Collections.Generic;
using UnityEngine;

namespace CleanWave.Systems
{
    public class ObjectPool : MonoBehaviour
    {
        [System.Serializable]
        public class PoolEntry
        {
            public string key;
            public GameObject prefab;
            public int initialSize = 10;
        }

        [SerializeField] private List<PoolEntry> entries = new();
        private readonly Dictionary<string, Queue<GameObject>> _pools = new();
        private readonly Dictionary<string, GameObject> _prefabByKey = new();

        private void Awake()
        {
            foreach (var entry in entries)
            {
                if (string.IsNullOrWhiteSpace(entry.key) || entry.prefab == null)
                {
                    continue;
                }

                if (!_pools.ContainsKey(entry.key))
                {
                    _pools[entry.key] = new Queue<GameObject>();
                    _prefabByKey[entry.key] = entry.prefab;
                }

                for (int i = 0; i < entry.initialSize; i++)
                {
                    var instance = Instantiate(entry.prefab, transform);
                    instance.SetActive(false);
                    _pools[entry.key].Enqueue(instance);
                }
            }
        }

        public GameObject Spawn(string key, Vector3 position, Quaternion rotation)
        {
            if (!_pools.TryGetValue(key, out var queue))
            {
                return null;
            }

            GameObject instance;
            if (queue.Count > 0)
            {
                instance = queue.Dequeue();
            }
            else
            {
                instance = Instantiate(_prefabByKey[key], transform);
            }

            instance.transform.SetPositionAndRotation(position, rotation);
            instance.SetActive(true);
            return instance;
        }

        public void Despawn(string key, GameObject instance)
        {
            if (instance == null || !_pools.TryGetValue(key, out var queue))
            {
                return;
            }

            instance.SetActive(false);
            instance.transform.SetParent(transform);
            queue.Enqueue(instance);
        }
    }
}
