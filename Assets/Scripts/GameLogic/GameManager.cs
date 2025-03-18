using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameMode gameMode;
    private GameState gameState;
    private List<PlayerInputNotifier> playerInputNotifiers = new List<PlayerInputNotifier>();
    private bool gameStarted = false;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetGameMode(GameMode gameMode)
    {
        this.gameMode = gameMode;
    }

    public void StartGame()
    {
        gameState = new GameState();
        gameState.AddPlayers(gameMode);

        // TODO : fix NullReferenceException when uncommented
        // gameState.CreateBoard();

        playerInputNotifiers.Clear();
        playerInputNotifiers.AddRange(FindObjectsOfType<PlayerInputNotifier>());

        foreach (var notifier in playerInputNotifiers)
        {
            notifier.OnGameObjectClicked += HandlePlayerInput;
        }

        Debug.Log("Partie en solo commencée !");
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
            Debug.Log("L'objet cliqué n'a pas de comportement spécifique.");
        }
    }

    private void Update()
    {
        if (!gameStarted) return;

        // Vérifier si c'est au tour de l'IA de jouer
        Player player = gameState.getCurrentPlayingPlayer();

        if (player != null && player is AIPlayer aiPlayer)
        {
            gameState = gameState.PlayCard(aiPlayer.GetBestPlayableCard());
            gameState.turnCount++;
            gameState.SetCurrentPlayerTurnToNextPlayer();
            // TODO : Mettre à jour les visuels du jeu ici
        }
    }
}
