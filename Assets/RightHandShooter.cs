using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class RightHandShooter : MonoBehaviour
{
    [Header("Setup")]
    public Transform muzzle;
    public GameObject projectilePrefab;
    public float muzzleSpeed = 22f;

    [Header("Input")]
    public InputActionProperty triggerAction;

    [Header("Refs")]
    public PlayerAmmo playerAmmo;

    void Awake()
    {
        if (!playerAmmo) playerAmmo = GetComponentInParent<PlayerAmmo>();
    }

    void OnEnable() => triggerAction.action?.Enable();
    void OnDisable() => triggerAction.action?.Disable();

    void Update()
    {
        if (Time.timeScale == 0f) return;
        if (triggerAction.action == null || muzzle == null || projectilePrefab == null) return;
        if (!triggerAction.action.WasPressedThisFrame()) return;

        // consume 1 round; stop if no ammo
        if (playerAmmo && !playerAmmo.TryConsume(1)) return;

        // spawn
        var proj = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);

        // give it velocity
        var rb = proj.GetComponentInChildren<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.linearVelocity = muzzle.forward * muzzleSpeed;
        }

        // ignore collisions with ALL colliders under the player rig
        var projCols = proj.GetComponentsInChildren<Collider>();
        var playerRoot = playerAmmo ? playerAmmo.transform.root : transform.root;
        var playerCols = playerRoot.GetComponentsInChildren<Collider>();
        foreach (var pc in projCols)
            foreach (var pl in playerCols)
                Physics.IgnoreCollision(pc, pl, true);
    }
}
