using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameState : MonoBehaviour
{
    private const int PLAYER_AMOUNT = 2;
    private const int WINNING_POINTS_AMOUNT = 100;
    // Players are stored in order of turn. First player will be at index 0, second at 1
    public List<Player> players = new List<Player>();
    // This is the player of the running game instance, it is used for multiplayer purposes. Do not confuse with the player currently playing.
    public Player currentInstancePlayer;
    GameObject playerUIObject = GameObject.Find("HUD");
    public int currentPlayerTurn = 0;
    public int turnCount = 0;
    private Board currentBoard = new Board();

    public void AddPlayers(GameMode gameMode)
    {
        currentInstancePlayer = new HumanPlayer();
        currentInstancePlayer.uiDisplayObject = playerUIObject;

        if (gameMode == GameMode.PVE)
        {
            players.Add(currentInstancePlayer);
            players.Add(new AIPlayer());
        }
        else if(gameMode == GameMode.PVP)
        {
            players.Add(currentInstancePlayer);
            players.Add(new HumanPlayer());
        }

        Shuffle(players);
    }

    private void Shuffle(List<Player> list)
    {
        System.Random rng = new System.Random();

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Player value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public void CreateBoard()
    {
        currentBoard.CreateBoard();
    }

    public Board GetCurrentBoard()
    {
        return currentBoard;
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

    public void SetCurrentPlayerTurnToNextPlayer()
    {
        currentPlayerTurn = (currentPlayerTurn + 1) % PLAYER_AMOUNT;
    }

    public int GetNextPlayingPlayerIndex()
    {
        return (currentPlayerTurn + 1) % PLAYER_AMOUNT;
    }

    public Player getCurrentPlayingPlayer()
    {
        if (players.Count <= currentPlayerTurn) return null;
        return players.ElementAt<Player>(currentPlayerTurn);
    }
}
