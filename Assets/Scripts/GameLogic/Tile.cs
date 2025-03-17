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
        producedRessources.Clear();

        switch (tileType)
        {
            case TileType.Forests:
                producedRessources[RessourceTypes.Trees] = Random.Range(3, 7); 
                break;
            case TileType.Mountains:
                producedRessources[RessourceTypes.Minerals] = Random.Range(2, 6); 
                break;
            case TileType.Lakes:
                producedRessources[RessourceTypes.Water] = Random.Range(4, 8);
                break;
            case TileType.Plains:
                producedRessources[RessourceTypes.Sun] = Random.Range(1, 5);
                break;
            case TileType.Deserts:
                producedRessources[RessourceTypes.Oil] = Random.Range(1, 3);
                break;
        }
    }
    public void OnClick(GameState gameState)
    {
        // TODO : Set player selected Tile

        // Update game visuals here for ressources display
        TileInfo.Instance.ChangeInfo(gameObject);
    }
}
