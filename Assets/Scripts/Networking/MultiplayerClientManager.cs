using UnityEngine;
using Mirror;

public class MultiplayerClientManager : MonoBehaviour
{
    private MultiplayerManager manager;
    private float connectionTimeout = 10f;
    private float connectionTimer;
    private bool isJoining = false;

    public void Initialize(MultiplayerManager mgr)
    {
        manager = mgr;
        JoinGame();
    }

    public void JoinGame()
    {
        if (NetworkClient.active || NetworkServer.active)
        {
            Debug.LogWarning("[MultiplayerClientManager] D�j� connect� � un serveur !");
            return;
        }

        Debug.Log("[MultiplayerClientManager] Connexion au serveur...");
        manager.multiplayerStatus.text = "Connexion en cours...";
        isJoining = true;
        connectionTimer = connectionTimeout;
        manager.StartClient();
    }

    private void Update()
    {
        if (isJoining)
        {
            connectionTimer -= Time.deltaTime;
            if (connectionTimer <= 0)
            {
                Debug.LogError("[MultiplayerClientManager] �chec de connexion : timeout !");
                manager.StopClient();
                manager.multiplayerStatus.text = "�chec de connexion (timeout).";
                isJoining = false;
            }
        }
    }
}
