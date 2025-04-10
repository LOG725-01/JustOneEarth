using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : AnimationController, IClickable, IPointerClickHandler
{
    List<ICardEffect> effectList = new List<ICardEffect>();
    public Dictionary<RessourceTypes, int> cost = new Dictionary<RessourceTypes, int>();

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI ressourceText;
    private List<ICardCondition> conditions = new List<ICardCondition>();
    private bool addOwnedTile;
    private bool isPersistent;
    private GameState GameStateReference;

    public bool debug = false;
    public void InitializeCard(string titleText, string ressourceText, List<ICardEffect> effectList, Dictionary<RessourceTypes, int> cost, 
        List<ICardCondition> conditionList, bool addOwnedTile, bool isPersistent)
    {
        this.titleText.text = titleText;
        this.ressourceText.text = ressourceText;
        this.effectList = effectList;
        this.cost = cost;
        this.conditions = conditionList;
        this.addOwnedTile = addOwnedTile;
        this.isPersistent = isPersistent;
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
    
    public bool CanBePlayed(Dictionary<RessourceTypes, int> playerResources, GameState gameState, Player player)
    {
        foreach (var entry in cost)
        {
            if (debug) Debug.Log("[Card] cost values : " + entry.Key.ToString() + " : " + entry.Value.ToString());
            if (debug) Debug.Log("[Card] playerResources values : " + playerResources.ContainsKey(entry.Key).ToString() + " : " + playerResources[entry.Key].ToString());
            if (!playerResources.ContainsKey(entry.Key) || playerResources[entry.Key] < entry.Value)
                return false;
        }

        foreach (var condition in conditions)
        {
            if (!condition.IsMet(gameState, player))
                return false;
        }

        return true;
    }

    public void OnClick(GameState gameState)
    {
        if (debug) Debug.Log("[Card] card clicked");
        // Check if its the turn of the player clicking
        if(gameState.GetCurrentPlayingPlayer() == gameState.currentInstancePlayer)
        {
            if (debug) Debug.Log("[Card] current Instance Player is current playing player");
            // Check if card can be played and if a tile is selected
            if (debug) Debug.Log("[Card] selected tile : " + gameState.currentInstancePlayer.selectedTile.ToString());
            if (CanBePlayed(gameState.currentInstancePlayer.currentRessources,gameState, gameState.currentInstancePlayer))
            {
                if (debug) Debug.Log("[Card] CanBePlayed");
                gameState = gameState.PlayCard(this, gameState.GetCurrentPlayingPlayer());

                //Update game visuals here
                SelectedVisual();
            }
        }
        else
        {
            NormalVisual();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Player player = GameStateReference.currentInstancePlayer;
            if (!isPersistent && player.hand.Contains(this))
            {
                player.MoveCardFromHandToDiscardPile(this);
                Transform discardTransform = player.transform.Find("Discard(Clone)");
                transform.SetParent(discardTransform, false);
                if (debug) Debug.Log("[Card] Carte défaussée via clic droit.");
            }
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

    public void AddCondition(ICardCondition condition)
    {
        conditions.Add(condition);
    }
    public bool GetAddOwnedTile() { return addOwnedTile; }
    public bool GetIsPersistent() { return isPersistent; }
    public bool TryPlay(GameState gameState, Player player)
    {
        if (!CanBePlayed(player.currentRessources, gameState, player))
            return false;

        ApplyEffects(gameState);
        if (addOwnedTile) player.AddOwnedTile(player.selectedTile);
        if (!isPersistent) player.MoveCardFromHandToDiscardPile(this);

        return true;
    }
    public void SetGameStateReference(GameState state)
    {
        GameStateReference = state;
    }
}
