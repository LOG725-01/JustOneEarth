using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameMode gameMode;

    GameState gameState;

    private List<PlayerInputNotifier> playerInputNotifiers = new List<PlayerInputNotifier>();

    private bool gameStarted = false;

    public void setGameMode(GameMode gameMode)
    {
        this.gameMode = gameMode;
    }

    public void StartGame()
    {
        gameState = new GameState();

        gameState.AddPlayers(gameMode);

        //TODO : fix nullexception when not commented
        //gameState.CreateBoard();

        playerInputNotifiers.Clear();
        playerInputNotifiers.AddRange(FindObjectsOfType<PlayerInputNotifier>());

        foreach (var notifier in playerInputNotifiers)
        {
            notifier.OnGameObjectClicked += HandlePlayerInput;
        }

        // TODO : add card playing logic. Dont forget to add new cards and remove used cards in playerInputNotifiers
        gameStarted = true;
    }

    private void HandlePlayerInput(GameObject clickedObject)
    {
        if (clickedObject.TryGetComponent<IClickable>(out var clickable))
        {
            clickable.OnClick(gameState);
        }
        else
        {
            Debug.Log("Clicked object has no specific click behavior.");
        }
    }

    private void Update()
    {
        if (!gameStarted) return;
        // Check if AI turn to play
        Player player = gameState.getCurrentPlayingPlayer();

        if (player != null) 
            if (player.GetType() == typeof(AIPlayer))
            {
                // AI play
                AIPlayer aiPlayer = (AIPlayer)player;
                gameState = gameState.PlayCard(aiPlayer.GetBestPlayableCard());

                gameState.turnCount++;
                gameState.SetCurrentPlayerTurnToNextPlayer();
                // TODO : Update game visuals here
            }
    }
}
