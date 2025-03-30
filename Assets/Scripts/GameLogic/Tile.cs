using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour, IClickable
{
    public Dictionary<RessourceTypes, int> producedRessources = new Dictionary<RessourceTypes, int>();
    public TileType tileType;
    public Player owner;

    public bool debug = false;


    public void Initialize(TileType type, bool _debug = false)
    {
        tileType = type;
        debug = _debug;
        AssignResources();

        if (debug) Debug.Log($"[Tile] Initialisation : {gameObject.name} - Type : {tileType} - Ressources : " +
                  string.Join(", ", producedRessources));
    }

    private void AssignResources()
    {
        producedRessources.Clear();
        if (debug) Debug.Log($"[Tile] Attribution des ressources pour la tuile de type {tileType}");

        switch (tileType)
        {
            case TileType.Forests:
                producedRessources[RessourceTypes.Trees] = Random.Range(3, 7);
                if (debug) Debug.Log($"[Tile] Ressource générée : Trees => {producedRessources[RessourceTypes.Trees]}");
                break;
            case TileType.Mountains:
                producedRessources[RessourceTypes.Minerals] = Random.Range(2, 6);
                if (debug) Debug.Log($"[Tile] Ressource générée : Minerals => {producedRessources[RessourceTypes.Minerals]}");
                break;
            case TileType.Lakes:
                producedRessources[RessourceTypes.Water] = Random.Range(4, 8);
                if (debug) Debug.Log($"[Tile] Ressource générée : Water => {producedRessources[RessourceTypes.Water]}");
                break;
            case TileType.Plains:
                producedRessources[RessourceTypes.Sun] = Random.Range(1, 5);
                if (debug) Debug.Log($"[Tile] Ressource générée : Sun => {producedRessources[RessourceTypes.Sun]}");
                break;
            case TileType.Deserts:
                producedRessources[RessourceTypes.Oil] = Random.Range(1, 3);
                if (debug) Debug.Log($"[Tile] Ressource générée : Oil => {producedRessources[RessourceTypes.Oil]}");
                break;
            default:
                if (debug) Debug.LogWarning($"[Tile] Aucun type reconnu pour la tuile : {tileType}");
                break;
        }
    }
    public void OnClick(GameState gameState)
    {
        gameState.currentInstancePlayer.selectedTile = this;
        if (debug) Debug.Log($"[Tile] Tuile cliquée : {gameObject.name}, Type : {tileType}, Propriétaire : {(owner != null ? owner.name : "Aucun")}");

        TileInfo.Instance.ChangeInfo(gameObject);
        if (debug) Debug.Log($"[Tile] Affichage des informations de la tuile mis à jour.");
    }
}
