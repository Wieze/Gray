using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints;       // Possible spawn locations

    [Header("Anomaly Prefab")]
    [SerializeField] private AChase aChasePrefab;           // Prefab for the chasing anomaly

    [Header("Stability Reference")]
    [SerializeField] private StabilityManager stabilityManager; // Reference to StabilityManager

    // Object pool for AChase instances
    private IObjectPool<AChase> aChasePool;

    void Awake()
    {
        // Initialize the object pool
        aChasePool = new ObjectPool<AChase>(
            CreateAChase,       // Create new instance when pool is empty
            OnGetAChase,        // Called when taking from pool
            OnReleaseAChase,    // Called when returning to pool
            maxSize: 10         // Adjust as needed
        );
    }

    void Start()
    {
        // Subscribe to the stability cycle event
        if (stabilityManager != null)
        {
            stabilityManager.OnStabilityCycleCompleted += SpawnAnomaly;
        }
        else
        {
            Debug.LogError("StabilityManager reference not set in Spawner!");
        }
    }

    void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (stabilityManager != null)
        {
            stabilityManager.OnStabilityCycleCompleted -= SpawnAnomaly;
        }
    }

    // Creates a brand new anomaly instance (only called when pool is empty)
    private AChase CreateAChase()
    {
        AChase anomaly = Instantiate(aChasePrefab);
        // Optionally, you can pass the pool reference to the anomaly for self‑release
        // anomaly.SetPool(aChasePool);
        return anomaly;
    }

    // Called when an anomaly is taken from the pool (spawned)
    private void OnGetAChase(AChase anomaly)
    {
        anomaly.gameObject.SetActive(true);

        // Place at a random spawn point
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform randomSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
            anomaly.transform.position = randomSpawn.position;
        }

        // If the anomaly needs to do anything when spawned (e.g., start chasing), call it here
        // anomaly.OnSpawned();
    }

    // Called when an anomaly is returned to the pool (despawned)
    private void OnReleaseAChase(AChase anomaly)
    {
        anomaly.gameObject.SetActive(false);
        // If the anomaly needs to clean up, call a method here
        // anomaly.OnDespawned();
    }

    // Event handler – spawns an anomaly when stability reaches 100%
    private void SpawnAnomaly()
    {
        aChasePool.Get();
    }

    // Optional: Public method to manually release an anomaly back to the pool
    public void ReleaseAChase(AChase anomaly)
    {
        aChasePool.Release(anomaly);
    }
}