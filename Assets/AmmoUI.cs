using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
public class AmmoUI : MonoBehaviour
{
    [Header("Refs")]
    public PlayerAmmo playerAmmo;
    public TMP_Text ammoText;

    void Reset()
    {
        ammoText = GetComponent<TMP_Text>();
        if (!playerAmmo) playerAmmo = FindFirstObjectByType<PlayerAmmo>();
    }

    void OnEnable()
    {
        if (!playerAmmo) playerAmmo = FindFirstObjectByType<PlayerAmmo>();
        if (!ammoText) ammoText = GetComponent<TMP_Text>();

        if (playerAmmo != null)
            playerAmmo.onAmmoChanged.AddListener(UpdateText);

        UpdateText(playerAmmo ? playerAmmo.ammo : 0);
    }

    void OnDisable()
    {
        if (playerAmmo != null)
            playerAmmo.onAmmoChanged.RemoveListener(UpdateText);
    }

    void UpdateText(int current)
    {
        if (!ammoText) return;
        ammoText.text = current.ToString();
    }
}
