using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player
{
    public int points = 0;
    public List<Observer> observers = new List<Observer>();
    public GameObject uiDisplayObject;
    
    public Dictionary<RessourceTypes, int> currentRessources = new Dictionary<RessourceTypes, int>()
    {
        { RessourceTypes.Trees, 0 },
        { RessourceTypes.Minerals, 0 },
        { RessourceTypes.Water, 0 },
        { RessourceTypes.Sun, 0 },
        { RessourceTypes.Oil, 0 }
    };
    public List<Tile> ownedTiles = new List<Tile>();
    public List<Card> cards = new List<Card>();
    public Tile selectedTile = null;

    public abstract Card GetBestPlayableCard();

    public void AddCardInHand(Card card)
    {
        cards.Add(card);
    }

    public void RemoveCardFromHand(Card card)
    {
        cards.Remove(card);
    }

    public void AddOwnedTile(Tile tile)
    {
        tile.owner = this;
        ownedTiles.Add(tile);
    }

    public void RemoveOwnedTile(Tile tile)
    {
        ownedTiles.Remove(tile);
    }

    public void ChangeSelectedTile(Tile tile)
    {
        selectedTile = tile;
    }

    public void ComputeRessources()
    {
        foreach (RessourceTypes resource in Enum.GetValues(typeof(RessourceTypes)))
            currentRessources[resource] = 0;

        foreach (Tile tile in ownedTiles)
        {
            foreach (var kvp in tile.producedRessources)
            {
                currentRessources[kvp.Key] += kvp.Value;
            }
        }

        NotifyObservers();
    }


    public void RegisterObserver(Observer observer)
    {
        if (!observers.Contains(observer))
            observers.Add(observer);
    }
    public void NotifyObservers()
    {
        if (uiDisplayObject == null)
        {
            Debug.LogWarning("Player's uiDisplayObject is null, can't notify observers.");
            return;
        }

        foreach (var observer in observers)
        {
            observer.ObserverUpdate(uiDisplayObject);
        }
    }

}
