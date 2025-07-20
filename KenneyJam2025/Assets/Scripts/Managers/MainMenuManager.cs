using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject mainMenuCanvas;
    public GameObject levelSelectionCanvas;
    
    [Header("Main Menu Buttons")]
    public Button playButton;
    public Button quitButton;

    [Header("Level Selection Buttons")]
    public Button backButton;
    public Button[] levelButtons;
    
    void Start()
    {
        SetupButtonListeners();
        ShowMainMenu();
    }

    public void LaunchScene(int index)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager not there");
            return;
        }
            
        string sceneName = "Level" + index;
        GameManager.Instance.LoadScene(sceneName);
    }
    
    private void SetupButtonListeners()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClicked);
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        ShowLevelSelection();
    }

    private void OnQuitButtonClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void OnBackButtonClicked()
    {
        ShowMainMenu();
    }

    private void ShowMainMenu()
    {
        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(true);
            
        if (levelSelectionCanvas != null)
            levelSelectionCanvas.SetActive(false);
    }

    private void ShowLevelSelection()
    {
        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(false);
            
        if (levelSelectionCanvas != null)
            levelSelectionCanvas.SetActive(true);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (GameManager.Instance != null)
            {
                if (i < GameManager.Instance.MaxLevel + 1) 
                    levelButtons[i].interactable = true;
                else
                    levelButtons[i].interactable = false;
            }
                    
                
        }
    }
}
