using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameMode gameMode;

    GameState gameState;

    [SerializeField] GameObject cardPrefab;

    [SerializeField] private HumanPlayer humanPlayerPrefab;
    [SerializeField] private AIPlayer aiPlayerPrefab;


    private HumanPlayer humanPlayerInstance;
    private AIPlayer aiPlayerInstance;

    [SerializeField] public Board board;
    [SerializeField] private GameObject cloudSpawnerPrefab;

    private List<PlayerInputNotifier> playerInputNotifiers = new List<PlayerInputNotifier>();

    private bool gameStarted = false;

    public void setGameMode(GameMode gameMode)
    {
        this.gameMode = gameMode;
    }

    private void Start()
    {
        if (board == null)
            board = FindObjectOfType<Board>();
        StartGame();
    }


    public void StartGame()
    {
        if (board != null)
        {
            Board boardObject = Instantiate(board);
            board = boardObject.GetComponent<Board>();
        }

        if (cloudSpawnerPrefab != null)
        {
            GameObject cloudSpawnerObject = Instantiate(cloudSpawnerPrefab, board.transform);
            CloudSpawner cloudSpawner = cloudSpawnerObject.GetComponent<CloudSpawner>();

            cloudSpawner.Initialize(board);
        }

        gameState = new GameObject("GameState").AddComponent<GameState>();
        gameState.SetBoard(board);
        var observers = FindObjectsOfType<Observer>();


        humanPlayerInstance = Instantiate(humanPlayerPrefab);
        aiPlayerInstance = Instantiate(aiPlayerPrefab);

        Player player = humanPlayerInstance;


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

        PopulatePlayerDeck();

        DrawCardToHand(humanPlayerInstance);

        playerInputNotifiers.Clear();
        playerInputNotifiers.AddRange(FindObjectsOfType<PlayerInputNotifier>());

        foreach (var notifier in playerInputNotifiers)
        {
            notifier.OnGameObjectClicked += HandlePlayerInput;
        }

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

    private void Update()
    {

        Player player = gameState.getCurrentPlayingPlayer();

        if (player != null)
            if (player.GetType() == typeof(AIPlayer))
            {

                AIPlayer aiPlayer = (AIPlayer)player;
                gameState = gameState.PlayCard(aiPlayer.GetBestPlayableCard());

                gameState.turnCount++;
                gameState.SetCurrentPlayerTurnToNextPlayer();

            }
            else if (player is HumanPlayer)
            {

            }
    }

    private void DrawCardToHand(Player player)
    {
        if (player.deck.Count > 0)
        {
            System.Random random = new System.Random();
            int randomIndex = random.Next(0, player.deck.Count);

            Card drawnCard = player.deck[randomIndex];

            player.MoveCardFromDeckToHand(drawnCard);

            GameObject hand = GameObject.Find("CardHand");
            drawnCard.gameObject.transform.SetParent(hand.transform, false);
        }
    }

    private Card CreateCardInDeck(CardData cardData)
    {
        GameObject cardHand = GameObject.Find("Deck");
        Card card = Instantiate(cardPrefab, cardHand.transform).GetComponent<Card>();

        card.InitializeCard(cardData.cardName, cardData.description, 
            cardData.effectList, cardData.cost);

        return card;
    }

    private void PopulatePlayerDeck()
    {
        for (int i = 0; i < 10; i++)
        {
            CardData cardData = ScriptableObject.CreateInstance<Card01>();
            Card card = CreateCardInDeck(cardData);
            humanPlayerInstance.AddCardInDeck(card);
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
            Debug.Log($"[GameManager] Ressource de depart : {kvp.Key} = {initialAmount}");
        }

        player.NotifyObservers();
    }

    private void AssignStartingTiles(Player player, List<Tile> tiles, int count)
    {
        Debug.Log($"[GameManager] Assignation des tuiles Total disponibles : {tiles.Count}");

        var unowned = tiles.FindAll(t => t != null && t.owner == null);

        Debug.Log($"[GameManager] Tuiles sans proprietaire : {unowned.Count}");

        if (unowned.Count == 0)
        {
            Debug.LogWarning("[GameManager] Aucune tuile unowned trouvee !");
            return;
        }

        Debug.Log($"[AssignStartingTiles] Total tiles en entree : {tiles.Count}");

        foreach (var tile in tiles)
        {
            Debug.Log($"[AssignStartingTiles] Tuile : {tile.name}, type: {tile.tileType}, owner: {(tile.owner == null ? "Aucun" : tile.owner.name)}");
        }

        for (int i = 0; i < count && unowned.Count > 0; i++)
        {
            var tile = unowned[UnityEngine.Random.Range(0, unowned.Count)];

            if (tile == null)
            {
                Debug.LogWarning("[GameManager] Tuile null rencontree !");
                continue;
            }

            player.AddOwnedTile(tile); // Appelle log dans Player.cs
            tile.owner = player;

            Debug.Log($"[GameManager] Tuile assignee : {tile.name}, Type : {tile.tileType}");

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

        player.NotifyObservers();
    }
}