using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    List<Tile> tiles = new List<Tile>();

    public Board CreateBoard()
    {
        // TODO : Implement procedural creation logic here
        Tile tile = new();
        tiles.Add(tile);
        return this;
    }
}
