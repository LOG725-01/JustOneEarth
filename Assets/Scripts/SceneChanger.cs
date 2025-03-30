using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance;
    private static string currentScene = "";

    [SerializeField] private string gameScene, homeScene;
    [SerializeField] private GameObject gameManagerPrefab;

    // TODO : gameMode must be changed dynamically when game starts using the selected mode from the interface
    private GameMode gameMode = GameMode.PVE;

    private void Awake()
    {
        if (Instance != null) { if (Instance != this) Destroy(this); }
        else Instance = this;
        if (currentScene == "") currentScene = homeScene;
    }

    static private void SceneChange(string scene)
    {
        currentScene = scene;
        SceneManager.LoadScene(scene);
    }

    public void GameStart()
    {
        // TODO : fix gameManagerObject creation
        SceneChange(gameScene);
        
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
