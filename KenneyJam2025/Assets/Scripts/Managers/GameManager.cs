using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene Management")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string launcherSceneName = "Launcher";
    
    [Header("Audio Management")]
    [SerializeField] private GameAudioManager gameAudioManager;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGameManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGameManager()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == launcherSceneName)
            LoadMainMenu(true);
    }

    private void LoadMainMenu(bool isLaunching = false)
    {
        SceneManager.LoadScene(mainMenuSceneName);
        gameAudioManager.SetGameState(GameAudioManager.GameState.Menu, true);
    }

    public void LoadScene(string sceneName)
    {
        if (sceneName == mainMenuSceneName)
        {
            LoadMainMenu();
            return;
        }
        
        SceneManager.LoadScene(sceneName);
        if (gameAudioManager.State != GameAudioManager.GameState.Game)
            gameAudioManager.SetGameState(GameAudioManager.GameState.Game);
    }

    public void ReloadLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
