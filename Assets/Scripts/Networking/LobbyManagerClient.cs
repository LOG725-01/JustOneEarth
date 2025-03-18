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
            Debug.Log("[LobbyManagerClient] Instance créée avec succès.");
        }
        else
        {
            Debug.LogWarning("[LobbyManagerClient] Une autre instance existe déjà, destruction de l'objet.");
            Destroy(gameObject);
        }
    }

    public void UpdatePlayerList(string[] playerNames)
    {
        if (playerListTMP != null)
        {
            Debug.Log("[LobbyManagerClient] Mise à jour de l'affichage des joueurs.");
            playerListTMP.text = "Joueurs connectés :\n" + string.Join("\n", playerNames);
        }
        else
        {
            Debug.LogError("[LobbyManagerClient] playerListTMP est NULL ! Assurez-vous qu'il est bien assigné dans l'Inspector.");
        }
    }
}
