using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public List<GameObject> objectsToSpawn;
    public List<Transform> spawnPoints;
    public float initialSpawnInterval = 2f;
    public float minSpawnInterval = 0.5f;
    public float spawnIntervalDecayRate = 0.1f;
    private float currentSpawnInterval;
    private float nextSpawnTime;

    void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        nextSpawnTime = Time.time + currentSpawnInterval;
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnObject();
            UpdateSpawnInterval();
            nextSpawnTime = Time.time + currentSpawnInterval;
        }
    }

    void SpawnObject()
    {
        if (objectsToSpawn.Count == 0 || spawnPoints.Count == 0) return;
        GameObject objectToSpawn = objectsToSpawn[Random.Range(0, objectsToSpawn.Count)];
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        Instantiate(objectToSpawn, spawnPoint.position, Quaternion.identity);
    }

    void UpdateSpawnInterval()
    {
        currentSpawnInterval = Mathf.Max(currentSpawnInterval - spawnIntervalDecayRate, minSpawnInterval);
    }
}
