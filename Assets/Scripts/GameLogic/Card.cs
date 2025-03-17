using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Card : AnimationController, IClickable
{
    List<ICardEffect> effectList = new List<ICardEffect>();
    Dictionary<RessourceTypes, int> cost = new Dictionary<RessourceTypes, int>();

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI ressourceText;

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

    public void OnClick(GameState gameState)
    {
        // Check if its the turn of the player clicking
        if(gameState.getCurrentPlayingPlayer() == gameState.currentInstancePlayer)
        {
            gameState = gameState.PlayCard(this);
        
            gameState.turnCount++;
            gameState.SetCurrentPlayerTurnToNextPlayer();
            //Update game visuals here
            SelectedVisual();
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
