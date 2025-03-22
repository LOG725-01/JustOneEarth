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
        var observers = FindObjectsOfType<Observer>();
        var player = gameState.getCurrentPlayingPlayer();
        gameState.AddPlayers(gameMode);

        //TODO : fix nullexception when not commented
        //gameState.CreateBoard();
        
        playerInputNotifiers.Clear();
        playerInputNotifiers.AddRange(FindObjectsOfType<PlayerInputNotifier>());

        foreach (var obs in observers)
        {
            player.RegisterObserver(obs);
        }

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
            TileInfo.Instance.Clear();
            Debug.Log("Clicked object has no specific click behavior.");
        }
    }

    public void PlayCardFromUI(Card card)
    {
        var player = gameState.getCurrentPlayingPlayer();

        if (player is HumanPlayer human)
        {
            if (card.CanBePlayed(human.currentRessources) && human.selectedTile != null)
            {
                card.ApplyEffects(gameState);
                human.RemoveCardFromHand(card);
                human.ComputeRessources();

                gameState.turnCount++;
                gameState.SetCurrentPlayerTurnToNextPlayer();
            }
            else
            {
                Debug.Log("Carte non jouable ou tuile non sélectionnée.");
            }
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
            else if (player is HumanPlayer)
            {
                // Le joueur humain peut interagir manuellement
                // Rien à faire ici : ses actions passent par les événements (clics, boutons UI, etc.)
            }
    }
}
