using System;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameMode gameMode;

    [SerializeField] public GameObject playerInputPrefab;

    GameState gameState;
    private List<PlayerInputNotifier> playerInputNotifier = new List<PlayerInputNotifier>;

    private void Start()
    {
        gameState = new GameState();

        playerInputNotifier.AddRange(FindObjectsOfType<PlayerInputNotifier>());

        foreach (var notifier in notifiers)
        {
            playerInputNotifier.OnGameObjectClicked += HandleGameObjectClick;
        }

        // TODO : Call to create board 
        // TODO : add players to gameState according to game mode (player vs player or player vs AI)
        // TODO : create GameManager from SceneManager and set gameMode 
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
        //AI turn to play
        Player player = gameState.players.ElementAt(gameState.currentPlayerTurn);

        if (player.GetType() == typeof(AIPlayer))
        {
            AIPlayer aiPlayer = (AIPlayer)player;
            gameState = gameState.PlayCard(aiPlayer.GetBestPlayableCard());

            gameState.turnCount++;
            gameState.SetCurrentPlayerTurnNext();
            // TODO : Update game visuals here
        }
    }
}
