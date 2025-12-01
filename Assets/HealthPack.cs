using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class HealthPack : MonoBehaviour
{
    [Header("Heal")]
    public int healAmount = 25;

    void Reset()
    {
        var col = GetComponent<Collider>() ?? gameObject.AddComponent<SphereCollider>();
        col.isTrigger = true;
    }

    public void Collect(PlayerHealth player)
    {
        if (!player) return;

        player.Heal(healAmount);
        Destroy(gameObject);
    }
}
