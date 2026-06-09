using UnityEngine;

namespace CleanWave.Player
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float smoothTime = 0.15f;
        [SerializeField] private Vector2 minBounds = new Vector2(-2f, -2f);
        [SerializeField] private Vector2 maxBounds = new Vector2(22f, 17f);

        private Vector3 _velocity;
        private Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            float vertExtent = _camera != null && _camera.orthographic ? _camera.orthographicSize : 5f;
            float horzExtent = _camera != null ? vertExtent * _camera.aspect : vertExtent * 1.77f;

            float clampX = Mathf.Clamp(target.position.x, minBounds.x + horzExtent, Mathf.Max(minBounds.x + horzExtent, maxBounds.x - horzExtent));
            float clampY = Mathf.Clamp(target.position.y, minBounds.y + vertExtent, Mathf.Max(minBounds.y + vertExtent, maxBounds.y - vertExtent));

            Vector3 goal = new Vector3(clampX, clampY, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, goal, ref _velocity, smoothTime);
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
