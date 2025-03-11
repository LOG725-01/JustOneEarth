using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    private const int PLAYER_AMOUNT = 2;
    private const int WINNING_POINTS_AMOUNT = 100;
    public List<Player> players;
    // TODO : Initialize dictionnary to null for each player
    public Dictionary<int, Tile> playerSelectedTile;
    public int currentPlayerTurn;
    public int turnCount;
    public Board currentBoard;

    public void AddPlayer(Player player)
    {
        players.Add(player);
    }

    public void CreateBoard()
    {
        currentBoard = currentBoard.CreateBoard();
    }

    public bool HasPlayerWon(Player player)
    {
        if(player.points >= WINNING_POINTS_AMOUNT) return true;
        return false;
    }

    public List<Card> GetPlayableCards(Player player)
    {
        // TODO : Add logic to return all playable cards available to player
        throw new NotImplementedException();
    }

    public GameState PlayCard(Card card)
    {
        card.ApplyEffects(this);
        return this;
    }

    public void SetCurrentPlayerTurnNext()
    {
        currentPlayerTurn = (gameState.currentPlayerTurn + 1) % gameState.PLAYER_AMOUNT;
    }
}
