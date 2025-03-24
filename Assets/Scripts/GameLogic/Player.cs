using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public int points = 0;
    public List<Observer> observers = new List<Observer>();
    
    public Dictionary<RessourceTypes, int> currentRessources = new Dictionary<RessourceTypes, int>()
    {
        { RessourceTypes.Trees, 0 },
        { RessourceTypes.Minerals, 0 },
        { RessourceTypes.Water, 0 },
        { RessourceTypes.Sun, 0 },
        { RessourceTypes.Oil, 0 }
    };
    public List<Tile> ownedTiles = new List<Tile>();
    public List<Card> deck = new List<Card>();
    public List<Card> hand = new List<Card>();
    public List<Card> discardPile = new List<Card>();
    public Tile selectedTile = null;

    public abstract Card GetBestPlayableCard();

    public void AddCardInDeck(Card card)
    {
        deck.Add(card);
    }

    public void MoveCardFromDeckToHand(Card card)
    {
        if (deck.Contains(card))
        {
            deck.Remove(card);
            hand.Add(card);
        }
    }

    public void MoveCardFromHandToDiscardPile(Card card)
    {
        if (hand.Contains(card))
        { 
            hand.Remove(card);
            discardPile.Add(card);
        }
    }

    public void AddOwnedTile(Tile tile)
    {
        tile.owner = this;
        ownedTiles.Add(tile);
        Debug.Log($"[Player] Tuile {tile.name} ajoutée au joueur.");
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
        Debug.Log("[Player] Début du calcul des ressources...");

        foreach (RessourceTypes resource in Enum.GetValues(typeof(RessourceTypes)))
        {
            currentRessources[resource] = 0;
            Debug.Log($"[Player] Ressource réinitialisée : {resource} = 0");
        }

        foreach (Tile tile in ownedTiles)
        {
            Debug.Log($"[Player] Analyse de la tuile : {tile.gameObject.name}, Type : {tile.tileType}");

            foreach (var kvp in tile.producedRessources)
            {
                currentRessources[kvp.Key] += kvp.Value;
                Debug.Log($"[Player] +{kvp.Value} {kvp.Key} depuis {tile.gameObject.name} (Total : {currentRessources[kvp.Key]})");
            }
        }

        Debug.Log("[Player] Calcul des ressources terminé. Résumé :");
        foreach (var res in currentRessources)
        {
            Debug.Log($"[Player] {res.Key} = {res.Value}");
        }

        NotifyObservers();
        Debug.Log("[Player] Observateurs notifiés.");
    }

    public void RegisterObserver(Observer observer)
    {
        if (!observers.Contains(observer))
            observers.Add(observer);
    }

    public void NotifyObservers()
    {
        if (gameObject == null)
        {
            Debug.LogWarning("Player is null, can't notify observers.");
            return;
        }

        foreach (var observer in observers)
        {
            observer.ObserverUpdate(gameObject);
        }
    }
}
