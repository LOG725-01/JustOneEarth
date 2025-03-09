using System;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const int PLAYER_AMOUNT = 2;
    private GameMode gameMode;

    [SerializeField] public GameObject playerInputPrefab;

    GameState gameState;
    PlayerInput playerInput;

    private void Start()
    {
        gameState = new GameState();

        GameObject inputObject = Instantiate(playerInputPrefab);
        playerInput = inputObject.GetComponent<PlayerInput>();

        // TODO : add OnPlayerInput to playerInput
        playerInput.OnPlayerInput += HandlePlayerInput;

        // TODO : add players to gameState according to game mode (player vs player or player vs AI)
        // TODO : create GameManager from SceneManager and set gameMode 
    }

    private void HandlePlayerInput(Card card)
    {
        gameState = gameState.PlayCard(card);

        gameState.turnCount++;
        gameState.currentPlayerTurn = (gameState.currentPlayerTurn + 1) % PLAYER_AMOUNT;
        // TODO : Update game visuals here
    }

    private void Update()
    {
        Player player = gameState.players.ElementAt(gameState.currentPlayerTurn);

        if (player.GetType() == typeof(AIPlayer))
        {
            AIPlayer aiPlayer = (AIPlayer)player;
            gameState = gameState.PlayCard(aiPlayer.GetBestPlayableCard());

            gameState.turnCount++;
            gameState.currentPlayerTurn = (gameState.currentPlayerTurn + 1) % PLAYER_AMOUNT;
            // TODO : Update game visuals here
        }
    }
}
