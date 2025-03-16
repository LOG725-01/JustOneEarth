using System;
using System.Collections.Generic;
using System.Linq;
using Mirror; // Importation pour g�rer le r�seau
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private GameMode gameMode;

    GameState gameState;

    private List<PlayerInputNotifier> playerInputNotifiers = new List<PlayerInputNotifier>();

    public void setGameMode(GameMode gameMode)
    {
        this.gameMode = gameMode;
    }

    public void StartGame()
    {
        gameState = new GameState();

        gameState.AddPlayers(gameMode);

        gameState.CreateBoard();

        playerInputNotifiers.AddRange(FindObjectsOfType<PlayerInputNotifier>());

        foreach (var notifier in playerInputNotifiers)
        {
            notifier.OnGameObjectClicked += HandlePlayerInput;
        }

        if (NetworkServer.active)
        {
            Debug.Log("Partie en multijoueur (serveur h�te actif)");
        }

        // TODO : add card playing logic. Dont forget to add new cards and remove used cards in playerInputNotifiers
    }

    [Command] // Cette fonction s'ex�cute sur le serveur et est appel�e par un client
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
        if (!isServer) return;

        // Check if AI turn to play
        Player player = gameState.players.ElementAt(gameState.currentPlayerTurn);

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
