using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameScene : MonoBehaviour
{
    public Text victoryText;
    public Button mainMenuButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.instance.PlayVictoryMusic();
        mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    void GoToMainMenu()
    {
        SceneManager.LoadScene("Menu"); 
    }
}
