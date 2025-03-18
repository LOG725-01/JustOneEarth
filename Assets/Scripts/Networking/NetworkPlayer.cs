using Mirror;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SyncVar] public string playerName;

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log($"[NetworkPlayer] Joueur connect� : {playerName}");
    }

    [Command] // Cette fonction est appel�e par le client et ex�cut�e sur le serveur
    public void CmdSetPlayerName(string newName)
    {
        playerName = newName;
    }
}
