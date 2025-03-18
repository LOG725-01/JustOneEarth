using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class SceneReturnManager : MonoBehaviour
{
    public void HardResetToMainMenu()
    {
        Debug.Log("[SceneReturnManager] R�initialisation compl�te...");

        // D�connecte le client s'il est actif
        if (NetworkClient.isConnected)
        {
            NetworkClient.Disconnect();
        }

        // Arr�te le serveur s'il est actif
        if (NetworkServer.active)
        {
            NetworkManager.singleton.StopHost();
        }

        // Supprime le NetworkManager
        if (NetworkManager.singleton != null)
        {
            Destroy(NetworkManager.singleton.gameObject);
        }

        // Recharge TOUT le jeu comme au premier lancement
        SceneManager.LoadScene("MainMenu");
    }
}
