using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button endButton;

    public void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
        endButton.onClick.AddListener(OnExitButtonClick);
    }

    private void OnStartButtonClicked()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void OnExitButtonClick()
    {
        Application.Quit();
    }
}
