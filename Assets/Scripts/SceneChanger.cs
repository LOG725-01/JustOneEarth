using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance;

    [SerializeField] private string gameScene, homeScene;
    [SerializeField] private GameObject gameManagerPrefab;

    // TODO : gameMode must be changed dynamically when game starts using the selected mode from the interface
    private GameMode gameMode = GameMode.PVE;

    private void Awake()
    {
        if (Instance != null) { if (Instance != this) Destroy(this); }
        else Instance = this;
    }

    static private void SceneChange(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void GameStart()
    {
        // TODO : fix gameManagerObject creation
        SceneChange(gameScene);
        GameObject gameManagerObject = Instantiate(gameManagerPrefab);

        // Move the spawned object to the correct scene
        Scene targetScene = SceneManager.GetSceneByName(gameScene);
        SceneManager.MoveGameObjectToScene(gameManagerObject, targetScene);

        Debug.Log(gameManagerObject.scene.name);

        gameManagerPrefab.GetComponent<GameManager>().setGameMode(gameMode);
        gameManagerPrefab.GetComponent<GameManager>().StartGame();
    }

    public void Home()
    {
        SceneChange(homeScene);
    }

    public void QuitApp()
    {
        Application.Quit();
    }

}
