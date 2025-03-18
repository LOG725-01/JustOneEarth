using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class LobbyManagerHost : NetworkBehaviour
{
    public static LobbyManagerHost Instance;
    public TMP_Text playerListTMP;

    // Liste synchronisée des joueurs côté serveur
    public SyncList<string> playerNames = new SyncList<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("[LobbyManagerHost] Instance créée avec succès.");
        }
        else
        {
            Debug.LogWarning("[LobbyManagerHost] Une autre instance existe déjà, destruction de l'objet.");
            Destroy(gameObject);
        }
    }
    private void OnPlayerListUpdated(SyncList<string>.Operation op, int index, string oldItem, string newItem)
    {
        Debug.Log($"[LobbyManagerHost] Mise à jour de la liste des joueurs - Opération: {op}, Index: {index}, Ancien: {oldItem}, Nouveau: {newItem}");
        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        if (playerListTMP != null)
        {
            Debug.Log("[LobbyManagerHost] Mise à jour de l'affichage des joueurs.");
            playerListTMP.text = "Joueurs connectés :\n" + string.Join("\n", playerNames);
        }
        else
        {
            Debug.LogError("[LobbyManagerHost] playerListTMP est NULL ! Assurez-vous qu'il est bien assigné dans l'Inspector.");
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("[LobbyManagerHost] Serveur démarré, écoute des joueurs connectés.");
        playerNames.Callback += OnPlayerListUpdated;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        Debug.Log("[LobbyManagerHost] Serveur arrêté, suppression des callbacks.");
        playerNames.Callback -= OnPlayerListUpdated;
    }

    public void AddPlayer(string playerName)
    {
        if (isServer) // Vérifie que seul le serveur gère la liste
        {
            Debug.Log($"[LobbyManagerHost] Ajout du joueur : {playerName}");
            playerNames.Add(playerName);
            RpcUpdatePlayerList(playerNames.ToArray()); // Envoi aux clients
        }
        else
        {
            Debug.LogWarning("[LobbyManagerHost] AddPlayer() appelé sur un client, mais seul le serveur peut gérer les joueurs !");
        }
    }

    [Server]
    public void RemovePlayer(string playerName)
    {
        if (playerNames.Contains(playerName))
        {
            Debug.Log($"[LobbyManagerHost] Suppression du joueur : {playerName}");
            playerNames.Remove(playerName);
            RpcUpdatePlayerList(playerNames.ToArray()); // Mise à jour côté clients
        }
    }

    [ClientRpc]
    private void RpcUpdatePlayerList(string[] updatedList)
    {
        if (LobbyManagerClient.Instance != null)
        {
            LobbyManagerClient.Instance.UpdatePlayerList(updatedList);
        }
    }
}
