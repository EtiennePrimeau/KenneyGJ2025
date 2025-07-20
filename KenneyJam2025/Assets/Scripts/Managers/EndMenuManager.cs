using UnityEngine;
using UnityEngine.UI;

public class EndMenuManager : MonoBehaviour
{
    public Button button;
    void Start()
    {
        button.onClick.AddListener(OnPlayAgainButtonClicked);
    }

    private void OnPlayAgainButtonClicked()
    {
        GameManager.Instance.RestartLevels();
    }
}
