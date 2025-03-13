using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour, IClickable
{
    public Dictionary<RessourceTypes, int> producedRessources = new Dictionary<RessourceTypes, int>();
    public TileType tileType;

    public void Initialize(TileType type)
    {
        this.tileType = type;
        AssignResources();
    }

    private void AssignResources()
    {
        producedRessources.Clear(); // Réinitialiser les ressources

        switch (tileType)
        {
            case TileType.Forests:
                producedRessources[RessourceTypes.Trees] = Random.Range(3, 7);  // Forêts produisent du bois
                break;
            case TileType.Mountains:
                producedRessources[RessourceTypes.Minerals] = Random.Range(2, 6); // Montagnes produisent des minéraux
                break;
            case TileType.Lakes:
                producedRessources[RessourceTypes.Water] = Random.Range(4, 8); // Lacs produisent de l’eau
                break;
            case TileType.Plains:
                producedRessources[RessourceTypes.Sun] = Random.Range(1, 5); // Plaines absorbent l'énergie solaire
                break;
            case TileType.Deserts:
                producedRessources[RessourceTypes.Oil] = Random.Range(1, 3); // Déserts peuvent contenir du pétrole
                break;
        }
    }
    public void OnClick(GameState gameState)
    {
        // TODO : Set player selected Tile

        // TODO : Update game visuals here for ressources display
    }
}
