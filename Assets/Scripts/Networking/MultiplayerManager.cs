using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

public class MultiplayerManager : NetworkManager
{
    public TMP_Text multiplayerStatus; // Associe ce champ � "multiplayer_status" dans l'Inspector
    public Button HOST_BTN;
    public Button JOIN_BTN;

    private float connectionTimeout = 300f; // Temps max avant d'abandonner une connexion (5 min)
    private float connectionTimer;
    private bool isJoining = false;

    public void HostGame()
    {

        if (NetworkClient.active || NetworkServer.active) return; // Emp�che plusieurs connexions

        if (IsServerAlreadyRunning()) // V�rifie si un serveur est d�j� actif
        {
            multiplayerStatus.text = "Un serveur est d�j� en cours.";
            return;
        }

        multiplayerStatus.text = "H�bergement en cours...";

        HOST_BTN.interactable = false; // D�sactive le bouton
        JOIN_BTN.interactable = false;

        StartHost();
    }

    public void JoinGame()
    {
        if (NetworkClient.active || NetworkServer.active) return; // Emp�che plusieurs connexions

        multiplayerStatus.text = "Connexion en cours...";

        HOST_BTN.interactable = false; // D�sactive le bouton
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

        HOST_BTN.interactable = false; // D�sactive le bouton
        JOIN_BTN.interactable = false;

        // V�rifie si ce client est l'h�te
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            multiplayerStatus.text = "Serveur h�berg� avec succ�s !";
        }
        else
        {
            multiplayerStatus.text = "Connect� avec succ�s !";
        }

        isJoining = false;
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        HOST_BTN.interactable = true; // R�active le bouton apr�s d�connexion
        JOIN_BTN.interactable = true;

        multiplayerStatus.text = "D�connect� du serveur.";
    }
    public override void Update()
    {
        if (isJoining)
        {
            connectionTimer -= Time.deltaTime;
            if (connectionTimer <= 0)
            {
                StopClient();
                multiplayerStatus.text = "�chec de connexion (timeout).";
                isJoining = false;

                // R�activer les boutons en cas d'�chec
                HOST_BTN.interactable = true;
                JOIN_BTN.interactable = true;
            }
        }
    }

    /// <summary>
    /// V�rifie si un serveur est d�j� actif sur le r�seau en essayant de se connecter � lui.
    /// </summary>
    /// <returns>True si un serveur est trouv�, False sinon.</returns>
    private bool IsServerAlreadyRunning()
    {
        string localIP = GetLocalIPAddress();
        int port = transport is TelepathyTransport ? ((TelepathyTransport)transport).port : 7777;

        try
        {
            using (TcpClient client = new TcpClient())
            {
                client.Connect(localIP, port);
                return true; // Une connexion a r�ussi, un serveur est donc actif
            }
        }
        catch (SocketException)
        {
            return false; // Aucune connexion possible, aucun serveur actif
        }
    }

    /// <summary>
    /// R�cup�re l'adresse IP locale de la machine.
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
        return "127.0.0.1"; // Valeur par d�faut si aucune adresse locale trouv�e
    }
}
