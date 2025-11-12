using UnityEngine;

public class FragmentInitialImpulse : MonoBehaviour
{
    [Tooltip("Extra explosion force applied to fragments on spawn.")]
    public float extraExplosionForce = 150f;
    public float extraRadius = 2f;
    public float lifetime = 8f;

    bool initialized = false;

    // If you want the prefab to apply its own force when created, call this
    // from the BreakOnParticleCollision script after instantiation.
    public void InitializeFromImpact(Vector3 impactPoint, Vector3 impactNormal)
    {
        if (initialized) return;
        initialized = true;

        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rbs)
        {
            if (rb == null) continue;
            rb.isKinematic = false;
            rb.AddExplosionForce(extraExplosionForce, impactPoint, extraRadius, 0.5f, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * (extraExplosionForce * 0.01f), ForceMode.Impulse);
        }

        if (lifetime > 0f)
            Destroy(gameObject, lifetime);
    }

    // If you want the prefab to self-initialize (no call from external), uncomment:
    // void Start() {
    //     // Optionally initialize using the prefab root as center
    //     InitializeFromImpact(transform.position, transform.forward);
    // }
}
