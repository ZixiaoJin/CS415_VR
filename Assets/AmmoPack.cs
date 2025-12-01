using UnityEngine;

[DisallowMultipleComponent]
public class AmmoPack : MonoBehaviour
{
    [Header("Ammo")]
    public int rounds = 3;   // each pickup gives +3 rounds

    void Reset()
    {
        // Trigger-only sphere, fixed radius 0.5 as you wanted
        var col = GetComponent<Collider>() ?? gameObject.AddComponent<SphereCollider>();
        col.isTrigger = true;
    }

    public void Collect(PlayerAmmo playerAmmo)
    {
        if (!playerAmmo) return;
        playerAmmo.AddAmmo(rounds);
        Destroy(gameObject);
    }
}
