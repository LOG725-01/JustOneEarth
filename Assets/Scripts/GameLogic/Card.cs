using System.Collections.Generic;
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
        gameState = gameState.PlayCard(this);

        gameState.turnCount++;
        gameState.SetCurrentPlayerTurnNext();

        // TODO : Update game visuals here
    }
}
