using UnityEngine;
using Mirror;
using TMPro;

public class LobbyManager : NetworkBehaviour
{   
    public enum LobbyType
    {
        None,
        Host,
        Client
    }

    [Header("UI Elements")]
    public TMP_Text playerListTMP;
    public static LobbyManager Instance;

    // Liste synchronis�e des joueurs (seul l'h�te peut la modifier)
    public SyncList<string> playerNames = new SyncList<string>();

    // Type de lobby r�cup�r� depuis `MultiplayerManager`
    private LobbyType lobbyType = LobbyType.None;
    private MultiplayerManager multiplayerManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("[LobbyManager] Instance cr��e avec succ�s.");
        }
        else
        {
            Debug.LogWarning("[LobbyManager] Une autre instance existe d�j�, destruction de l'objet.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // R�cup�re `MultiplayerManager`
        multiplayerManager = FindObjectOfType<MultiplayerManager>();

        if (multiplayerManager == null)
        {
            Debug.LogError("[LobbyManager] Impossible de trouver MultiplayerManager !");
            return;
        }

        // D�termine le type de lobby en fonction du mode de connexion
        if (multiplayerManager.mode == NetworkManagerMode.Host)
        {
            lobbyType = LobbyType.Host;
            Debug.Log("[LobbyManager] Mode d�fini : H�TE.");
        }
        else if (multiplayerManager.mode == NetworkManagerMode.ClientOnly)
        {
            lobbyType = LobbyType.Client;
            Debug.Log("[LobbyManager] Mode d�fini : CLIENT.");
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("[LobbyManager] Serveur d�marr�, �coute des joueurs connect�s.");
        lobbyType = LobbyType.Host;
        playerNames.Callback += OnPlayerListUpdated;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        Debug.Log("[LobbyManager] Serveur arr�t�, suppression des callbacks.");
        lobbyType = LobbyType.None;
        playerNames.Callback -= OnPlayerListUpdated;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("[LobbyManager] Client connect� au lobby.");
        lobbyType = LobbyType.Client;
    }

    public void AddPlayer(string playerName)
    {
        if (lobbyType == LobbyType.Host) // Seul l'h�te peut modifier la liste des joueurs
        {
            Debug.Log($"[LobbyManager] Ajout du joueur : {playerName}");
            playerNames.Add(playerName);
            UpdatePlayerList();
        }
        else
        {
            Debug.LogWarning("[LobbyManager] AddPlayer() appel� sur un client, mais seul l'h�te peut g�rer les joueurs !");
        }
    }

    private void OnPlayerListUpdated(SyncList<string>.Operation op, int index, string oldItem, string newItem)
    {
        Debug.Log($"[LobbyManager] Mise � jour de la liste des joueurs - Op�ration: {op}, Index: {index}, Ancien: {oldItem}, Nouveau: {newItem}");
        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        if (playerListTMP != null)
        {
            Debug.Log("[LobbyManager] Mise � jour de l'affichage des joueurs.");
            playerListTMP.text = "Joueurs connect�s :\n" + string.Join("\n", playerNames);
        }
        else
        {
            Debug.LogError("[LobbyManager] playerListTMP est NULL ! Assurez-vous qu'il est bien assign� dans l'Inspector.");
        }
    }
}
