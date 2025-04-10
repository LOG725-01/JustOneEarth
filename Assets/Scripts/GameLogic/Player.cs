using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    private int points = 0;
    public int Points { get => points; 
    set {
            points = value;
            NotifyObservers();
        } }

    private PlayerType playerType = PlayerType.Civilisation;
    public PlayerType PlayerType { get => playerType; set => playerType = value; }

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

    public const int MaxHandSize = 5;

    public bool debug = true;

    public abstract Card GetBestPlayableCard(GameState gameState);

    public void AddCardInDeck(Card card)
    {
        deck.Add(card);
    }

    public void GiveCard(Card card)
    {
        hand.Add(card);
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
        if (card.GetIsPersistent()) return;
        if (hand.Contains(card))
        {
            hand.Remove(card);
            discardPile.Add(card);
        }
    }

    public void AddOwnedTile(Tile tile)
    {
        if (tile == null) return;
        tile.owner = this;
        ownedTiles.Add(tile);
        tile.UpdateBorder();
        if (debug) Debug.Log($"[Player] Tuile {tile.name} ajoutée au joueur.");
    }

    public void RemoveOwnedTile(Tile tile)
    {
        ownedTiles.Remove(tile);
    }

    public void ChangeSelectedTile(Tile tile)
    {
        selectedTile = tile;
    }

    public void ComputeRessources(GameState gameState)
    {
        if (debug) Debug.Log("[Player] Début du calcul des ressources...");
        if (gameState.turnCount == 0)
        {
            foreach (RessourceTypes resource in Enum.GetValues(typeof(RessourceTypes)))
            {
                currentRessources[resource] = 0;
                if (debug) Debug.Log($"[Player] Ressource réinitialisée : {resource} = 0");
            }
        }


        foreach (Tile tile in ownedTiles)
        {
            if (debug) Debug.Log($"[Player] Analyse de la tuile : {tile.gameObject.name}, Type : {tile.tileType}");

            foreach (var kvp in tile.producedRessources)
            {
                currentRessources[kvp.Key] += kvp.Value;
                if (debug) Debug.Log($"[Player] +{kvp.Value} {kvp.Key} depuis {tile.gameObject.name} (Total : {currentRessources[kvp.Key]})");
            }
        }

        if (debug) Debug.Log("[Player] Calcul des ressources terminé. Résumé :");
        foreach (var res in currentRessources)
        {
            if (debug) Debug.Log($"[Player] {res.Key} = {res.Value}");
        }

        NotifyObservers();
        if (debug) Debug.Log("[Player] Observateurs notifiés.");
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

    public bool TrySpendResources(Dictionary<RessourceTypes, int> cost)
    {
        // Vérifie si le joueur a assez de ressources
        foreach (var res in cost)
        {
            if (!currentRessources.ContainsKey(res.Key) || currentRessources[res.Key] < res.Value)
            {
                if (debug) Debug.LogWarning($"[Player] Ressource insuffisante : {res.Key} requis = {res.Value}, disponible = {currentRessources[res.Key]}");
                return false; // Ressource manquante
            }
        }

        // Retire les ressources
        foreach (var res in cost)
        {
            currentRessources[res.Key] -= res.Value;
            if (debug) Debug.Log($"[Player] -{res.Value} {res.Key} (Nouveau total : {currentRessources[res.Key]})");
        }

        NotifyObservers(); // Met à jour les UI/écouteurs
        return true;
    }

    public void DeselectTile()
    {
        if (selectedTile != null)
        {
            selectedTile.ResetElevation();
            selectedTile = null;
            if (debug) Debug.Log("[Player] Tuile désélectionnée.");
        }
    }
    
    public void ShuffleDiscardIntoDeck()
    {
        deck.AddRange(discardPile);
        discardPile.Clear();

        // Optionnel : mélanger le deck
        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck[i];
            int randomIndex = UnityEngine.Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }
    
    public void DrawCard(GameState gameState)
    {
        int nonPersistentCount = hand.FindAll(c => !c.GetIsPersistent()).Count;
        if (nonPersistentCount >= MaxHandSize) return;

        if (deck.Count == 0 && discardPile.Count > 0)
            ShuffleDiscardIntoDeck();

        if (deck.Count == 0) return;

        var randomIndex = UnityEngine.Random.Range(0, deck.Count);
        Card card = deck[randomIndex];
        deck.RemoveAt(randomIndex);
        hand.Add(card);

        Transform handTransform = this is HumanPlayer ?
            GameObject.Find("PlayerHand").transform :
            transform.Find("Hand(Clone)");

        card.transform.SetParent(handTransform, false);
    }
}
