using UnityEngine;
using System.Collections.Generic;

public class CloudSpawner : MonoBehaviour
{
    public GameObject cloudPrefab;
    public int cloudCount = 2;
    public float minHeight = 1f, maxHeight = 2f;

    private float minX, maxX, minZ, maxZ;
    private Board board;
    private List<Vector3> spawnedClouds = new List<Vector3>(); // Stocke les positions des nuages existants

    public float minScale = 0.8f, maxScale = 1.5f; // Taille aléatoire
    public float minDistance = 1f; // Distance minimale entre deux nuages

    void Start()
    {
        board = FindObjectOfType<Board>();

        if (board != null)
        {
            float boardSize = board.radius * 2;
            minX = -boardSize / 2f;
            maxX = boardSize / 2f;
            minZ = -boardSize / 2f;
            maxZ = boardSize / 2f;
        }
        else
        {
            Debug.LogError("Board introuvable dans la scène !");
            return;
        }

        SpawnClouds();
    }
    public float GetMinX() { return minX; }
    public float GetMaxX() { return maxX; }
    public float GetMinZ() { return minZ; }
    public float GetMaxZ() { return maxZ; }
    void SpawnClouds()
    {
        int attempts = 0; // Évite une boucle infinie
        for (int i = 0; i < cloudCount*board.radius; i++)
        {
            Vector3 position;
            bool validPosition = false;

            // Tente de trouver un emplacement valide
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
                    spawnedClouds.Add(position); // Ajoute la position à la liste des nuages existants

                    float rotationY =  0f;
                    Quaternion rotation = Quaternion.Euler(0, rotationY, 0);

                    float randomScale = Random.Range(minScale, maxScale);

                    GameObject newCloud = Instantiate(cloudPrefab, position, rotation);
                    newCloud.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
                    newCloud.transform.parent = transform;
                }
            }
        }
    }

    // Vérifie si un nouveau nuage est trop proche des autres
    bool IsPositionValid(Vector3 newPos)
    {
        foreach (Vector3 existingPos in spawnedClouds)
        {
            if (Vector3.Distance(newPos, existingPos) < minDistance*board.radius)
            {
                return false; // Trop proche d'un autre nuage
            }
        }
        return true; // La position est valide
    }
}
