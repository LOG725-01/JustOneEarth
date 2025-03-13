using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour, IClickable
{
    public Dictionary<RessourceTypes, int> producedRessources = new Dictionary<RessourceTypes, int>();
    TileType tileType;

    public Tile()
    {

    }

    public Tile(Dictionary<RessourceTypes, int> producedRessources, TileType tileType)
    {
        this.producedRessources = producedRessources;
        this.tileType = tileType;
    }

    public void OnClick(GameState gameState)
    {
        gameState.currentInstancePlayer.ChangeSelectedTile(this);

        // TODO : Update game visuals here
    }
}
