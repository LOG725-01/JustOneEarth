using Mirror;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SyncVar] public string playerName;

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log($"[NetworkPlayer] Joueur connecté : {playerName}");
    }

    [Command] // Cette fonction est appelée par le client et exécutée sur le serveur
    public void CmdSetPlayerName(string newName)
    {
        playerName = newName;
    }
}
