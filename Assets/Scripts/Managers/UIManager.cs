using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance;

	[SerializeField] private GameObject gameOverPanel;
	[SerializeField] protected GameObject pauseGamePanel;
	[SerializeField] private GameObject notification;

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
		if(notification != null) notification.SetActive(false);
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			TogglePause();
		}
	}

	public void ShowNotification()
	{
		if (notification != null) StartCoroutine(ShowNotificationRoutine());
	}

	private IEnumerator ShowNotificationRoutine()
	{
		notification.SetActive(true);
		yield return new WaitForSecondsRealtime(2f);
		notification.SetActive(false);
	}

	public void ShowGameOverScreen()
	{
		AudioManager.instance.PlayGameOverMusic();

		gameOverPanel.SetActive(true);
		pauseGamePanel.SetActive(false);
		Time.timeScale = 0f;
	}

	public void TogglePause()
	{
		//unable toggle if game over is on
		if (gameOverPanel.activeInHierarchy) {
			return;
		}
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
		if (player != null)
		{
			GameManager.Instance.RestartGame();
			gameOverPanel.SetActive(false);
		}

	}

	public void LoadMainMenu()
	{
		AudioManager.instance.PlayClickButtonSound();
		Time.timeScale = 1f;
		SceneManager.LoadScene("Menu");

	}

	public void SaveGame()
	{
		AudioManager.instance.PlayClickButtonSound();
		GameManager.Instance.LoadGame();
	}
}
