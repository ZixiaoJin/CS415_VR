using System.Collections.Generic;
using UnityEngine;

public class TunnelSpawner : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] List<TunnelSegment> segmentPrefabs = new();
    [SerializeField, Min(1)] int visibleCount = 6;
    [SerializeField] Transform startAnchor;
    [SerializeField] Transform player;

    [Header("Prefabs")]
    [SerializeField] GameObject healthPackPrefab;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject ammoPackPrefab;

    [Header("Per-Segment Spawn Counts")]
    [SerializeField, Min(0)] int healthPacksPerSegment = 1;
    [SerializeField, Min(0)] int enemiesPerSegment = 1;
    [SerializeField, Min(0)] int ammoPacksPerSegment = 1;

    const float kItemRadius = 1.5f;
    const float kEpsilon = 0.01f;

    readonly Queue<TunnelSegment> active = new();
    Transform nextAnchor;

    void Awake()
    {
        if (!startAnchor) startAnchor = transform;
        nextAnchor = startAnchor;
    }

    void Start()
    {
        if (segmentPrefabs.Count == 0)
        {
            Debug.LogError("Assign at least one tunnel prefab.");
            enabled = false;
            return;
        }
        for (int i = 0; i < 4; i++) SpawnNext();
    }

    void SpawnNext()
    {
        var prefab = segmentPrefabs[Random.Range(0, segmentPrefabs.Count)];
        var seg = Instantiate(prefab, nextAnchor.position, nextAnchor.rotation, transform);

        if (seg.triggerZone)
        {
            var tt = seg.triggerZone.GetComponent<TunnelTrigger>()
                     ?? seg.triggerZone.gameObject.AddComponent<TunnelTrigger>();
        }

        if (seg.spawnVolume)
        {
            var placed = new List<Vector3>();

            // Health packs
            for (int i = 0; i < healthPacksPerSegment; i++)
            {
                if (!healthPackPrefab) break;
                if (TrySampleNonOverlapping(seg.spawnVolume, placed, out var p))
                {
                    placed.Add(p);
                    Instantiate(healthPackPrefab, p, Quaternion.identity, seg.transform);
                }
            }

            // Enemies
            for (int i = 0; i < enemiesPerSegment; i++)
            {
                if (!enemyPrefab) break;
                if (TrySampleNonOverlapping(seg.spawnVolume, placed, out var p))
                {
                    placed.Add(p);
                    Instantiate(enemyPrefab, p, Quaternion.identity, seg.transform);
                }
            }

            // Ammo packs
            for (int i = 0; i < ammoPacksPerSegment; i++)
            {
                if (!ammoPackPrefab) break;
                if (TrySampleNonOverlapping(seg.spawnVolume, placed, out var p))
                {
                    placed.Add(p);
                    Instantiate(ammoPackPrefab, p, Quaternion.identity, seg.transform);
                }
            }
        }

        active.Enqueue(seg);
        nextAnchor = seg.attachPoint ? seg.attachPoint : seg.transform;
    }

    public void OnAnyTriggerEntered(TunnelTrigger trigger)
    {
        if (active.Count == 0) return;

        if (active.Count == visibleCount)
        {
            var first = active.Dequeue();
            Destroy(first.gameObject);
        }

        SpawnNext();
    }

    static Vector3 RandomPointInBox(BoxCollider box)
    {
        var t = box.transform;
        var center = t.TransformPoint(box.center);
        var size = Vector3.Scale(box.size, t.lossyScale);

        var local = new Vector3(
            Random.Range(-size.x * 0.5f, size.x * 0.5f),
            Random.Range(-size.y * 0.5f, size.y * 0.5f),
            Random.Range(-size.z * 0.5f, size.z * 0.5f)
        );
        return center + t.rotation * local;
    }

    static bool IsFarEnough(Vector3 candidate, List<Vector3> placed, float minDist)
    {
        foreach (var p in placed)
            if ((candidate - p).sqrMagnitude < minDist * minDist) return false;
        return true;
    }

    static bool TrySampleNonOverlapping(BoxCollider volume, List<Vector3> placed, out Vector3 pos)
    {
        float minDist = 2f * kItemRadius + kEpsilon;

        for (int tries = 0; tries < 8; tries++)
        {
            var candidate = RandomPointInBox(volume);
            if (IsFarEnough(candidate, placed, minDist))
            {
                pos = candidate;
                return true;
            }
        }
        pos = default;
        return false;
    }
}
