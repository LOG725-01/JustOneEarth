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

    // Liste synchronisée des joueurs (seul l'hôte peut la modifier)
    public SyncList<string> playerNames = new SyncList<string>();

    // Type de lobby récupéré depuis `MultiplayerManager`
    private LobbyType lobbyType = LobbyType.None;
    private MultiplayerManager multiplayerManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("[LobbyManager] Instance créée avec succès.");
        }
        else
        {
            Debug.LogWarning("[LobbyManager] Une autre instance existe déjà, destruction de l'objet.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Récupère `MultiplayerManager`
        multiplayerManager = FindObjectOfType<MultiplayerManager>();

        if (multiplayerManager == null)
        {
            Debug.LogError("[LobbyManager] Impossible de trouver MultiplayerManager !");
            return;
        }

        // Détermine le type de lobby en fonction du mode de connexion
        if (multiplayerManager.mode == NetworkManagerMode.Host)
        {
            lobbyType = LobbyType.Host;
            Debug.Log("[LobbyManager] Mode défini : HÔTE.");
        }
        else if (multiplayerManager.mode == NetworkManagerMode.ClientOnly)
        {
            lobbyType = LobbyType.Client;
            Debug.Log("[LobbyManager] Mode défini : CLIENT.");
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("[LobbyManager] Serveur démarré, écoute des joueurs connectés.");
        lobbyType = LobbyType.Host;
        playerNames.Callback += OnPlayerListUpdated;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        Debug.Log("[LobbyManager] Serveur arrêté, suppression des callbacks.");
        lobbyType = LobbyType.None;
        playerNames.Callback -= OnPlayerListUpdated;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("[LobbyManager] Client connecté au lobby.");
        lobbyType = LobbyType.Client;
    }

    public void AddPlayer(string playerName)
    {
        if (lobbyType == LobbyType.Host) // Seul l'hôte peut modifier la liste des joueurs
        {
            Debug.Log($"[LobbyManager] Ajout du joueur : {playerName}");
            playerNames.Add(playerName);
            UpdatePlayerList();
        }
        else
        {
            Debug.LogWarning("[LobbyManager] AddPlayer() appelé sur un client, mais seul l'hôte peut gérer les joueurs !");
        }
    }

    private void OnPlayerListUpdated(SyncList<string>.Operation op, int index, string oldItem, string newItem)
    {
        Debug.Log($"[LobbyManager] Mise à jour de la liste des joueurs - Opération: {op}, Index: {index}, Ancien: {oldItem}, Nouveau: {newItem}");
        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        if (playerListTMP != null)
        {
            Debug.Log("[LobbyManager] Mise à jour de l'affichage des joueurs.");
            playerListTMP.text = "Joueurs connectés :\n" + string.Join("\n", playerNames);
        }
        else
        {
            Debug.LogError("[LobbyManager] playerListTMP est NULL ! Assurez-vous qu'il est bien assigné dans l'Inspector.");
        }
    }
}
