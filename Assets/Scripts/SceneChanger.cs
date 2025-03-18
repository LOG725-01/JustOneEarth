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
            Debug.LogWarning("[SceneChanger] Une autre instance existe déjà, suppression de cet objet.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Détacher de son parent avant de le rendre persistant
        transform.SetParent(null);

        DontDestroyOnLoad(gameObject); // Préserve SceneChanger entre les scènes
        Debug.Log("[SceneChanger] Instance créée et préservée entre les scènes.");

        if (currentScene == "")
            currentScene = homeScene;
    }

    private void Start()
    {
        Debug.Log("[SceneChanger] Start() - Scène actuelle : " + currentScene);

        if (currentScene != homeScene)
        {
            GameObject gameManagerObject = Instantiate(gameManagerPrefab);
            Scene targetScene = SceneManager.GetSceneByName(gameScene);
            SceneManager.MoveGameObjectToScene(gameManagerObject, targetScene);
            Debug.Log("[SceneChanger] GameManager ajouté à la scène : " + gameManagerObject.scene.name);

            gameManagerPrefab.GetComponent<GameManager>().StartGame();
        }

        if (SceneManager.GetActiveScene().name == "MultiplayerLobby")
        {
            Debug.Log("[SceneChanger] Lobby détecté, suppression de la persistance.");
            Destroy(gameObject); // Supprime l'objet SceneChanger après le chargement du Lobby
        }
    }

    static private void SceneChange(string scene)
    {
        currentScene = scene;
        SceneManager.LoadScene(scene);
    }

    public void GameStart()
    {
        Debug.Log("[SceneChanger] Démarrage du jeu, passage à " + gameScene);
        CurrentMode = MultiplayerMode.None;
        SceneChange(gameScene);
    }

    public void SetHostMode()
    {
        Debug.Log("[SceneChanger] Mode défini : Host");
        CurrentMode = MultiplayerMode.Host;
        SceneManager.LoadScene("MultiplayerLobby");
    }

    public void SetClientMode()
    {
        Debug.Log("[SceneChanger] Mode défini : Client");
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
