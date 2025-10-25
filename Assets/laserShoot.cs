using UnityEngine;

public class laserShoot : MonoBehaviour
{
    [SerializeField] private float defDistanceRay = 100;
    public Transform laserFirePoint;
    Transform m_transform;

    // Direction in local space of the fire point. Default: down (0,-1).
    [SerializeField] private Vector2 localDirection = Vector2.down;

    // Layers to hit (default = everything)
    [SerializeField] private LayerMask hitLayers = ~0;

    public LineRenderer m_lineRenderer;

    private void Awake()
    {
        m_transform = GetComponent<Transform>();
        // Ensure the LineRenderer has two positions available
        if (m_lineRenderer != null)
        {
            m_lineRenderer.positionCount = 2;
        }
    }

    private void Update()
    {
        ShootLaser();
    }

    void ShootLaser()
    {
        if (m_lineRenderer == null)
        {
            // Nothing to draw to
            return;
        }

        // Origin: prefer the configured fire point, otherwise the object transform
        Vector2 origin = (laserFirePoint != null) ? (Vector2)laserFirePoint.position : (Vector2)m_transform.position;

        // Direction: transform the localDirection by the fire point (or object) so local down points downward relative to the transform
        Vector2 direction;
        if (laserFirePoint != null)
            direction = (Vector2)laserFirePoint.TransformDirection(localDirection);
        else
            direction = (Vector2)m_transform.TransformDirection(localDirection);

        // Single raycast (with max distance)
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, defDistanceRay, hitLayers);

        if (hit.collider != null)
        {
            Draw2DRay(origin, hit.point);
        }
        else
        {
            // No hit: draw to origin + direction * distance
            Vector2 endPoint = origin + direction.normalized * defDistanceRay;
            Draw2DRay(origin, endPoint);
        }
    }

    void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        m_lineRenderer.SetPosition(0, startPos);
        m_lineRenderer.SetPosition(1, endPos);
    }

    // Public helpers so other scripts (like enemy controllers) can query the
    // laser's origin, world direction and max distance for damage checks.
    public Vector2 GetOrigin()
    {
        return (laserFirePoint != null) ? (Vector2)laserFirePoint.position : (Vector2)m_transform.position;
    }

    public Vector2 GetWorldDirection()
    {
        if (laserFirePoint != null)
            return (Vector2)laserFirePoint.TransformDirection(localDirection).normalized;
        return (Vector2)m_transform.TransformDirection(localDirection).normalized;
    }

    public float GetMaxDistance()
    {
        return defDistanceRay;
    }
}
