using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

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
        typeof(MineralSurge),
        typeof(WoodSurge),
        typeof(WaterSurge),
        typeof(SuperPointBurst)
        // Ajoutez les cartes ici
    };

    private System.Type[] civilisationCardTypes = new System.Type[]
    {
        typeof(CreateVillage),
        typeof(CreateDam),
        typeof(OilRefinery),
        typeof(SolarFarm),
        typeof(MechanicalBurst)
        // Ajoute ici toutes les cartes Civilisation
    };

    private System.Type[] worldCardTypes = new System.Type[]
    {
        // Ajoute ici les cartes World (attaque, effets climatiques, etc.)
        typeof (Earthquake),
        typeof (NaturalBurst),
        typeof (SpiritOfTheLake),
        typeof (SpiritOfTheMountains)
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
        if (Chat.Instance != null && Chat.Instance.IsOpened())
        {
            Chat.Instance.Close();
            EventSystem.current.SetSelectedGameObject(null); // enlève le focus
            return;
        }

        Player currentPlayer = gameState.currentInstancePlayer;
        if (currentPlayer.selectedTile != null)
        {
            DeselectCurrentTile();
            return;
        }
        if (inGameMenu != null)
        {
            inGameMenu.ToggleMenu();
            isMenuOpen = inGameMenu.IsMenuOpen();
        }
    }
    public void StartGame()
    {
        playerType = SceneChanger.PlayerType;

        // set background color depending on the plyer type chosen
        switch (playerType)
        {
            case PlayerType.Civilisation:
                Camera.main.backgroundColor = new Color(0.54f, 0.6f, 0.65f);
                break;
            case PlayerType.World:
                Camera.main.backgroundColor = new Color(0.188f, 0.54f, 0.81f);
                break;
        }
        

        PlayerTurnUi.Instance.SetTurn(playerType);

        humanPlayerInstance = Instantiate(humanPlayerPrefab);
        humanPlayerInstance.name = "humanPlayer";
        humanPlayerInstance.PlayerType = playerType;

        RessourceButtonHandler.Instance.Player = humanPlayerInstance;

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
        gameState = new GameObject("GameState").AddComponent<GameState>();
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
                gameState.PlayerWorld = humanPlayerInstance;
                gameState.PlayerCivilisation = aiPlayerInstance;
                break;
        }
        gameState.SetFirstPlayer(playerType);
        gameState.currentInstancePlayer = humanPlayerInstance;
        gameState.debug = debugValues.gameState;

        PopulateDeck(humanPlayerDeck, humanPlayerInstance);
        PopulateDeck(aiPlayerDeck, aiPlayerInstance);

        AddPersistentCardToHand(humanPlayerInstance);

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
        List<System.Type> combinedCardTypes = new();

        // Ajouter les cartes communes
        combinedCardTypes.AddRange(availableCardTypes);

        // Ajouter les cartes spécifiques en fonction du type du joueur
        if (player.PlayerType == PlayerType.Civilisation)
            combinedCardTypes.AddRange(civilisationCardTypes);
        else if (player.PlayerType == PlayerType.World)
            combinedCardTypes.AddRange(worldCardTypes);

        // Génération aléatoire du deck
        System.Random random = new();
        for (int i = 0; i < 20; i++)
        {
            int index = random.Next(combinedCardTypes.Count);
            System.Type cardType = combinedCardTypes[index];

            CardData cardData = ScriptableObject.CreateInstance(cardType) as CardData;
            Card card = gameState.CreateCardGameObject(cardData, deck, cardPrefab);
            player.AddCardInDeck(card);
        }
    }

    private void AddPersistentCardToHand(Player player)
    {
        CardData freeCardData = ScriptableObject.CreateInstance<FreeCard>();

        Transform handTransform = (player is HumanPlayer)
            ? GameObject.Find("PlayerHand").transform
            : player.transform.Find("Hand(Clone)");

        Card freeCard = gameState.CreateCardGameObject(freeCardData, handTransform.gameObject, cardPrefab);

        player.hand.Add(freeCard);
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

        if (unowned.Count == 0)
        {
            Debug.LogWarning("[GameManager] Aucune tuile unowned trouvée !");
            return;
        }

        // Dictionnaire temporaire pour stocker les types déjà assignés
        HashSet<TileType> usedTypes = new HashSet<TileType>();

        int assigned = 0;

        while (assigned < count && unowned.Count > 0)
        {
            // Trouver une tuile non assignée avec un type encore non utilisé
            Tile tile = unowned.Find(t => !usedTypes.Contains(t.tileType));

            if (tile == null)
            {
                Debug.LogWarning("[GameManager] Plus de types uniques disponibles pour assignation.");
                break; // on ne peut plus garantir des types différents
            }

            // Assigner la tuile au joueur
            tile.owner = player;
            player.AddOwnedTile(tile);
            usedTypes.Add(tile.tileType);
            unowned.Remove(tile);
            assigned++;

            if (debugValues.gameManager) Debug.Log($"[GameManager] Tuile assignée : {tile.name}, Type : {tile.tileType}");
        }

        player.ComputeRessources(gameState);
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