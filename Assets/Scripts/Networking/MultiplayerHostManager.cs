using UnityEngine;
using Mirror;

public class MultiplayerHostManager : MonoBehaviour
{
    private MultiplayerManager manager;

    public void Initialize(MultiplayerManager mgr)
    {
        manager = mgr;
        HostGame();
    }

    public void HostGame()
    {
        if (NetworkClient.active || NetworkServer.active)
        {
            Debug.LogWarning("[MultiplayerHostManager] Un serveur est déjà actif !");
            return;
        }

        Debug.Log("[MultiplayerHostManager] Démarrage de l'hébergement...");
        manager.multiplayerStatus.text = "Hébergement en cours...";
        manager.StartHost();

        if (NetworkServer.active)
        {
            Debug.Log("[MultiplayerHostManager] Serveur démarré avec succès !");
        }
        else
        {
            Debug.LogError("[MultiplayerHostManager] Échec du démarrage du serveur !");
        }
    }

    public void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject player = Instantiate(manager.playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player);
        Debug.Log("[MultiplayerHostManager] Nouveau joueur ajouté !");

        if (LobbyManager.Instance != null)
        {
            string playerName = "Joueur " + manager.numPlayers;
            Debug.Log("[MultiplayerHostManager] Ajout du joueur : " + playerName);
            LobbyManager.Instance.AddPlayer(playerName);
        }
        else
        {
            Debug.LogError("[MultiplayerHostManager] LobbyManager.Instance est null !");
        }

        if (manager.numPlayers > 1 && manager.LaunchMultiplayerBTN != null)
        {
            Debug.Log("[MultiplayerHostManager] Plus d'un joueur présent, activation du bouton 'Lancer la partie'.");
            manager.LaunchMultiplayerBTN.gameObject.SetActive(true);
        }
    }
}
