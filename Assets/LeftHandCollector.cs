using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class LeftHandCollector : MonoBehaviour
{
    [Header("Input (Grip/Grab)")]
    public InputActionProperty grabAction;

    [Header("Pickup Search")]
    public float collectRadius = 0.35f;
    public LayerMask pickupMask;
    public bool requireLineOfSight = false;

    [Header("Debug")]
    public bool drawGizmo = true;

    PlayerHealth playerHealth;
    PlayerAmmo playerAmmo;

    void Awake()
    {
        playerHealth = GetComponentInParent<PlayerHealth>() ?? FindFirstObjectByType<PlayerHealth>();
        playerAmmo = GetComponentInParent<PlayerAmmo>() ?? FindFirstObjectByType<PlayerAmmo>();
    }

    void OnEnable() => grabAction.action?.Enable();
    void OnDisable() => grabAction.action?.Disable();

    void Update()
    {
        if (grabAction.action == null || !grabAction.action.WasPressedThisFrame()) return;

        // Find nearby pickups (health or ammo)
        var hits = Physics.OverlapSphere(transform.position, collectRadius, pickupMask, QueryTriggerInteraction.Collide);
        if (hits == null || hits.Length == 0) return;

        // nearest first
        var nearest = hits
            .Select(h => new { col = h, sq = (h.transform.position - transform.position).sqrMagnitude })
            .OrderBy(p => p.sq)
            .Select(p => p.col)
            .ToList();

        foreach (var col in nearest)
        {
            // Try HealthPack
            var hp = col.GetComponentInParent<HealthPack>() ?? col.GetComponent<HealthPack>();
            if (hp)
            {
                if (requireLineOfSight && !LOSOk(hp.transform)) continue;
                hp.Collect(playerHealth);
                return;
            }

            // Try AmmoPack
            var ap = col.GetComponentInParent<AmmoPack>() ?? col.GetComponent<AmmoPack>();
            if (ap)
            {
                if (requireLineOfSight && !LOSOk(ap.transform)) continue;
                ap.Collect(playerAmmo);
                return;
            }
        }
    }

    bool LOSOk(Transform target)
    {
        if (!target) return false;
        var origin = transform.position;
        var dir = target.position - origin;
        if (Physics.Raycast(origin, dir.normalized, out var hit, dir.magnitude + 0.01f, ~0, QueryTriggerInteraction.Ignore))
        {
            if (!hit.collider.GetComponentInParent<HealthPack>() && !hit.collider.GetComponentInParent<AmmoPack>())
                return false;
        }
        return true;
    }

    void OnDrawGizmosSelected()
    {
        if (!drawGizmo) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, collectRadius);
    }
}
