using UnityEngine;
using TMPro;

public class LobbyManagerClient : MonoBehaviour
{
    public static LobbyManagerClient Instance;
    public TMP_Text playerListTMP;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("[LobbyManagerClient] Instance cr��e avec succ�s.");
        }
        else
        {
            Debug.LogWarning("[LobbyManagerClient] Une autre instance existe d�j�, destruction de l'objet.");
            Destroy(gameObject);
        }
    }

    public void UpdatePlayerList(string[] playerNames)
    {
        if (playerListTMP != null)
        {
            Debug.Log("[LobbyManagerClient] Mise � jour de l'affichage des joueurs.");
            playerListTMP.text = "Joueurs connect�s :\n" + string.Join("\n", playerNames);
        }
        else
        {
            Debug.LogError("[LobbyManagerClient] playerListTMP est NULL ! Assurez-vous qu'il est bien assign� dans l'Inspector.");
        }
    }
}
