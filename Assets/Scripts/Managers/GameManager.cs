using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }


	public Vector3 lastCheckpoint = Vector3.zero;

	public enum Map { Earth, Lava, Castle }
	public Map currentMap = Map.Earth;

	private bool bossDefeated = false;
	private bool taurusDefeated = false; // Cờ cho Taurus trong Castle
	private bool conquerorDefeated = false; // Cờ cho The Conqueror trong Castle
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

		if (completedMap == Map.Castle)
		{
			if (taurusDefeated && conquerorDefeated)
			{
				Debug.Log("Both Taurus and The Conqueror defeated, ending game.");
				EndGame();
				return;
			}
			else if (taurusDefeated)
			{
				Debug.Log("Taurus defeated, proceed to face The Conqueror.");
				return; // Không chuyển scene, tiếp tục trong Castle
			}
		}

		// Chuyển map cho Earth và Lava nếu mini-boss bị đánh bại
		if (bossDefeated)
		{
			switch (completedMap)
			{
				case Map.Earth:
					currentMap = Map.Lava;
					break;
				case Map.Lava:
					currentMap = Map.Castle;
					break;
				default:
					Debug.LogWarning("Unknown map completed: " + completedMap);
					return;
			}
			StartCoroutine(LoadMapWithSpawn(currentMap.ToString()));
		}
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
			SetCheckpoint(spawnPoint.transform.position);
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
		if (AudioManager.instance != null)
		{
			AudioManager.instance.PlayBackgroundMusic(currentMap);
		}

		string sceneName = SceneManager.GetActiveScene().name;
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
		yield return new WaitUntil(() => operation.isDone);

		GameObject player = GameObject.FindGameObjectWithTag("Player");
		GameObject spawnPoint = GameObject.FindGameObjectWithTag("Respawn");

		if (player != null)
		{
			if (lastCheckpoint == Vector3.zero && spawnPoint != null)
			{
				player.transform.position = spawnPoint.transform.position;
				Debug.Log("Player respawned at spawn point: " + spawnPoint.transform.position);
			}
			else if (lastCheckpoint != Vector3.zero)
			{
				player.transform.position = lastCheckpoint;
				Debug.Log("Player respawned at last checkpoint: " + lastCheckpoint);
			}
			else
			{
				Debug.LogWarning("No valid spawn point or checkpoint found!");
			}
		}
		else
		{
			Debug.LogWarning("Player not found in scene!");
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
