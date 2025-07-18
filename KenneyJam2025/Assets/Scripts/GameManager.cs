using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene Management")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string launcherSceneName = "Launcher";

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
            LoadMainMenu();
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Example method for loading other scenes
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
