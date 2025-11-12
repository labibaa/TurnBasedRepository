using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class BreakOnParticleCollision : MonoBehaviour
{
    [Tooltip("Prefab containing the pre-broken pieces (root of fragments).")]
    public GameObject brokenWallPrefab;

    [Tooltip("How strong the impulse applied to fragments is (from collision point).")]
    public float fragmentImpulse = 300f;

    [Tooltip("Radius used when applying explosion force to fragments.")]
    public float fragmentRadius = 2f;

    [Tooltip("Destroy this intact wall after breaking (true) or just disable visuals/collider (false).")]
    public bool destroyOriginal = true;

    // Optional: only particle systems with this tag will break the wall (empty = any)
    public string allowedParticleSystemTag = "";

    // Class-level flag — NOT shadowed by any local variable
    private bool isBroken = false;

    // Reusable list for particle collision events to avoid allocations
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    void OnParticleCollision(GameObject other)
    {
        if (isBroken) return;

        if (brokenWallPrefab == null)
        {
            Debug.LogWarning($"[{name}] brokenWallPrefab is not set.");
            return;
        }

        // If a tag is set, ignore other particle systems that don't match
        if (!string.IsNullOrEmpty(allowedParticleSystemTag) && !other.CompareTag(allowedParticleSystemTag))
            return;

        var ps = other.GetComponent<ParticleSystem>();
        if (ps == null)
        {
            // no particle system found on the other object; break at wall center
            BreakAtPoint(transform.position, transform.forward);
            return;
        }

        // Get collision events for this particle system against this gameobject
        int num = ParticlePhysicsExtensions.GetCollisionEvents(ps, gameObject, collisionEvents);
        if (num == 0)
        {
            // fallback: break at wall center
            BreakAtPoint(transform.position, transform.forward);
            return;
        }

        // Use the first collision event
        var evt = collisionEvents[0];
        Vector3 impactPoint = evt.intersection;
        Vector3 impactNormal = evt.normal;

        BreakAtPoint(impactPoint, impactNormal);
    }

    public void BreakAtPoint(Vector3 impactPointWorld, Vector3 impactNormalWorld)
    {
        // Use the class-level flag
        if (isBroken) return;
        isBroken = true;

        // Instantiate broken pieces so they align with this wall's transform.
        GameObject brokenInstance = Instantiate(brokenWallPrefab, transform.position, transform.rotation, transform.parent);

        // Match scale (useful if the original wall was scaled)
        //brokenInstance.transform.localScale = transform.localScale;

        // Apply explosion/impulse to any child rigidbodies
        Rigidbody[] rbs = brokenInstance.GetComponentsInChildren<Rigidbody>();
        if (rbs != null && rbs.Length > 0)
        {
            foreach (var rb in rbs)
            {
                if (rb == null) continue;
                rb.isKinematic = false; // ensure physics active
                rb.AddExplosionForce(fragmentImpulse, impactPointWorld, fragmentRadius, 0.5f, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * (fragmentImpulse * 0.01f), ForceMode.Impulse);
            }
        }

        // Optional: if broken prefab contains a FragmentInitialImpulse script, trigger it
        var init = brokenInstance.GetComponentInChildren<FragmentInitialImpulse>();
        if (init != null)
        {
            init.InitializeFromImpact(impactPointWorld, impactNormalWorld);
        }

        // Disable or destroy the original intact wall
        if (destroyOriginal)
        {
            Destroy(gameObject);
        }
        else
        {
            var renderers = GetComponentsInChildren<Renderer>();
            foreach (var r in renderers) r.enabled = false;
            var cols = GetComponentsInChildren<Collider>();
            foreach (var c in cols) c.enabled = false;
        }
    }
}
