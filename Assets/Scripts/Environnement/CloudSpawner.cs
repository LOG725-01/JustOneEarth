using UnityEngine;
using System.Collections.Generic;

public class CloudSpawner : MonoBehaviour
{
    public GameObject cloudPrefab;
    public int cloudCount = 2;
    public float minHeight = 1f, maxHeight = 2f;

    private float minX, maxX, minZ, maxZ;
    private Board board;
    private List<Vector3> spawnedClouds = new List<Vector3>();

    public float minScale = 0.8f, maxScale = 1.5f;
    public float minDistance = 1f;

    public bool debug = false;
    public void Initialize(Board board)
    {
        this.board = board;
        if (board != null)
        {
            float boardSize = board.radius * 2;
            minX = -boardSize / 2f;
            maxX = boardSize / 2f;
            minZ = -boardSize / 2f;
            maxZ = boardSize / 2f;

            if (debug) Debug.Log("[CloudSpawner] Board initialisé pour la génération des nuages.");
            SpawnClouds();
        }
        else
        {
            Debug.LogError("[CloudSpawner] Board introuvable !");
        }
    }
    public float GetMinX() { return minX; }
    public float GetMaxX() { return maxX; }
    public float GetMinZ() { return minZ; }
    public float GetMaxZ() { return maxZ; }
    void SpawnClouds()
    {
        int attempts = 0;
        for (int i = 0; i < cloudCount * board.radius; i++)
        {
            Vector3 position;
            bool validPosition = false;

            while (!validPosition && attempts < 100)
            {
                attempts++;
                position = new Vector3(
                    Random.Range(minX, maxX),
                    Random.Range(minHeight, maxHeight),
                    Random.Range(minZ, maxZ)
                );

                if (IsPositionValid(position))
                {
                    validPosition = true;
                    spawnedClouds.Add(position);

                    float rotationY = 0f;
                    Quaternion rotation = Quaternion.Euler(0, rotationY, 0);

                    float randomScale = Random.Range(minScale, maxScale);

                    GameObject newCloud = Instantiate(cloudPrefab, position, rotation);
                    newCloud.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
                    newCloud.transform.parent = transform;
                }
            }
        }

        if (debug) Debug.Log("[CloudSpawner] Nuages générés.");
    }

    bool IsPositionValid(Vector3 newPos)
    {
        foreach (Vector3 existingPos in spawnedClouds)
        {
            if (Vector3.Distance(newPos, existingPos) < minDistance * board.radius)
            {
                return false;
            }
        }
        return true;
    }
}
