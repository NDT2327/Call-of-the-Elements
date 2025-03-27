using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] protected GameObject pauseGamePanel;

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        gameOverPanel.SetActive(false);
        pauseGamePanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void ShowGameOverScreen()
    {
		AudioManager.instance.PlayGameOverMusic();

		gameOverPanel.SetActive
            (true);
        Time.timeScale = 0f;
    }

    public void TogglePause()
    {
		AudioManager.instance.PlayClickButtonSound();

		isPaused = !isPaused;
        pauseGamePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;

    }

    public void RestartGame()
    {
		AudioManager.instance.PlayClickButtonSound();

		Time.timeScale = 1f;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {
            player.GetComponent<Health>().RestartFromCheckpoint();
            GameManager.Instance.RestartGame();
            gameOverPanel.SetActive (false);
        }

    }

    public void LoadMainMenu()
    {
		AudioManager.instance.PlayClickButtonSound();
		Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");

    }
}
