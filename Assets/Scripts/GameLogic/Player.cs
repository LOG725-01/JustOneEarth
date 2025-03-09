using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player
{
    public int points = 0;
    public Dictionary<RessourceTypes, int> currentRessources = new Dictionary<RessourceTypes, int>()
    {
        { RessourceTypes.Trees, 0 },
        { RessourceTypes.Minerals, 0 },
        { RessourceTypes.Water, 0 },
        { RessourceTypes.Sun, 0 },
        { RessourceTypes.Oil, 0 }
    };
    List<Tile> ownedTiles = new List<Tile>();
    List<Card> cards = new List<Card>();

    public abstract Card GetBestPlayableCard();

    public void AddCard(Card card)
    {
        cards.Add(card);
    }

    public void RemoveCard(Card card)
    {
        cards.Remove(card);
    }

    public void AddOwnedTile(Tile tile)
    {
        ownedTiles.Add(tile);
    }

    public void RemoveOwnedTile(Tile tile)
    {
        ownedTiles.Remove(tile);
    }

    public void ComputeRessources()
    {
        // Reset current resources for each resource type
        foreach (RessourceTypes resource in Enum.GetValues(typeof(RessourceTypes)))
        {
            currentRessources[resource] = 0;
        }

        // Accumulate resources produced by each tile
        foreach (Tile tile in ownedTiles)
        {
            foreach (KeyValuePair<RessourceTypes, int> kvp in tile.producedRessources)
            {
                currentRessources[kvp.Key] += kvp.Value;
            }
        }
    }
}
