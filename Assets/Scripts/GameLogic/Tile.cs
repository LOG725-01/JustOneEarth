using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour, IClickable
{
    public Dictionary<RessourceTypes, int> producedRessources = new Dictionary<RessourceTypes, int>();
    public TileType tileType;
    public Player owner;

    public void Initialize(TileType type)
    {
        this.tileType = type;
        AssignResources();

        Debug.Log($"[Tile] Initialisation : {gameObject.name} - Type : {tileType} - Ressources : " +
                  string.Join(", ", producedRessources));
    }

    private void AssignResources()
    {
        producedRessources.Clear();
        Debug.Log($"[Tile] Attribution des ressources pour la tuile de type {tileType}");

        switch (tileType)
        {
            case TileType.Forests:
                producedRessources[RessourceTypes.Trees] = Random.Range(3, 7);
                Debug.Log($"[Tile] Ressource générée : Trees => {producedRessources[RessourceTypes.Trees]}");
                break;
            case TileType.Mountains:
                producedRessources[RessourceTypes.Minerals] = Random.Range(2, 6);
                Debug.Log($"[Tile] Ressource générée : Minerals => {producedRessources[RessourceTypes.Minerals]}");
                break;
            case TileType.Lakes:
                producedRessources[RessourceTypes.Water] = Random.Range(4, 8);
                Debug.Log($"[Tile] Ressource générée : Water => {producedRessources[RessourceTypes.Water]}");
                break;
            case TileType.Plains:
                producedRessources[RessourceTypes.Sun] = Random.Range(1, 5);
                Debug.Log($"[Tile] Ressource générée : Sun => {producedRessources[RessourceTypes.Sun]}");
                break;
            case TileType.Deserts:
                producedRessources[RessourceTypes.Oil] = Random.Range(1, 3);
                Debug.Log($"[Tile] Ressource générée : Oil => {producedRessources[RessourceTypes.Oil]}");
                break;
            default:
                Debug.LogWarning($"[Tile] Aucun type reconnu pour la tuile : {tileType}");
                break;
        }
    }
    public void OnClick(GameState gameState)
    {
        // TODO : Set player selected Tile
        Debug.Log($"[Tile] Tuile cliquée : {gameObject.name}, Type : {tileType}, Propriétaire : {(owner != null ? owner.name : "Aucun")}");

        // Update game visuals here for ressources display
        TileInfo.Instance.ChangeInfo(gameObject);
        Debug.Log($"[Tile] Affichage des informations de la tuile mis à jour.");
    }
}
