using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class Card : AnimationController, IClickable
{
    List<ICardEffect> effectList = new List<ICardEffect>();
    Dictionary<RessourceTypes, int> cost = new Dictionary<RessourceTypes, int>();

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI ressourceText;

    public void InitializeCard(TextMeshProUGUI titleText, TextMeshProUGUI ressourceText, List<ICardEffect> effectList, Dictionary<RessourceTypes, int> cost)
    {
        this.titleText = titleText;
        this.ressourceText = ressourceText;
        this.effectList = effectList;
        this.cost = cost;
    }

    public void Start()
    {
        NormalVisual();
    }

    public void AddEffect(ICardEffect effect)
    {
        effectList.Add(effect);
    }

    public void AddCost(RessourceTypes ressource, int quantity)
    {
        cost.Add(ressource, quantity);
        ressourceText.text = quantity.ToString();
        //TODO : update ressource icon
    }

    public void ApplyEffects(GameState gameState)
    {
        foreach (var effect in effectList) 
        {
            effect.ApplyEffect(gameState);
        }
    }
    
    public Dictionary<RessourceTypes, int> GetCost()
    {
        return new Dictionary<RessourceTypes, int>(cost); // Copie défensive
    }
    
    public bool CanBePlayed(Dictionary<RessourceTypes, int> playerResources)
    {
        foreach (var entry in cost)
        {
            if (!playerResources.ContainsKey(entry.Key) || playerResources[entry.Key] < entry.Value)
                return false;
        }
        return true;
    }

    public void OnClick(GameState gameState)
    {
        Debug.Log("card clicked");
        // Check if its the turn of the player clicking
        if(gameState.getCurrentPlayingPlayer() == gameState.currentInstancePlayer)
        {
            // Check if card can be played and if a tile is selected
            if (CanBePlayed(gameState.currentInstancePlayer.currentRessources) && gameState.currentInstancePlayer.selectedTile != null)
            {
                gameState = gameState.PlayCard(this);
                gameState.turnCount++;
                gameState.SetCurrentPlayerTurnToNextPlayer();
                gameState.currentInstancePlayer.MoveCardFromHandToDiscardPile(this);

                GameObject hand = GameObject.Find("DiscardPile");
                this.gameObject.transform.SetParent(hand.transform, false);

                //Update game visuals here
                SelectedVisual();
            }
        }
        else
        {
            NormalVisual();
        }
    }

    private void SelectedVisual()
    {
        ChangeAnimation("Selected");

    }
    
    private void NormalVisual()
    {
        ChangeAnimation("Normal");
    }
}
