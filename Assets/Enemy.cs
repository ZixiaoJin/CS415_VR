using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class Enemy : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 30;

    bool hasDealtDamage;

    void Reset()
    {
        var col = GetComponent<Collider>() ?? gameObject.AddComponent<SphereCollider>();
        col.isTrigger = true;
    }

    void OnEnable()
    {
        hasDealtDamage = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasDealtDamage) return;

        var playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null) return;

        playerHealth.TakeDamage(damage);
        hasDealtDamage = true;

        // Enemy is removed after dealing damage
        Destroy(gameObject);
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
