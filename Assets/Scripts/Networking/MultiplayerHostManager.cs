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
            Debug.LogWarning("[MultiplayerHostManager] Un serveur est d�j� actif !");
            return;
        }

        Debug.Log("[MultiplayerHostManager] D�marrage de l'h�bergement...");
        manager.multiplayerStatus.text = "H�bergement en cours...";
        manager.StartHost();

        if (NetworkServer.active)
        {
            Debug.Log("[MultiplayerHostManager] Serveur d�marr� avec succ�s !");
        }
        else
        {
            Debug.LogError("[MultiplayerHostManager] �chec du d�marrage du serveur !");
        }
    }

    public void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject player = Instantiate(manager.playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player);
        Debug.Log("[MultiplayerHostManager] Nouveau joueur ajout� !");

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
            Debug.Log("[MultiplayerHostManager] Plus d'un joueur pr�sent, activation du bouton 'Lancer la partie'.");
            manager.LaunchMultiplayerBTN.gameObject.SetActive(true);
        }
    }
}
