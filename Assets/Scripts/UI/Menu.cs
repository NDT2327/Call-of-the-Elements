using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void LoadGame()
    {
        GameManager.Instance.LoadGame();
    }
    public void Play()
    {
        GameManager.Instance.NewGame();
    }

    // Update is called once per frame
    public void Control()
    {
        SceneManager.LoadScene("Control");
    }

    public void Main()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Quit()
    {
        Application.Quit();
    }

}
