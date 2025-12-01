using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class PlayerAmmo : MonoBehaviour
{
    [Header("Ammo")]
    public int ammo = 0;

    [Header("Events")]
    public UnityEvent<int> onAmmoChanged;   // passes current ammo

    public void AddAmmo(int amount)
    {
        if (amount <= 0) return;
        ammo += amount;
        onAmmoChanged?.Invoke(ammo);
        Debug.Log($"[PlayerAmmo] +{amount} (now {ammo})");
    }

    public bool TryConsume(int amount = 1)
    {
        if (ammo < amount) return false;
        ammo -= amount;
        onAmmoChanged?.Invoke(ammo);
        return true;
    }
}
