using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerCustom : MonoBehaviour
{
	public static SceneManagerCustom instance;
	private string lastLoadedScene;

	private void Awake()
	{
		if (instance == null) instance = this;
		else Destroy(gameObject);
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.name != lastLoadedScene)
		{
			PlayMusicForScene(scene.name);
			lastLoadedScene = scene.name;
		}
	}

	private void PlayMusicForScene(string sceneName)
	{
		switch (sceneName)
		{
			case "Lava":
				AudioManager.instance.PlayBackgroundMusic(GameManager.Map.Lava);
				break;
			case "Earth":
				AudioManager.instance.PlayBackgroundMusic(GameManager.Map.Earth);
				break;
			case "Castle":
				AudioManager.instance.PlayBackgroundMusic(GameManager.Map.Castle);
				break;
			default:
				AudioManager.instance.PlayMainBackgroundMusic();
				break;
		}
	}

	public void LoadScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

	public void ReloadCurrentScene()
	{
		string currentScene = SceneManager.GetActiveScene().name;
		SceneManager.LoadScene(currentScene);
	}

}
