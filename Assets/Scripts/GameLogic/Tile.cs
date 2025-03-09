using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Dictionary<RessourceTypes, int> producedRessources = new Dictionary<RessourceTypes, int>();
    TileType tileType;

    Tile(Dictionary<RessourceTypes, int> producedRessources, TileType tileType)
    {
        this.producedRessources = producedRessources;
        this.tileType = tileType;
    }

}
