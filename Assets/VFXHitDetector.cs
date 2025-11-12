using UnityEngine;

[RequireComponent(typeof(Transform))]
public class VFXHitDetector : MonoBehaviour
{
    [Tooltip("Layers that should be considered breakable walls.")]
    public LayerMask wallLayerMask = ~0; // default = everything

    [Tooltip("Whether to destroy this GameObject after hitting a breakable wall.")]
    public bool destroyOnHit = false;

    [Tooltip("Maximum distance to check per frame (safety clamp).")]
    public float maxSegmentDistance = 50f;

    // If your VFX moves using velocity, you may want this true to use Rigidbody velocity (if present)
    public bool useRigidbodyVelocityIfAvailable = false;

    // previous frame position for segment raycast
    private Vector3 previousPosition;

    void Start()
    {
        previousPosition = transform.position;
    }

    void Update()
    {

        Debug.DrawLine(previousPosition, transform.position, Color.red, 0.2f);
        Vector3 currentPosition = transform.position;

        // Determine ray origin/direction using previous/current position to avoid tunneling
        Vector3 dir = currentPosition - previousPosition;
        float dist = dir.magnitude;

        // If VFX is stationary and we want to shoot a short forward ray, fallback to forward ray
        if (dist <= 0.0001f)
        {
            dir = transform.forward;
            dist = 0.1f;
        }

        // safety clamp
        if (dist > maxSegmentDistance)
        {
            dir = dir.normalized * maxSegmentDistance;
            dist = maxSegmentDistance;
        }

        if (dist > 0f)
        {
            RaycastHit[] hits = Physics.RaycastAll(previousPosition, dir.normalized, dist, wallLayerMask, QueryTriggerInteraction.Collide);
            if (hits != null && hits.Length > 0)
            {
                // find the closest hit along the segment
                System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
                foreach (var hit in hits)
                {
                    if (TryHandleHit(hit)) { return; } // stop after the first handled hit
                }
            }
        }

        previousPosition = currentPosition;
    }

    bool TryHandleHit(RaycastHit hit)
    {
        // Look for your breakable component on the hit collider or its parents
        var breakable = hit.collider.GetComponentInParent<BreakOnParticleCollision>();
        if (breakable != null)
        {
            // Make sure BreakAtPoint is public on BreakOnParticleCollision
            breakable.BreakAtPoint(hit.point, hit.normal);

            if (destroyOnHit)
                Destroy(gameObject);

            return true;
        }

        // Optionally handle other kinds of walls/objects here

        return false;
    }
}
