using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }


	public Vector3 lastCheckpoint;

	public enum Map { Earth, Lava, Castle }
	public Map currentMap = Map.Earth;

	private bool bossDefeated = false;
	private bool skillCollected = false;

	public bool hasEarthItem = false;
	public bool hasLavaItem = false;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void OnMapCompleted(Map completedMap)
	{
		Player player = FindFirstObjectByType<Player>();
		if (player != null)
		{
			player.RecoverHealthAndStamina(1f); // Hồi 50% máu và stamina
		}

		if (completedMap == Map.Castle && bossDefeated)
		{
			EndGame();
			return;
		}
		switch (completedMap)
		{
			case Map.Earth:
				currentMap = Map.Lava; // Chuyển từ Earth sang Lava
				break;
			case Map.Lava:
				currentMap = Map.Castle; // Chuyển từ Lava sang Castle
				break;
			default:
				Debug.LogWarning("Unknown map completed: " + completedMap);
				return;
		}
		StartCoroutine(LoadMapWithSpawn(currentMap.ToString())); // Dùng coroutine để load và đặt spawn 
	}


	private IEnumerator LoadMapWithSpawn(string sceneName)
	{
		AudioManager.instance.PlayBackgroundMusic(currentMap);

		// Load scene bất đồng bộ
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
		yield return new WaitUntil(() => operation.isDone);

		// Tìm điểm spawn trong scene mới
		GameObject spawnPoint = GameObject.FindGameObjectWithTag("Respawn");
		GameObject player = GameObject.FindGameObjectWithTag("Player");

		if (spawnPoint != null && player != null)
		{
			player.transform.position = spawnPoint.transform.position;
			Debug.Log("Player spawned at: " + spawnPoint.transform.position + " in map: " + sceneName);
		}

		ResetCondition();
	}

	public void OnBossDefeated()
	{
		AudioManager.instance.PlayBossDefeatedSound();

		bossDefeated = true;
		Debug.Log("Terrible Knight has been defeated!");

		CheckMapCompletion();
	}

	private void CheckMapCompletion()
	{

		OnMapCompleted(currentMap);

	}

	public void OnSkillCollected(string gemType)
	{
		skillCollected = true;
		Debug.Log("Skill item collected: " + gemType);

		// Cập nhật trạng thái ngọc
		if (gemType == "EarthGem")
			hasEarthItem = true;
		else if (gemType == "FireGem")
			hasLavaItem = true;

		CheckMapCompletion(); // Kiểm tra để chuyển map
	}

	public void SetCheckpoint(Vector3 position)
	{
		AudioManager.instance.PlayCheckpointSound();

		lastCheckpoint = position;
		Debug.Log("Checkpoint saved at: " + lastCheckpoint);
		SaveGame();

	}

	public Vector3 GetCheckpoint()
	{
		return lastCheckpoint;
	}

	private void ResetCondition()
	{
		bossDefeated = false;
	}

	public void RestartGame()
	{
		StartCoroutine(RestartScene());
	}


	private IEnumerator RestartScene()
	{
		AudioManager.instance.PlayBackgroundMusic(currentMap);
		string sceneName = SceneManager.GetActiveScene().name;
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
		yield return new WaitUntil(() => operation.isDone);

		// Sau khi load xong, đặt lại vị trí người chơi
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		GameObject spawnPoint = GameObject.FindGameObjectWithTag("Respawn");


		if (player != null)
		{
			if (lastCheckpoint == Vector3.zero && spawnPoint != null)
			{
				player.transform.position = spawnPoint.transform.position;
			}
			player.transform.position = lastCheckpoint;
		}
	}

	private void EndGame()
	{
		SceneManager.LoadScene("EndGame");
	}

	//save game
	public void SaveGame()
	{
		PlayerPrefs.SetInt("CurrentMap", (int)currentMap);
		PlayerPrefs.SetFloat("CheckpointX", lastCheckpoint.x);
		PlayerPrefs.SetFloat("CheckpointY", lastCheckpoint.y);
		PlayerPrefs.SetFloat("CheckpointZ", lastCheckpoint.z);
		PlayerPrefs.SetInt("BossDefeated", bossDefeated ? 1 : 0);
		PlayerPrefs.SetInt("SkillCollected", skillCollected ? 1 : 0);
		PlayerPrefs.SetInt("HasEarthGem", hasEarthItem ? 1 : 0);
		PlayerPrefs.SetInt("HasFireGem", hasLavaItem ? 1 : 0);
		PlayerPrefs.Save();
		Debug.Log("Game saved");
	}

	//load game
	public void LoadGame()
	{
		if (PlayerPrefs.HasKey("CurrentMap"))
		{
			currentMap = (Map)PlayerPrefs.GetInt("CurrentMap");
			lastCheckpoint = new Vector3(
				PlayerPrefs.GetFloat("CheckpointX"),
				PlayerPrefs.GetFloat("CheckpointY"),
				PlayerPrefs.GetFloat("CheckpointZ")
				);
			bossDefeated = PlayerPrefs.GetInt("bossDefeated") == 1;
			skillCollected = PlayerPrefs.GetInt("SkillCollected") == 1;
			hasEarthItem = PlayerPrefs.GetInt("HasEarthGem") == 1;
			hasLavaItem = PlayerPrefs.GetInt("HasFireGem") == 1;

			string sceneToLoad = currentMap switch
			{
				Map.Earth => Map.Earth.ToString(),
				Map.Lava => Map.Lava.ToString(),
				Map.Castle => Map.Castle.ToString()
			};
			SceneManagerCustom.instance.LoadScene(sceneToLoad);
			Debug.Log("Game Loaded!");
		}
		else
		{
			Debug.LogWarning("No saved game found, starting new game.");
			NewGame();
		}
	}

	//new game
	public void NewGame()
	{
		PlayerPrefs.DeleteAll();
		currentMap = Map.Earth;
		lastCheckpoint = Vector3.zero;
		bossDefeated = false;
		skillCollected = false;
		hasEarthItem = false;
		hasLavaItem = false;
		SceneManager.LoadScene("Story");
		Debug.Log("New Game started");
	}
}
