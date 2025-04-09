using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private int turnCount = 0;
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
        player.MoveCardFromHandToDiscardPile(card);
        player.AddOwnedTile(player.selectedTile);
        player.TrySpendResources(card.cost);
        player.ComputeRessources();
        SetCurrentPlayerTurnToNextPlayer();

        Transform hand = player.transform.Find("Discard(Clone)");
        card.gameObject.transform.SetParent(hand, false);
        return this;
    }

    public void SetCurrentPlayerTurnToNextPlayer()
    {
        switch (currentPlayerTurn)
        {
            case PlayerType.Civilisation:
                currentPlayerTurn = PlayerType.World;
                break;
            case PlayerType.World:
                currentPlayerTurn = PlayerType.Civilisation;
                break;
        }
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
        if (player.deck.Count > 0)
        {
            System.Random random = new System.Random();
            int randomIndex = random.Next(0, player.deck.Count);

            Card drawnCard = player.deck[randomIndex];
            player.MoveCardFromDeckToHand(drawnCard);

            Transform hand;

            if (player is HumanPlayer)
            {
                hand = GameObject.Find("PlayerHand").transform;
            }
            else
            {
                hand = player.transform.Find("Hand(Clone)");
            }
            drawnCard.gameObject.transform.SetParent(hand, false);
        }
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
            cardData.effectList, cardData.cost, cardData.conditionList);
        return card;
    }
}
