using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class MultiplayerManager : NetworkManager
{
    [Header("UI Elements")]
    public TMP_Text multiplayerStatus;
    public Button LaunchMultiplayerBTN;

    private void Start()
    {
        if (FindObjectsOfType<NetworkManager>().Length > 1)
        {
            Debug.LogWarning("[MultiplayerManager] Une autre instance détectée, suppression...");
            Destroy(gameObject);
            return;
        }

        Debug.Log("[MultiplayerManager] Start() - Initialisation");

        if (SceneChanger.Instance == null)
        {
            Debug.LogError("[MultiplayerManager] SceneChanger.Instance est null !");
            return;
        }

        Debug.Log("[MultiplayerManager] Mode sélectionné : " + SceneChanger.Instance.CurrentMode);

        if (SceneChanger.Instance.CurrentMode == MultiplayerMode.Host)
        {
            gameObject.AddComponent<MultiplayerHostManager>().Initialize(this);
        }
        else if (SceneChanger.Instance.CurrentMode == MultiplayerMode.Client)
        {
            gameObject.AddComponent<MultiplayerClientManager>().Initialize(this);
        }

        if (LaunchMultiplayerBTN != null)
        {
            Debug.Log("[MultiplayerManager] Configuration du bouton 'Lancer la partie'.");
            LaunchMultiplayerBTN.gameObject.SetActive(false);
            LaunchMultiplayerBTN.onClick.RemoveAllListeners();
            LaunchMultiplayerBTN.onClick.AddListener(StartMultiplayerGame);
        }
        else
        {
            Debug.LogWarning("[MultiplayerManager] LaunchMultiplayerBTN est null !");
        }
    }

    public void StartMultiplayerGame()
    {
        if (NetworkServer.active)
        {
            Debug.Log("[MultiplayerManager] Démarrage de la partie multijoueur...");
            SceneManager.LoadScene("MultiplayerGame");
        }
        else
        {
            Debug.LogWarning("[MultiplayerManager] StartMultiplayerGame() appelé, mais ce n'est pas un serveur !");
        }
    }
}
