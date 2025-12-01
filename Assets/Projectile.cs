using UnityEngine;

[DisallowMultipleComponent]
public class Projectile : MonoBehaviour
{
    [Header("Lifetime")]
    public float lifeSeconds = 5f;

    void OnEnable()
    {
        // Auto-despawn if nothing is hit
        Destroy(gameObject, lifeSeconds);
    }

    void OnTriggerEnter(Collider other)
    {
        if (Ignore(other)) return;

        // kill enemy + score
        if (other.TryGetComponent<Enemy>(out var enemy))
        {
            if (enemy) enemy.Kill();
            var score = FindFirstObjectByType<PlayerScore>();
            if (score) score.Add(1);
        }

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision c)
    {
        if (Ignore(c.collider)) return;

        if (c.collider.TryGetComponent<Enemy>(out var enemy))
        {
            if (enemy) enemy.Kill();
            var score = FindFirstObjectByType<PlayerScore>();
            if (score) score.Add(1);
        }

        Destroy(gameObject);
    }

    bool Ignore(Collider col)
    {
        // don’t pop on our own projectiles or the player rig
        if (col.CompareTag("Projectile")) return true;
        if (col.GetComponentInParent<PlayerHealth>()) return true;
        return false;
    }
}
