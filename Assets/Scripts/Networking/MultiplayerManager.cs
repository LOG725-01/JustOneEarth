using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

public class MultiplayerManager : NetworkManager
{
    public TMP_Text multiplayerStatus; // Associe ce champ à "multiplayer_status" dans l'Inspector
    public Button HOST_BTN;
    public Button JOIN_BTN;

    private float connectionTimeout = 300f; // Temps max avant d'abandonner une connexion (5 min)
    private float connectionTimer;
    private bool isJoining = false;

    public void HostGame()
    {

        if (NetworkClient.active || NetworkServer.active) return; // Empêche plusieurs connexions

        if (IsServerAlreadyRunning()) // Vérifie si un serveur est déjà actif
        {
            multiplayerStatus.text = "Un serveur est déjà en cours.";
            return;
        }

        multiplayerStatus.text = "Hébergement en cours...";

        HOST_BTN.interactable = false; // Désactive le bouton
        JOIN_BTN.interactable = false;

        StartHost();
    }

    public void JoinGame()
    {
        if (NetworkClient.active || NetworkServer.active) return; // Empêche plusieurs connexions

        multiplayerStatus.text = "Connexion en cours...";

        HOST_BTN.interactable = false; // Désactive le bouton
        JOIN_BTN.interactable = false;

        isJoining = true;
        connectionTimer = connectionTimeout;
        // Forcer la connexion au serveur local
        networkAddress = "127.0.0.1";
        StartClient();
    }

    public override void OnClientConnect()
    {

        base.OnClientConnect();

        HOST_BTN.interactable = false; // Désactive le bouton
        JOIN_BTN.interactable = false;

        // Vérifie si ce client est l'hôte
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            multiplayerStatus.text = "Serveur hébergé avec succès !";
        }
        else
        {
            multiplayerStatus.text = "Connecté avec succès !";
        }

        isJoining = false;
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        HOST_BTN.interactable = true; // Réactive le bouton après déconnexion
        JOIN_BTN.interactable = true;

        multiplayerStatus.text = "Déconnecté du serveur.";
    }
    public override void Update()
    {
        if (isJoining)
        {
            connectionTimer -= Time.deltaTime;
            if (connectionTimer <= 0)
            {
                StopClient();
                multiplayerStatus.text = "Échec de connexion (timeout).";
                isJoining = false;

                // Réactiver les boutons en cas d'échec
                HOST_BTN.interactable = true;
                JOIN_BTN.interactable = true;
            }
        }
    }

    /// <summary>
    /// Vérifie si un serveur est déjà actif sur le réseau en essayant de se connecter à lui.
    /// </summary>
    /// <returns>True si un serveur est trouvé, False sinon.</returns>
    private bool IsServerAlreadyRunning()
    {
        string localIP = GetLocalIPAddress();
        int port = transport is TelepathyTransport ? ((TelepathyTransport)transport).port : 7777;

        try
        {
            using (TcpClient client = new TcpClient())
            {
                client.Connect(localIP, port);
                return true; // Une connexion a réussi, un serveur est donc actif
            }
        }
        catch (SocketException)
        {
            return false; // Aucune connexion possible, aucun serveur actif
        }
    }

    /// <summary>
    /// Récupère l'adresse IP locale de la machine.
    /// </summary>
    /// <returns>Adresse IP sous forme de string.</returns>
    private string GetLocalIPAddress()
    {
        foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (netInterface.OperationalStatus == OperationalStatus.Up &&
                netInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            {
                foreach (UnicastIPAddressInformation ip in netInterface.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.Address.ToString();
                    }
                }
            }
        }
        return "127.0.0.1"; // Valeur par défaut si aucune adresse locale trouvée
    }
}
