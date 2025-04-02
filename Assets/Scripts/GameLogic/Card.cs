using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Card : AnimationController, IClickable
{
    List<ICardEffect> effectList = new List<ICardEffect>();
    public Dictionary<RessourceTypes, int> cost = new Dictionary<RessourceTypes, int>();

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI ressourceText;

    public bool debug = false;
    public void InitializeCard(string titleText, string ressourceText, List<ICardEffect> effectList, Dictionary<RessourceTypes, int> cost)
    {
        this.titleText.text = titleText;
        this.ressourceText.text = ressourceText;
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
        if (debug) Debug.Log("[Card] CanBePlayed called");
        foreach (var entry in cost)
        {
            if (debug) Debug.Log("[Card] cost values : " + entry.Key.ToString() + " : " + entry.Value.ToString());
            if (debug) Debug.Log("[Card] playerResources values : " + playerResources.ContainsKey(entry.Key).ToString() + " : " + playerResources[entry.Key].ToString());
            if (!playerResources.ContainsKey(entry.Key) || playerResources[entry.Key] < entry.Value)
                return false;
        }
        return true;
    }

    public void OnClick(GameState gameState)
    {
        if (debug) Debug.Log("[Card] card clicked");
        // Check if its the turn of the player clicking
        if(gameState.getCurrentPlayingPlayer() == gameState.currentInstancePlayer)
        {
            if (debug) Debug.Log("[Card] current Instance Player is current playing player");
            // Check if card can be played and if a tile is selected
            if (debug) Debug.Log("[Card] selected tile : " + gameState.currentInstancePlayer.selectedTile.ToString());
            if (CanBePlayed(gameState.currentInstancePlayer.currentRessources) && gameState.currentInstancePlayer.selectedTile != null)
            {
                if (debug) Debug.Log("[Card] CanBePlayed");
                gameState = gameState.PlayCard(this, gameState.getCurrentPlayingPlayer());

                StartCoroutine(gameState.DrawCardToHandAfterDelay(3));

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
