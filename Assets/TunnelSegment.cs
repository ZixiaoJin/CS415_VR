using UnityEngine;

public class TunnelSegment : MonoBehaviour
{
    [Header("Anchors")]
    public Transform attachPoint;      // where the next segment should be placed
    public Collider triggerZone;      // the trigger you pass through

    [Header("Pickups")]
    public BoxCollider spawnVolume;
}
