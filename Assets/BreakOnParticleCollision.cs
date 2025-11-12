using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BreakOnParticleCollision : MonoBehaviour
{
    [Tooltip("Prefab containing the pre-broken pieces (root of fragments).")]
    public GameObject brokenWallPrefab;

    [Tooltip("Destroy this intact wall after breaking (true) or just hide visuals & colliders (false).")]
    public bool destroyOriginal = true;

    [Tooltip("Disable original colliders before spawning fragments to avoid immediate overlap collisions).")]
    public bool disableOriginalColliderBeforeSpawn = true;

    [Tooltip("If > 0, destroyed broken prefab after this many seconds (helps cleanup).")]
    public float brokenLifetime = 15f;

    private bool isBroken = false;
    private readonly List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    void OnParticleCollision(GameObject other)
    {
        if (isBroken) return;
        if (brokenWallPrefab == null)
        {
            Debug.LogWarning($"[{name}] brokenWallPrefab is not set.");
            return;
        }

        var ps = other.GetComponent<ParticleSystem>();
        if (ps == null)
        {
            BreakAtPoint(transform.position, transform.forward);
            return;
        }

        int num = ParticlePhysicsExtensions.GetCollisionEvents(ps, gameObject, collisionEvents);
        if (num == 0)
        {
            BreakAtPoint(transform.position, transform.forward);
            return;
        }

        var evt = collisionEvents[0];
        BreakAtPoint(evt.intersection, evt.normal);
    }

    public void BreakAtPoint(Vector3 impactPointWorld, Vector3 impactNormalWorld)
    {
        if (isBroken) return;
        isBroken = true;

        if (disableOriginalColliderBeforeSpawn)
        {
            var origColliders = GetComponentsInChildren<Collider>();
            foreach (var c in origColliders)
                if (c != null) c.enabled = false;
        }

        // Instantiate broken prefab in world space (no parent)
        GameObject brokenInstance = Instantiate(brokenWallPrefab);

        // Match the original object's world position & rotation
        brokenInstance.transform.position = transform.position;
        brokenInstance.transform.rotation = transform.rotation;

        //// IMPORTANT: use lossyScale (world scale) so scale matches exactly irrespective of parent scales
        //brokenInstance.transform.localScale = transform.lossyScale;

        if (brokenLifetime > 0f)
            Destroy(brokenInstance, brokenLifetime);

        if (destroyOriginal)
        {
            Destroy(gameObject);
        }
        else
        {
            var rends = GetComponentsInChildren<Renderer>();
            foreach (var r in rends) if (r != null) r.enabled = false;
        }
    }
}
