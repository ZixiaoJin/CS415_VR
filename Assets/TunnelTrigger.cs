using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TunnelTrigger : MonoBehaviour
{
    [SerializeField] TunnelSpawner spawner;
    [Tooltip("Reference to the wall for this segment. If null, no pass-check is done.")]
    [SerializeField] WallHitTrigger wall;     // <-- link matching wall in this segment

    bool fired;

    void Awake()
    {
        if (!spawner) spawner = FindFirstObjectByType<TunnelSpawner>();
    }

    void OnEnable()
    {
        fired = false;                         // reset for pooled segments
    }

    void OnTriggerEnter(Collider other)
    {
        if (fired || !spawner) return;

        // Is it the player?
        var cc = other.GetComponentInParent<CharacterController>();
        if (!cc) return;

        fired = true;

        // If a wall is linked and it was NOT hit, award a point.
        var score = other.GetComponentInParent<PlayerScore>();
        if (score && (!wall || wall.WasHit == false))
        {
            score.Add(1);
            // Debug.Log("[TunnelTrigger] Clean pass! +1 score");
        }

        spawner.OnAnyTriggerEntered(this);
    }
}
