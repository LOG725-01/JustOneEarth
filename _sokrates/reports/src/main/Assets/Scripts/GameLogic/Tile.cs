using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour, IClickable
{
    public Dictionary<RessourceTypes, int> producedRessources = new Dictionary<RessourceTypes, int>();
    public TileType tileType;
    public Player owner;
    [SerializeField] private GameObject borderRedPrefab;
    [SerializeField] private GameObject borderBluePrefab;
    [SerializeField] private GameObject ParticlePrefab;
    [SerializeField] private GameObject housePrefab;
    private GameObject currentStructure;

    private GameObject currentBorder;

    public bool debug = false;

    public void Initialize(TileType type, bool _debug = false)
    {
        tileType = type;
        debug = _debug;
        AssignResources();

        if (debug) Debug.Log($"[Tile] Initialisation : {gameObject.name} - Type : {tileType} - Ressources : " +
                  string.Join(", ", producedRessources));
    }
    public void UpdateBorder()
    {
        if (currentBorder != null)
            Destroy(currentBorder);

        if (owner == null)
            return;

        GameObject borderPrefab = owner is HumanPlayer ? borderBluePrefab : borderRedPrefab;

        currentBorder = Instantiate(borderPrefab, transform);
    }
    private void AssignResources()
    {
        producedRessources.Clear();
        if (debug) Debug.Log($"[Tile] Attribution des ressources pour la tuile de type {tileType}");

        switch (tileType)
        {
            case TileType.Forests:
                producedRessources[RessourceTypes.Trees] = Random.Range(1, 3);
                if (debug) Debug.Log($"[Tile] Ressource g�n�r�e : Trees => {producedRessources[RessourceTypes.Trees]}");
                break;
            case TileType.Mountains:
                producedRessources[RessourceTypes.Minerals] = Random.Range(1, 3);
                if (debug) Debug.Log($"[Tile] Ressource g�n�r�e : Minerals => {producedRessources[RessourceTypes.Minerals]}");
                break;
            case TileType.Lakes:
                producedRessources[RessourceTypes.Water] = Random.Range(1, 3);
                if (debug) Debug.Log($"[Tile] Ressource g�n�r�e : Water => {producedRessources[RessourceTypes.Water]}");
                break;
            case TileType.Plains:
                producedRessources[RessourceTypes.Sun] = Random.Range(1, 3);
                if (debug) Debug.Log($"[Tile] Ressource g�n�r�e : Sun => {producedRessources[RessourceTypes.Sun]}");
                break;
            case TileType.Deserts:
                producedRessources[RessourceTypes.Oil] = Random.Range(1, 3);
                if (debug) Debug.Log($"[Tile] Ressource g�n�r�e : Oil => {producedRessources[RessourceTypes.Oil]}");
                break;
            default:
                if (debug) Debug.LogWarning($"[Tile] Aucun type reconnu pour la tuile : {tileType}");
                break;
        }
    }
    public void OnClick(GameState gameState)
    {
        Player currentPlayer = gameState.currentInstancePlayer;
        if (currentPlayer.selectedTile == this)
        {
            ResetElevation();
            currentPlayer.ChangeSelectedTile(null);
            TileInfo.Instance.Clear();
            return;
        }

        if (currentPlayer.selectedTile != null && currentPlayer.selectedTile != this)
        {
            currentPlayer.selectedTile.ResetElevation();
        }

        ElevateTile();
        currentPlayer.ChangeSelectedTile(this);

        if (debug) Debug.Log($"[Tile] Tuile cliqu�e : {gameObject.name}, Type : {tileType}, Propri�taire : {(owner != null ? owner.name : "Aucun")}");

        TileInfo.Instance.ChangeInfo(gameObject);

    }

    public bool TryGetProducedAmount(RessourceTypes type, out int amount)
    {
        return producedRessources.TryGetValue(type, out amount);
    }
    public List<RessourceTypes> GetProducedRessourceTypes()
    {
        return new List<RessourceTypes>(producedRessources.Keys);
    }

    public void ElevateTile()
    {
        Vector3 elevatedPosition = transform.position + new Vector3(0, 0.2f, 0); // Sur�l�vation de 0.5 unit�s
        transform.position = elevatedPosition;
        if (debug) Debug.Log($"[Tile] Tuile sur�lev�e : {gameObject.name}");
    }

    public void ResetElevation()
    {
        Vector3 originalPosition = transform.position - new Vector3(0, 0.2f, 0); // R�initialiser la position
        transform.position = originalPosition;
        if (debug) Debug.Log($"[Tile] Tuile r�initialis�e : {gameObject.name}");
    }

    public void SpawnParticle()
    {
        Instantiate(ParticlePrefab, transform);
    }
    public void SetStructure(GameObject structure)
    {
        currentStructure = structure;
    }

    public GameObject GetStructureOnTile()
    {
        return currentStructure;
    }

    public bool HasStructure()
    {
        return currentStructure != null;
    }
}
