using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Card : MonoBehaviour, IClickable
{
    List<ICardEffect> effectList = new List<ICardEffect>();
    Dictionary<RessourceTypes, int> cost = new Dictionary<RessourceTypes, int>();

    public void AddEffect(ICardEffect effect)
    {
        effectList.Add(effect);
    }

    public void AddCost(RessourceTypes ressource, int quantity)
    {
        cost.Add(ressource, quantity);
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
            // TODO : Update game visuals here
        }
    }
}
