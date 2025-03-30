using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance;
    private static string currentScene = "";

    private static PlayerType playerType = PlayerType.Civilisation;

    public static PlayerType PlayerType {  get { return playerType; } }

    [SerializeField] private string gameScene, homeScene;
    [SerializeField] private GameObject gameManagerPrefab;    

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

    public void GameStartCivilisation()
    {
        playerType = PlayerType.Civilisation;
        GameStart();
    }

    public void GameStartWorld()
    {
        playerType = PlayerType.World;
        GameStart();
    }

    public void GameStart()
    {
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
