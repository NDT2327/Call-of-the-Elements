using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject loadGameButton;

	private void Start()
	{
		UpdateLoadGameButton();
	}
	private void UpdateLoadGameButton()
    {
        if(loadGameButton != null)
        {
            bool hasSavedData = PlayerPrefs.HasKey("CurrentMap");
            loadGameButton.SetActive(hasSavedData);
        }
    }

    public void LoadGame()
    {
		AudioManager.instance.PlayClickButtonSound();
		GameManager.Instance.LoadGame();
    }
    public void Play()
    {
		if (AudioManager.instance == null)
		{
			Debug.LogError("AudioManager.instance is null!");
			return;
		}
		AudioManager.instance.PlayClickButtonSound();
		GameManager.Instance.NewGame();
    }

    // Update is called once per frame
    public void Control()
    {
        AudioManager.instance.PlayClickButtonSound();
        SceneManager.LoadScene("Control");
    }

    public void Main()
    {
		AudioManager.instance.PlayClickButtonSound();
		SceneManager.LoadScene("Menu");
    }

    public void Quit()
    {
		AudioManager.instance.PlayClickButtonSound();
		Application.Quit();
    }

}
