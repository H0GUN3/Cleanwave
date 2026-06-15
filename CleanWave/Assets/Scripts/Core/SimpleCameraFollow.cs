using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new Vector3(0f, 0f, -10f);
    [SerializeField, Min(1f)] float orthographicSize = 8f;
    [SerializeField] bool clampToMapBounds = true;
    [SerializeField] Transform boundsRoot;
    [SerializeField] string boundsRootName = "Map_Blockers";
    [SerializeField] Vector2 boundsPadding;

    Camera followCamera;
    bool hasBounds;
    Bounds mapBounds;

    void Awake()
    {
        followCamera = GetComponent<Camera>();
        ApplyCameraSize();
        ResolveMapBounds();
    }

    void OnValidate()
    {
        ApplyCameraSize();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
            transform.position = ClampPosition(target.position + offset);
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        transform.position = ClampPosition(target.position + offset);
    }

    void ApplyCameraSize()
    {
        Camera targetCamera = followCamera != null ? followCamera : GetComponent<Camera>();
        if (targetCamera != null && targetCamera.orthographic)
            targetCamera.orthographicSize = orthographicSize;
    }

    void ResolveMapBounds()
    {
        hasBounds = false;

        if (!clampToMapBounds)
            return;

        Transform root = boundsRoot;
        if (root == null && !string.IsNullOrWhiteSpace(boundsRootName))
        {
            GameObject foundRoot = GameObject.Find(boundsRootName);
            if (foundRoot != null)
                root = foundRoot.transform;
        }

        if (root == null)
            return;

        Collider2D[] colliders = root.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D mapCollider in colliders)
        {
            if (!hasBounds)
            {
                mapBounds = mapCollider.bounds;
                hasBounds = true;
                continue;
            }

            mapBounds.Encapsulate(mapCollider.bounds);
        }

        if (hasBounds && boundsPadding != Vector2.zero)
            mapBounds.Expand(new Vector3(boundsPadding.x * 2f, boundsPadding.y * 2f, 0f));
    }

    Vector3 ClampPosition(Vector3 desiredPosition)
    {
        if (!clampToMapBounds || !hasBounds)
            return desiredPosition;

        Camera targetCamera = followCamera != null ? followCamera : GetComponent<Camera>();
        if (targetCamera == null || !targetCamera.orthographic)
            return desiredPosition;

        float halfHeight = targetCamera.orthographicSize;
        float halfWidth = halfHeight * targetCamera.aspect;
        float minX = mapBounds.min.x + halfWidth;
        float maxX = mapBounds.max.x - halfWidth;
        float minY = mapBounds.min.y + halfHeight;
        float maxY = mapBounds.max.y - halfHeight;

        if (minX > maxX)
            desiredPosition.x = mapBounds.center.x;
        else
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);

        if (minY > maxY)
            desiredPosition.y = mapBounds.center.y;
        else
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);

        return desiredPosition;
    }
}
