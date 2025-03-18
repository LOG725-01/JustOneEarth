using UnityEngine;
using UnityEngine.SceneManagement;

public enum MultiplayerMode { None, Host, Client }

public class SceneChanger : MonoBehaviour
{
    public MultiplayerMode CurrentMode { get; private set; } = MultiplayerMode.None;
    public static SceneChanger Instance;
    private static string currentScene = "";

    [SerializeField] private string gameScene, homeScene;
    [SerializeField] private GameObject gameManagerPrefab;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("[SceneChanger] Une autre instance existe d�j�, suppression de cet objet.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // D�tacher de son parent avant de le rendre persistant
        transform.SetParent(null);

        DontDestroyOnLoad(gameObject); // Pr�serve SceneChanger entre les sc�nes
        Debug.Log("[SceneChanger] Instance cr��e et pr�serv�e entre les sc�nes.");

        if (currentScene == "")
            currentScene = homeScene;
    }

    private void Start()
    {
        Debug.Log("[SceneChanger] Start() - Sc�ne actuelle : " + currentScene);

        if (currentScene != homeScene)
        {
            GameObject gameManagerObject = Instantiate(gameManagerPrefab);
            Scene targetScene = SceneManager.GetSceneByName(gameScene);
            SceneManager.MoveGameObjectToScene(gameManagerObject, targetScene);
            Debug.Log("[SceneChanger] GameManager ajout� � la sc�ne : " + gameManagerObject.scene.name);

            gameManagerPrefab.GetComponent<GameManager>().StartGame();
        }

        if (SceneManager.GetActiveScene().name == "MultiplayerLobby")
        {
            Debug.Log("[SceneChanger] Lobby d�tect�, suppression de la persistance.");
            Destroy(gameObject); // Supprime l'objet SceneChanger apr�s le chargement du Lobby
        }
    }

    static private void SceneChange(string scene)
    {
        currentScene = scene;
        SceneManager.LoadScene(scene);
    }

    public void GameStart()
    {
        Debug.Log("[SceneChanger] D�marrage du jeu, passage � " + gameScene);
        CurrentMode = MultiplayerMode.None;
        SceneChange(gameScene);
    }

    public void SetHostMode()
    {
        Debug.Log("[SceneChanger] Mode d�fini : Host");
        CurrentMode = MultiplayerMode.Host;
        SceneManager.LoadScene("MultiplayerLobby");
    }

    public void SetClientMode()
    {
        Debug.Log("[SceneChanger] Mode d�fini : Client");
        CurrentMode = MultiplayerMode.Client;
        SceneManager.LoadScene("MultiplayerLobby");
    }

    public void Home()
    {
        Debug.Log("[SceneChanger] Retour au menu principal.");
        SceneChange(homeScene);
    }

    public void QuitApp()
    {
        Debug.Log("[SceneChanger] Fermeture de l'application.");
        Application.Quit();
    }
}
