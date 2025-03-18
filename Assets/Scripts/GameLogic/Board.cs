using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject forestPrefab;
    public GameObject mountainPrefab;
    public GameObject lakePrefab;
    public GameObject plainPrefab;
    public GameObject desertPrefab;

    private TileType[,] grid;
    public int radius = 5;

    private GenerationConfig config;
    Dictionary<TileType, float> probabilities;

    private void Awake()
    {
        config = GenerationConfig.LoadFromJson();
        if (config == null)
        {
            Debug.LogError("Échec du chargement de la configuration");
            return;
        }

        CreateBoard();
    }

    public void CreateBoard()
    {
        grid = new TileType[radius * 2 + 1, radius * 2 + 1]; // Plus besoin du nullable

        probabilities = new Dictionary<TileType, float>
    {
        { TileType.Forests, config.forestProbability / 100f },
        { TileType.Mountains, config.mountainProbability / 100f },
        { TileType.Lakes, config.lakeProbability / 100f },
        { TileType.Plains, config.plainProbability / 100f },
        { TileType.Deserts, config.desertProbability / 100f }
    };

        for (int q = -radius; q <= radius; q++)
        {
            int r1 = Mathf.Max(-radius, -q - radius);
            int r2 = Mathf.Min(radius, -q + radius);

            for (int r = r1; r <= r2; r++)
            {
                //Applique un bruit de Perlin
                if (!IsTileGenerated(q, r))
                {
                    grid[q + radius, r + radius] = TileType.None;
                    continue;
                }

                TileType selectedType = GetRandomTileType(probabilities);
                grid[q + radius, r + radius] = selectedType;
            }
        }
        FixLonelyLakes();     //Remplace les lacs aux bords de la carte
        for (int q = -radius; q <= radius; q++)
        {
            int r1 = Mathf.Max(-radius, -q - radius);
            int r2 = Mathf.Min(radius, -q + radius);

            for (int r = r1; r <= r2; r++)
            {
                if (grid[q + radius, r + radius] == TileType.None)
                    continue;

                Vector3 worldPos = AxialToIsometric(q, r);
                Instantiate(GetTilePrefab(grid[q + radius, r + radius]), worldPos, Quaternion.identity);
            }
        }
    }
    
    private bool IsTileGenerated(int q, int r)
    {
        float noiseValue = Mathf.PerlinNoise(q * 0.1f, r * 0.1f);
        return noiseValue > 0.4335f;
    }

    private void FixLonelyLakes()
    {
        for (int q = -radius; q <= radius; q++)
        {
            for (int r = -radius; r <= radius; r++)
            {
                if (!IsInsideMap(q, r)) continue;

                if (grid[q + radius, r + radius] == TileType.Lakes && CountNeighbors(q, r) !=6)
                {
                    grid[q + radius, r + radius] = GetRandomTileType(probabilities,TileType.Lakes);
                }
            }
        }
    }
    private int CountNeighbors(int q, int r)
    {
        List<Vector2Int> directions = new List<Vector2Int>
    {
        new Vector2Int(1, 0), new Vector2Int(-1, 0),  // Droite, Gauche
        new Vector2Int(0, 1), new Vector2Int(0, -1),  // Haut, Bas
        new Vector2Int(1, -1), new Vector2Int(-1, 1)  // Diagonales
    };

        int count = 0;
        foreach (var dir in directions)
        {
            int neighborQ = q + dir.x;
            int neighborR = r + dir.y;

            if (IsInsideMap(neighborQ, neighborR))
            {
                if (grid[neighborQ + radius, neighborR + radius] != TileType.None)
                {
                    count++;
                }
            }
        }
        return count;
    }

    private bool IsInsideMap(int q, int r)
    {
        return Math.Abs(q) <= radius && Math.Abs(r) <= radius && Math.Abs(-q - r) <= radius;
    }

    private GameObject GetTilePrefab(TileType type)
    {
        switch (type)
        {
            case TileType.Forests: return forestPrefab;
            case TileType.Mountains: return mountainPrefab;
            case TileType.Lakes: return lakePrefab;
            case TileType.Plains: return plainPrefab;
            case TileType.Deserts: return desertPrefab;
            default: return null;
        }
    }
    private Vector3 AxialToIsometric(int q, int r)
    {
        float hexWidth = 1f;
        float hexHeight = hexWidth * Mathf.Sqrt(3) / 2;

        float x = hexWidth * (q + r * 0.5f);
        float z = hexHeight * r;

        return new Vector3(x, 0, z);
    }

    private TileType GetRandomTileType(Dictionary<TileType, float> probabilities, TileType? excludeType = null)
    {
        float roll = UnityEngine.Random.value;
        float cumulative = 0f;

        foreach (var entry in probabilities)
        {
            if (excludeType.HasValue && entry.Key == excludeType.Value)
                continue;

            cumulative += entry.Value;
            if (roll <= cumulative)
                return entry.Key;
        }

        return TileType.Plains;
    }
}
