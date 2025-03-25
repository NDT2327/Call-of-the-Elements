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
        gameOverPanel.SetActive
            (true);
        Time.timeScale = 0f;
        AudioManager.instance.PlayGameOverMusic();

    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseGamePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
        AudioManager.instance.PlayClickButtonSound();

    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {
            player.GetComponent<Health>().RestartFromCheckpoint();
            GameManager.Instance.RestartGame();
            gameOverPanel.SetActive (false);
        }
        AudioManager.instance.PlayClickButtonSound();

    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
        AudioManager.instance.PlayClickButtonSound();

    }
}
