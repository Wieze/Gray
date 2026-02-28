using UnityEngine;
using System.Collections;

public class AnomalyZone : MonoBehaviour
{
    public GameObject anomalyPrefab;
    public Transform[] spawnPoints;

    private GameObject currentAnomaly;
    private bool hasSpawned = false;  // Ensures we only spawn once

    void OnTriggerEnter(Collider other)
    {
        // Only spawn if the player enters and we haven't spawned yet
        if (other.CompareTag("Player") && !hasSpawned)
        {
            SpawnAnomaly();
            hasSpawned = true;
        }
    }

    // Removed OnTriggerExit – the anomaly now follows the player indefinitely

    void SpawnAnomaly()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned in AnomalyZone!");
            return;
        }

        int index = Random.Range(0, spawnPoints.Length);
        currentAnomaly = Instantiate(anomalyPrefab, spawnPoints[index].position, Quaternion.identity);
    }
}