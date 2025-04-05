using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct DebugValues
{
    public bool gameManager;
    public bool board;
    public bool animal;
    public bool tile;
    public bool player;
    public bool gameState;
    public bool cloud;
   
    public DebugValues(bool _)
    {
        gameManager = true;
        board = true;
        animal = false;
        tile = true;
        player = true;
        gameState = true;
        cloud = false;
    }
}

public class GameManager : MonoBehaviour
{
    private PlayerType playerType;

    private GameState gameState;

    [SerializeField] GameObject cardPrefab;

    [SerializeField] private HumanPlayer humanPlayerPrefab;
    [SerializeField] private AIPlayer aiPlayerPrefab;

    [SerializeField] private GameObject deckPrefab;
    [SerializeField] private GameObject discardPrefab;
    [SerializeField] private GameObject handPrefab;

    private HumanPlayer humanPlayerInstance;
    private AIPlayer aiPlayerInstance;

    [SerializeField] public Board board;
    [SerializeField] private GameObject cloudSpawnerPrefab;

    private InGameMenu inGameMenu;
    private bool isAiTurn = false;
    private bool isMenuOpen = false;

    private List<PlayerInputNotifier> playerInputNotifiers = new List<PlayerInputNotifier>();

    private bool gameStarted = false;

    DebugValues debugValues;

    private System.Type[] availableCardTypes = new System.Type[]
    {
        typeof(GetOnePointCard),
        typeof(FreeCard)
        // Ajoutez les cartes ici
    };

    private void Start()
    {
        if (board == null)
            board = FindObjectOfType<Board>();
        StartGame();
        inGameMenu = FindObjectOfType<InGameMenu>();
    }

    private void Update()
    {
        if (!isAiTurn) { 
        Player player = gameState.GetCurrentPlayingPlayer();

        if (player != null)
            if (player.GetType() == typeof(AIPlayer))
            {
                isAiTurn = true;
                StartCoroutine(AITurn());
            }
            else if (player is HumanPlayer)
            {

            }
        }
        if (Input.GetButtonDown("Cancel"))
        {
            HandleEscapePress();
        }
    }

    private IEnumerator AITurn()
    {
        yield return new WaitForSeconds(2);
        Player player = gameState.GetCurrentPlayingPlayer();
        AIPlayer aiPlayer = (AIPlayer)player;
        gameState = gameState.PlayCard(aiPlayer.GetBestPlayableCard(), aiPlayerInstance);
        gameState.DrawCardToHand(player);
        isAiTurn = false;
    }

    private void HandleEscapePress()
    {
        if (inGameMenu != null)
        {
            inGameMenu.ToggleMenu();
            isMenuOpen = inGameMenu.IsMenuOpen();
        }
    }
    public void StartGame()
    {
        playerType = SceneChanger.PlayerType;

        PlayerTurnUi.Instance.SetTurn(playerType);

        humanPlayerInstance = Instantiate(humanPlayerPrefab);
        humanPlayerInstance.name = "humanPlayer";
        humanPlayerInstance.PlayerType = playerType;

        GameObject humanPlayerDeck = Instantiate(deckPrefab);
        humanPlayerDeck.transform.SetParent(humanPlayerInstance.gameObject.transform);
        GameObject humanPlayerHand = Instantiate(handPrefab);
        humanPlayerHand.transform.SetParent(humanPlayerInstance.gameObject.transform);
        GameObject humanPlayerDiscard = Instantiate(discardPrefab);
        humanPlayerDiscard.transform.SetParent(humanPlayerInstance.gameObject.transform);

        humanPlayerInstance.debug = debugValues.player;
        aiPlayerInstance = Instantiate(aiPlayerPrefab);
        aiPlayerInstance.name = "aiPlayer";

        GameObject aiPlayerDeck = Instantiate(deckPrefab);
        aiPlayerDeck.transform.SetParent(aiPlayerInstance.gameObject.transform);
        GameObject aiPlayerHand = Instantiate(handPrefab);
        aiPlayerHand.transform.SetParent(aiPlayerInstance.gameObject.transform);
        GameObject aiPlayerDiscard = Instantiate(discardPrefab);
        aiPlayerDiscard.transform.SetParent(aiPlayerInstance.gameObject.transform);

        if (board != null)
        {
            Board boardObject = Instantiate(board);
            board = boardObject.GetComponent<Board>();
            board.debug = debugValues.board;
            board.debugAnimal = debugValues.animal;
            board.debugTile = debugValues.tile;
            board.OnBoardGenerated += () =>
            {
                if (debugValues.gameManager) Debug.Log($"[OnBoardGenerated] Invoker");
                var allTiles = board.GetAllTiles();
                InitializePlayerStartingResources(humanPlayerInstance, allTiles);
                AssignStartingTiles(humanPlayerInstance, allTiles, 3);
                RegisterObserversToPlayer(humanPlayerInstance);
            };
            board.CreateBoard();
        }

        if (cloudSpawnerPrefab != null)
        {
            GameObject cloudSpawnerObject = Instantiate(cloudSpawnerPrefab, board.transform);
            CloudSpawner cloudSpawner = cloudSpawnerObject.GetComponent<CloudSpawner>();
            cloudSpawner.debug = debugValues.cloud;
            cloudSpawner.Initialize(board);
        }

        gameState = new GameObject("GameState").AddComponent<GameState>();
        gameState.SetBoard(board);
        PlayerInputDetection.Instance.GameState = gameState;

        switch (playerType)
        {
            case PlayerType.Civilisation:
                aiPlayerInstance.PlayerType = PlayerType.World;
                gameState.PlayerCivilisation = humanPlayerInstance;
                gameState.PlayerWorld = aiPlayerInstance;
                break;
            case PlayerType.World:
                aiPlayerInstance.PlayerType = PlayerType.Civilisation;
                gameState.PlayerCivilisation = aiPlayerInstance;
                gameState.PlayerWorld = humanPlayerInstance;
                break;
        }
        gameState.SetFirstPlayer(playerType);
        gameState.currentInstancePlayer = humanPlayerInstance;
        gameState.debug = debugValues.gameState;

        PopulateDeck(humanPlayerDeck, humanPlayerInstance);
        PopulateDeck(aiPlayerDeck, aiPlayerInstance);

        gameState.DrawCardToHand(humanPlayerInstance);
        gameState.DrawCardToHand(aiPlayerInstance);


        playerInputNotifiers.Clear();
        playerInputNotifiers.AddRange(FindObjectsOfType<PlayerInputNotifier>(true));

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
            return;
        }
        else
        {
            DeselectCurrentTile();
        }
        Debug.Log("[HandlePlayerInput] Clicked object has no specific click behavior.");
    }

    private void PopulateDeck(GameObject deck, Player player)
    {
        System.Random random = new System.Random();

        for (int i = 0; i < 20; i++)
        {
            // Sélectionne un type aléatoire parmi les cartes disponibles
            int index = random.Next(availableCardTypes.Length);
            System.Type cardType = availableCardTypes[index];

            // Crée dynamiquement une instance du type sélectionné
            CardData cardData = ScriptableObject.CreateInstance(cardType) as CardData;

            Card card = gameState.CreateCardGameObject(cardData, deck, cardPrefab);
            player.AddCardInDeck(card);
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
            if (debugValues.gameManager) Debug.Log($"[GameManager] Ressource de depart : {kvp.Key} = {initialAmount}");
        }

        player.NotifyObservers();
    }

    private void AssignStartingTiles(Player player, List<Tile> tiles, int count)
    {
        if (debugValues.gameManager) Debug.Log($"[GameManager] Assignation des tuiles Total disponibles : {tiles.Count}");

        var unowned = tiles.FindAll(t => t != null && t.owner == null);

        if (debugValues.gameManager) Debug.Log($"[GameManager] Tuiles sans proprietaire : {unowned.Count}");

        if (unowned.Count == 0)
        {
            Debug.LogWarning("[GameManager] Aucune tuile unowned trouvee !");
            return;
        }

        if (debugValues.gameManager) Debug.Log($"[AssignStartingTiles] Total tiles en entree : {tiles.Count}");

        foreach (var tile in tiles)
        {
            if (debugValues.gameManager) Debug.Log($"[AssignStartingTiles] Tuile : {tile.name}, type: {tile.tileType}, owner: {(tile.owner == null ? "Aucun" : tile.owner.name)}");
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

            if (debugValues.gameManager) Debug.Log($"[GameManager] Tuile assignee : {tile.name}, Type : {tile.tileType}");

            unowned.Remove(tile);
        }

        if (debugValues.gameManager) Debug.Log($"[GameManager] Tuiles finales du joueur : {player.ownedTiles.Count}");

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
    private void DeselectCurrentTile()
    {
        gameState.currentInstancePlayer.DeselectTile();
        TileInfo.Instance.Clear();
    }
}