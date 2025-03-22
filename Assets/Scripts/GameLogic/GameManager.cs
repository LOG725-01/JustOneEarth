using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameMode gameMode;

    GameState gameState;

    [SerializeField] private Board board;
    [SerializeField] private HumanPlayer humanPlayerPrefab;
    [SerializeField] private AIPlayer aiPlayerPrefab;

    private HumanPlayer humanPlayerInstance;
    private AIPlayer aiPlayerInstance;

    private List<PlayerInputNotifier> playerInputNotifiers = new List<PlayerInputNotifier>();

    private bool gameStarted = false;

    public void setGameMode(GameMode gameMode)
    {
        this.gameMode = gameMode;
    }

    private void Awake()
    {
        if (board == null)
            board = FindObjectOfType<Board>();
    }


    public void StartGame()
    {
        gameState = new GameObject("GameState").AddComponent<GameState>();
        gameState.SetBoard(board);
        var observers = FindObjectsOfType<Observer>();


        // Instanciation dans la scène
        humanPlayerInstance = Instantiate(humanPlayerPrefab);
        aiPlayerInstance = Instantiate(aiPlayerPrefab);

        Player player = humanPlayerInstance;

        // Passage à GameState
        gameState.players.Add(humanPlayerInstance);
        gameState.players.Add(aiPlayerInstance);

        gameState.currentInstancePlayer = humanPlayerInstance;

        board.OnBoardGenerated += () =>
        {
            Debug.Log($"[OnBoardGenerated] Invoker");
            var allTiles = board.GetAllTiles();
            InitializePlayerStartingResources(humanPlayerInstance, allTiles);
            AssignStartingTiles(humanPlayerInstance, allTiles, 3);
            RegisterObserversToPlayer(humanPlayerInstance);
        };

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

    private void InitializePlayerStartingResources(Player player, List<Tile> tiles)
    {
        Dictionary<RessourceTypes, int> totalResources = new();

        foreach (RessourceTypes type in System.Enum.GetValues(typeof(RessourceTypes)))
            totalResources[type] = 0;

        foreach (var tile in tiles)
        {
            foreach (var kvp in tile.producedRessources)
                totalResources[kvp.Key] += kvp.Value;
        }

        foreach (var kvp in totalResources)
        {
            int initialAmount = Mathf.FloorToInt(kvp.Value * 0.1f);
            player.currentRessources[kvp.Key] = initialAmount;
            Debug.Log($"[GameManager] Ressource de départ : {kvp.Key} = {initialAmount}");
        }

        player.NotifyObservers();
    }

    private void AssignStartingTiles(Player player, List<Tile> tiles, int count)
    {
        Debug.Log($"[GameManager] Assignation des tuiles — Total disponibles : {tiles.Count}");

        var unowned = tiles.FindAll(t => t != null && t.owner == null);

        Debug.Log($"[GameManager] Tuiles sans propriétaire : {unowned.Count}");

        if (unowned.Count == 0)
        {
            Debug.LogWarning("[GameManager] Aucune tuile unowned trouvée !");
            return;
        }

        Debug.Log($"[AssignStartingTiles] Total tiles en entrée : {tiles.Count}");

        foreach (var tile in tiles)
        {
            Debug.Log($"[AssignStartingTiles] Tuile : {tile.name}, type: {tile.tileType}, owner: {(tile.owner == null ? "Aucun" : tile.owner.name)}");
        }

        for (int i = 0; i < count && unowned.Count > 0; i++)
        {
            var tile = unowned[UnityEngine.Random.Range(0, unowned.Count)];

            if (tile == null)
            {
                Debug.LogWarning("[GameManager] Tuile null rencontrée !");
                continue;
            }

            player.AddOwnedTile(tile); // Appelle log dans Player.cs
            tile.owner = player;

            Debug.Log($"[GameManager] Tuile assignée : {tile.name}, Type : {tile.tileType}");

            unowned.Remove(tile);
        }

        Debug.Log($"[GameManager] Tuiles finales du joueur : {player.ownedTiles.Count}");

        player.ComputeRessources();
    }


    void RegisterObserversToPlayer(Player player)
    {
        Observer[] observers = FindObjectsOfType<Observer>();

        foreach (var obs in observers)
        {
            player.RegisterObserver(obs);
        }

        player.NotifyObservers(); // Force un premier affichage
    }
}
