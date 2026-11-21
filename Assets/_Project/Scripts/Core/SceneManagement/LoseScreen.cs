using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoseScreen : MonoBehaviour
{
    [SerializeField] private Button mainMenue;
    [SerializeField] private Button exit;

    public void Start()
    {
        mainMenue.onClick.AddListener(OnMainMenueWasClickes);
        exit.onClick.AddListener(OnExitWasClicked);
    }

    private void OnMainMenueWasClickes()
    {
        SceneManager.LoadScene("StartScreen");
    }

    private void OnExitWasClicked()
    {
        Application.Quit();
    }
}
