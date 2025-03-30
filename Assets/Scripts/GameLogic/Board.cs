using System;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject forestPrefab;
    public GameObject mountainPrefab;
    public GameObject lakePrefab;
    public GameObject plainPrefab;
    public GameObject desertPrefab;
    public GameObject[] animalPrefabs;

    public event Action OnBoardGenerated;

    private TileType[,] grid;
    public int radius = 5;

    private GenerationConfig config;
    Dictionary<TileType, float> probabilities;

    private void Awake()
    {
        //Debug.Log("[Board] Chargement de la configuration...");
        config = GenerationConfig.LoadFromJson();
        if (config == null)
        {
            Debug.LogError("[Board] Échec du chargement de la configuration !");
            return;
        }

        //Debug.Log("[Board] Configuration chargée avec succès.");
    }

    private void Start()
    {
        CreateBoard();
    }
    private void PlaceAnimals()
    {
        bool debug = true;
        foreach (Transform tile in transform) // Parcourt tous les enfants du Board
        {
            Tile tileScript = tile.GetComponent<Tile>();
            if (tileScript != null && tileScript.tileType == TileType.Plains)
            {
                // 50% de chance d'avoir un animal
                if (UnityEngine.Random.value < 0.5f)
                {
                    Vector3 spawnPos = tile.position + Vector3.up * 0.2f; // Décalage vertical
                    GameObject animal = Instantiate(
                        animalPrefabs[UnityEngine.Random.Range(0, animalPrefabs.Length)], 
                        spawnPos, Quaternion.identity, tile);
                    if(debug) animal.GetComponent<AnimalMouvement>().SetDebug();
                    debug = false;
                }
            }
        }
    }
    public void CreateBoard()
    {
        //Debug.Log("[Board] Début de la génération du plateau...");
        grid = new TileType[radius * 2 + 1, radius * 2 + 1];

        probabilities = new Dictionary<TileType, float>
        {
            { TileType.Forests, config.forestProbability / 100f },
            { TileType.Mountains, config.mountainProbability / 100f },
            { TileType.Lakes, config.lakeProbability / 100f },
            { TileType.Plains, config.plainProbability / 100f },
            { TileType.Deserts, config.desertProbability / 100f }
        };

        //Debug.Log("[Board] Probabilités assignées :");
        foreach (var pair in probabilities)
            //Debug.Log($"  - {pair.Key}: {pair.Value * 100f}%");

        for (int q = -radius; q <= radius; q++)
        {
            int r1 = Mathf.Max(-radius, -q - radius);
            int r2 = Mathf.Min(radius, -q + radius);

            for (int r = r1; r <= r2; r++)
            {
                float noise = Mathf.PerlinNoise(q * 0.1f, r * 0.1f);
                //Debug.Log($"[Board] Perlin Noise pour ({q},{r}) = {noise}");

                if (!IsTileGenerated(q, r))
                {
                    grid[q + radius, r + radius] = TileType.None;
                    //Debug.Log($"[Board] Tuile ignorée (noise trop faible) à ({q},{r})");
                    continue;
                }

                TileType selectedType = GetRandomTileType(probabilities);
                grid[q + radius, r + radius] = selectedType;
                //Debug.Log($"[Board] Tuile générée à ({q},{r}) : {selectedType}");
            }
        }

        //Debug.Log("[Board] Correction des lacs isolés...");
        FixLonelyLakes();

        //Debug.Log("[Board] Instanciation des préfabriqués...");
        for (int q = -radius; q <= radius; q++)
        {
            int r1 = Mathf.Max(-radius, -q - radius);
            int r2 = Mathf.Min(radius, -q + radius);

            for (int r = r1; r <= r2; r++)
            {
                TileType type = grid[q + radius, r + radius];

                if (type == TileType.None)
                    continue;

                Vector3 worldPos = AxialToIsometric(q, r);
                GameObject prefab = GetTilePrefab(type);

                if (prefab == null)
                {
                    Debug.LogWarning($"[Board] Aucun prefab trouvé pour le type {type} à ({q},{r}) !");
                    continue;
                }

                GameObject tileObj = Instantiate(prefab, worldPos, Quaternion.identity, transform);

                Tile tileComponent = tileObj.GetComponent<Tile>();
                if (tileComponent != null)
                {
                    tileComponent.Initialize(type); // ⬅️ Ceci assigne le TileType ET génère les ressources
                    //Debug.Log($"[Board] Tuile initialisée avec type {type} et ressources : " + string.Join(", ", tileComponent.producedRessources));
                }
                else
                {
                    Debug.LogWarning($"[Board] Le prefab {prefab.name} n'a pas de composant Tile !");
                }

                //Debug.Log($"[Board] Instancié {type} à {worldPos}");
            }
        }

        //Debug.Log("[Board] Génération du plateau terminée !");
        LogAllTiles();
        OnBoardGenerated?.Invoke();
        PlaceAnimals();
    }

    private bool IsTileGenerated(int q, int r)
    {
        float noiseValue = Mathf.PerlinNoise(q * 0.1f, r * 0.1f);
        bool generated = noiseValue > 0.4335f;

        //Debug.Log($"[Board] PerlinNoise ({q}, {r}) = {noiseValue:F4} → {(generated ? "Générée" : "Ignorée")}");

        return generated;
    }


    private void FixLonelyLakes()
    {
        for (int q = -radius; q <= radius; q++)
        {
            for (int r = -radius; r <= radius; r++)
            {
                if (!IsInsideMap(q, r)) continue;

                if (grid[q + radius, r + radius] == TileType.Lakes && CountNeighbors(q, r) != 6)
                {
                    //Debug.Log($"[Board] Lac isolé détecté à ({q},{r}) → remplacement...");
                    grid[q + radius, r + radius] = GetRandomTileType(probabilities, TileType.Lakes);
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
        //Debug.Log($"[Board] → Comptage des voisins pour la tuile ({q}, {r})");

        foreach (var dir in directions)
        {
            int neighborQ = q + dir.x;
            int neighborR = r + dir.y;

            if (IsInsideMap(neighborQ, neighborR))
            {
                TileType neighborType = grid[neighborQ + radius, neighborR + radius];

                if (neighborType != TileType.None)
                {
                    count++;
                    //Debug.Log($"  ✓ Voisin à ({neighborQ}, {neighborR}) : {neighborType}");
                }
                else
                {
                    //Debug.Log($"  ✗ Voisin à ({neighborQ}, {neighborR}) : vide");
                }
            }
            else
            {
                //Debug.Log($"  ⛔ Hors de la carte : ({neighborQ}, {neighborR})");
            }
        }

        //Debug.Log($"[Board] Nombre total de voisins valides : {count}");
        return count;
    }
    private bool IsInsideMap(int q, int r)
    {
        bool inside = Math.Abs(q) <= radius && Math.Abs(r) <= radius && Math.Abs(-q - r) <= radius;

        //Debug.Log($"[Board] Vérification IsInsideMap({q}, {r}) → {(inside ? "✔️ À l’intérieur" : "❌ Hors limites")}");

        return inside;
    }


    private GameObject GetTilePrefab(TileType type)
    {
        switch (type)
        {
            case TileType.Forests:
                //Debug.Log("[Board] Préfabriqué demandé pour : Forests");
                return forestPrefab;

            case TileType.Mountains:
                //Debug.Log("[Board] Préfabriqué demandé pour : Mountains");
                return mountainPrefab;

            case TileType.Lakes:
                //Debug.Log("[Board] Préfabriqué demandé pour : Lakes");
                return lakePrefab;

            case TileType.Plains:
                //Debug.Log("[Board] Préfabriqué demandé pour : Plains");
                return plainPrefab;

            case TileType.Deserts:
                //Debug.Log("[Board] Préfabriqué demandé pour : Deserts");
                return desertPrefab;

            default:
                //Debug.LogWarning($"[Board] Aucun prefab défini pour le type : {type}");
                return null;
        }
    }

    private Vector3 AxialToIsometric(int q, int r)
    {
        float hexWidth = 1f;
        float hexHeight = hexWidth * Mathf.Sqrt(3) / 2;

        float x = hexWidth * (q + r * 0.5f);
        float z = hexHeight * r;

        Vector3 position = new Vector3(x, 0, z);
        //Debug.Log($"[Board] Conversion AxialToIsometric → ({q}, {r}) → WorldPos : {position}");

        return position;
    }


    private TileType GetRandomTileType(Dictionary<TileType, float> probabilities, TileType? excludeType = null)
    {
        float roll = UnityEngine.Random.value;
        float cumulative = 0f;

        //Debug.Log($"[Board] Sélection aléatoire d'une tuile... Roll = {roll:F4}" + (excludeType.HasValue ? $" (exclusion : {excludeType.Value})" : ""));

        foreach (var entry in probabilities)
        {
            if (excludeType.HasValue && entry.Key == excludeType.Value)
            {
                //Debug.Log($"[Board] → Ignoré : {entry.Key} (exclu)");
                continue;
            }

            cumulative += entry.Value;
            //Debug.Log($"[Board] → Cumul {entry.Key} : {cumulative:F4}");

            if (roll <= cumulative)
            {
                //Debug.Log($"[Board] ✅ Tuile sélectionnée : {entry.Key}");
                return entry.Key;
            }
        }

        Debug.LogWarning("[Board] Aucune tuile sélectionnée par probabilités, retour par défaut : Plains");
        return TileType.Plains;
    }

    public void LogAllTiles()
    {
        Tile[] tiles = FindObjectsOfType<Tile>();
        //Debug.Log($"[Board] --- Résumé des tuiles générées ({tiles.Length}) ---");

        foreach (Tile tile in tiles)
        {
            string resSummary = tile.producedRessources.Count > 0
                ? string.Join(", ", tile.producedRessources)
                : "Aucune";

            //Debug.Log($"→ {tile.name} | Type: {tile.tileType} | Ressources: {resSummary}");
        }
    }

    public List<Tile> GetAllTiles()
    {
        return new List<Tile>(FindObjectsOfType<Tile>());
    }

    public void InitializePlayerResources(Player player)
    {
        Dictionary<RessourceTypes, int> totalResources = new();

        // Parcours toutes les tuiles générées
        foreach (TileType type in Enum.GetValues(typeof(TileType)))
        {
            totalResources[(RessourceTypes)type] = 0;
        }

        foreach (Tile tile in FindObjectsOfType<Tile>())
        {
            foreach (var kvp in tile.producedRessources)
            {
                if (!totalResources.ContainsKey(kvp.Key))
                    totalResources[kvp.Key] = 0;

                totalResources[kvp.Key] += kvp.Value;
            }
        }

        // Initialise les ressources du joueur à 10 % du total
        foreach (var kvp in totalResources)
        {
            int initialAmount = Mathf.FloorToInt(kvp.Value * 0.1f);
            player.currentRessources[kvp.Key] = initialAmount;

            //Debug.Log($"[Board] Ressource initiale pour {kvp.Key} : {initialAmount} (10% de {kvp.Value})");
        }
    }
}
