using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints;       // Possible spawn locations
    [SerializeField] private float timeBetweenSpawns = 5f;  // Time between spawns
    private float timeSinceLastSpawn;                        // Fixed typo

    [Header("Anomaly Prefab")]
    [SerializeField] private AChase aChasePrefab;           // Prefab for the chasing anomaly

    // Object pool for AChase instances
    private IObjectPool<AChase> aChasePool;

    void Awake()
    {
        // Initialize the object pool with creation, get, and release actions
        aChasePool = new ObjectPool<AChase>(
            CreateAChase,       // Method to create a new anomaly
            OnGetAChase,        // Method called when taking from pool
            OnReleaseAChase,    // Method called when returning to pool
            maxSize: 10         // Optional: limit the pool size (adjust as needed)
        );
    }

    // Creates a brand new anomaly instance (only called when pool is empty)
    private AChase CreateAChase()
    {
        AChase anomaly = Instantiate(aChasePrefab);
        // Optionally, set the pool reference on the anomaly so it can release itself
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

    void Update()
    {
        // Spawn a new anomaly when the timer reaches the interval
        if (Time.time > timeSinceLastSpawn)
        {
            // Get an instance from the pool (this triggers OnGetAChase)
            aChasePool.Get();
            timeSinceLastSpawn = Time.time + timeBetweenSpawns;
        }
    }

    // Optional: Public method to manually release an anomaly back to the pool
    public void ReleaseAChase(AChase anomaly)
    {
        aChasePool.Release(anomaly);
    }
}