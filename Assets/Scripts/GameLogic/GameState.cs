using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GameState : MonoBehaviour
{
    private const int WINNING_POINTS_AMOUNT = 100;

    private Player playerCivilisation = null;
    private Player playerWorld = null;
    public Player PlayerCivilisation { set =>  playerCivilisation = value; }
    public Player PlayerWorld { set => playerWorld = value; }
    // This is the player of the running game instance, it is used for multiplayer purposes. Do not confuse with the player currently playing.
    public Player currentInstancePlayer;
    private PlayerType currentPlayerTurn = PlayerType.Civilisation;
    public int turnCount = 0;
    private Board currentBoard;
    public Card lastPlayedCard;

    public bool debug = false;

    public Board GetCurrentBoard()
    {
        return currentBoard;
    }
    public void SetFirstPlayer(PlayerType playerType)
    {
        currentPlayerTurn = playerType;
    }

    public bool HasPlayerWon(Player player)
    {
        if(player.Points >= WINNING_POINTS_AMOUNT) return true;
        return false;
    }


    public List<Card> GetPlayableCards(Player player)
    {
        // TODO : Add logic to return all playable cards available to player
        throw new NotImplementedException();
    }

    public GameState PlayCard(Card card, Player player)
    {
        if (debug) Debug.Log("[GameStage] play a card by " + player.PlayerType.ToString());
        lastPlayedCard = card;
        card.ApplyEffects(this);
        turnCount++;
        if (!card.GetIsPersistent())
        {
            player.MoveCardFromHandToDiscardPile(card);
            Transform hand = player.transform.Find("Discard(Clone)");
            card.gameObject.transform.SetParent(hand, false);
        }

        if (card.GetAddOwnedTile())
        {
            player.AddOwnedTile(player.selectedTile);
        }
        
        player.TrySpendResources(card.cost);
        player.ComputeRessources(this);
        SetCurrentPlayerTurnToNextPlayer();

        return this;
    }

    public void SetCurrentPlayerTurnToNextPlayer()
    {
        currentPlayerTurn = (currentPlayerTurn == PlayerType.Civilisation) ? PlayerType.World : PlayerType.Civilisation;
        GetCurrentPlayingPlayer().DrawCard(this);
        PlayerTurnUi.Instance.SetTurn(currentPlayerTurn);
    }

    public Player GetCurrentPlayingPlayer()
    {
        return currentPlayerTurn switch
        {
            PlayerType.Civilisation => playerCivilisation,
            PlayerType.World => playerWorld,
            _ => null,
        };
    }

    public void SetBoard(Board board)
    {
        currentBoard = board;
    }

    public void DrawCardToHand(Player player)
    {
        int nonPersistentCount = player.hand.FindAll(c => !c.GetIsPersistent()).Count;

        if (nonPersistentCount >= Player.MaxHandSize)
            return;

        if (player.deck.Count == 0 && player.discardPile.Count > 0)
        {
            // Mélanger la défausse dans le deck
            player.ShuffleDiscardIntoDeck();
        }

        if (player.deck.Count == 0)
            return; // Plus rien à piocher

        System.Random random = new System.Random();
        int randomIndex = random.Next(0, player.deck.Count);

        Card drawnCard = player.deck[randomIndex];
        player.MoveCardFromDeckToHand(drawnCard);

        Transform handTransform = (player is HumanPlayer)
            ? GameObject.Find("PlayerHand").transform
            : player.transform.Find("Hand(Clone)");

        drawnCard.gameObject.transform.SetParent(handTransform, false);
    }

    public IEnumerator DrawCardToHandAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DrawCardToHand(GetCurrentPlayingPlayer());
    }

    public Card CreateCardGameObject(CardData cardData, GameObject deck, GameObject cardPrefab)
    {
        Card card = Instantiate(cardPrefab, deck.transform).GetComponent<Card>();

        card.InitializeCard(cardData.cardName, cardData.description,
            cardData.effectList, cardData.cost, cardData.conditionList, cardData.addOwnedTile, cardData.isPersistent);
        return card;
    }
}
