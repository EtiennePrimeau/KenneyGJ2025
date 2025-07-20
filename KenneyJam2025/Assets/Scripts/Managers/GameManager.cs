using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene Management")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string launcherSceneName = "Launcher";
    [SerializeField] private string endMenuSceneName = "EndMenu";
    [SerializeField] private string winMenuSceneName = "WinMenu";
    
    [Header("Audio Management")]
    [SerializeField] private GameAudioManager gameAudioManager;

    private int maxLevel = 0;
    public int MaxLevel => maxLevel;

    private float timer = 0f;
    public float timerAdd = 5f;
    public float timerStart = 20f;
    private bool timerRunning = false;
    public TextMeshProUGUI timerText;

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

    private void Update()
    {
        if (timerRunning)
        {
            timer -= Time.deltaTime;
            timerText.text = timer.ToString("F2");
            if (timer < 0f)
                EndGame();
        }
    }

    private void EndGame()
    {
        Debug.Log("Game Over");
        SceneManager.LoadScene(endMenuSceneName);
        timerRunning = false;
        timerText.gameObject.SetActive(false);
    }

    public void RestartLevels()
    {
        LoadMainMenu(false);
    }
    
    private void LoadMainMenu(bool isLaunching = false)
    {
        timer = 0f;
        timerText.gameObject.SetActive(false);
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
        timerRunning = true;
        timer = timerStart;
        timerText.gameObject.SetActive(true);
        if (gameAudioManager.State != GameAudioManager.GameState.Game)
            gameAudioManager.SetGameState(GameAudioManager.GameState.Game);
    }

    public void ReloadLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void FinishLevel(int finishedLevel)
    {
        timer += timerAdd;
        if (finishedLevel > maxLevel)
            maxLevel =  finishedLevel;

        if (finishedLevel == 6) // last level
        {
            timerRunning = false;
            timerText.gameObject.SetActive(false);
            SceneManager.LoadScene(winMenuSceneName);
            return;
        }
        
        string sceneName = "Level" + (finishedLevel + 1);
        Debug.Log(sceneName);
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
