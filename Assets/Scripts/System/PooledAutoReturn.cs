using UnityEngine;

namespace CleanWave.Systems
{
    public class PooledAutoReturn : MonoBehaviour
    {
        [SerializeField] private ObjectPool pool;
        [SerializeField] private string poolKey;
        [SerializeField] private float lifeSeconds = 1f;

        private float _expireTime;

        private void OnEnable()
        {
            _expireTime = Time.time + lifeSeconds;
        }

        private void Update()
        {
            if (Time.time >= _expireTime)
            {
                ReturnToPool();
            }
        }

        public void ReturnToPool()
        {
            if (pool != null && !string.IsNullOrWhiteSpace(poolKey))
            {
                pool.Despawn(poolKey, gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
