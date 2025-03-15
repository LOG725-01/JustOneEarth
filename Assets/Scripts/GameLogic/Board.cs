using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase forestTile, mountainTile, lakeTile, plainTile, desertTile;
    public Tilemap tilemapDetails; // Tilemap pour les détails (arbres, montagnes)
    public TileBase forestOverlayTile;
    public TileBase mountainOverlayTile;

    private TileType[,] grid;
    public int radius = 4;

    private GenerationConfig config;

    private void Start()
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
    grid = new TileType[radius * 2 + 1, radius * 2 + 1];

    Dictionary<TileType, float> probabilities = new Dictionary<TileType, float>
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
            TileType selectedType;
            do
            {
                selectedType = GetRandomTileType(probabilities);
            } while (!ValidateTilePlacement(selectedType, q, r));

            grid[q + radius, r + radius] = selectedType;

            // Conversion des coordonnées axiales en coordonnées monde
            Vector3 worldPos = AxialToIsometric(q, r);
            Vector3Int tilePosition = tilemap.WorldToCell(worldPos);

            tilemap.SetTile(tilePosition, GetTile(selectedType));
                Debug.Log($"Ajout de {selectedType} à {tilePosition}");
                Debug.Log($"Selected type forest ? {selectedType == TileType.Forests}");
                // Ajout des détails (arbres, montagnes...)
            if (selectedType == TileType.Forests)
            {
                tilemapDetails.SetTile(tilePosition, forestOverlayTile);
            }
            else if (selectedType == TileType.Mountains)
            {
                tilemapDetails.SetTile(tilePosition, mountainOverlayTile);
            }
            }
    }
}

    private Vector3 AxialToIsometric(int q, int r)
    {
        float hexWidth = 78f / 256f;  // Largeur de l'hexagone en Unity Units
        float hexHeight = (79f * Mathf.Sqrt(3) / 2) / 256f; // Hauteur correcte

        float x = hexWidth * q;
        float y = hexHeight * r * 0.75f;  // Facteur 0.75 pour ajuster l'espace vertical

        // Décalage des lignes impaires
        if (r % 2 != 0)
        {
            x += hexWidth / 2;
        }

        return new Vector3(x, y, 0);
    }




    private TileBase GetTile(TileType type)
    {
        switch (type)
        {
            case TileType.Forests: return forestTile;
            case TileType.Mountains: return mountainTile;
            case TileType.Lakes: return lakeTile;
            case TileType.Plains: return plainTile;
            case TileType.Deserts: return desertTile;
            default: return null;
        }
    }


    private TileType GetRandomTileType(Dictionary<TileType, float> probabilities)
    {
        float roll = UnityEngine.Random.value;
        float cumulative = 0f;

        foreach (var entry in probabilities)
        {
            cumulative += entry.Value;
            if (roll <= cumulative)
                return entry.Key;
        }

        return TileType.Plains;
    }

    private bool ValidateTilePlacement(TileType type, int q, int r)
    {
        if (type == TileType.Deserts && config.preventLargeDeserts)
        {
            int desertCount = 0;
            int totalNeighbors = 0;

            for (int dq = -1; dq <= 1; dq++)
            {
                for (int dr = -1; dr <= 1; dr++)
                {
                    int neighborQ = q + dq;
                    int neighborR = r + dr;

                    if (neighborQ >= -radius && neighborQ <= radius && neighborR >= -radius && neighborR <= radius)
                    {
                        totalNeighbors++;
                        if (grid[neighborQ + radius, neighborR + radius] == TileType.Deserts)
                            desertCount++;
                    }
                }
            }

            return (desertCount / (float)totalNeighbors) < config.maxDesertClusterRatio;
        }

        return true;
    }
}
