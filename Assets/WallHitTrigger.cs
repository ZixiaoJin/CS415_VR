using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class WallHitTrigger : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 30;
    public bool oneShot = true;

    [Header("Events")]
    public UnityEvent onPlayerDamaged;

    bool fired;
    public bool WasHit { get; private set; }

    void OnEnable()
    {
        fired = false;
        WasHit = false;                        // reset when segment is (re)enabled/pooled
    }

    void Reset()
    {
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (oneShot && fired) return;

        var health = other.GetComponentInParent<PlayerHealth>();
        if (health == null) return;

        WasHit = true;                         // player hit this wall
        health.TakeDamage(damage);
        onPlayerDamaged?.Invoke();

        if (oneShot) fired = true;
    }
}
