using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //public EnemyManager enemyManager;
    //public UIManager uiManager;
    public AudioManager audioManager;

    private Vector3 lastCheckpoint;

    public enum Map { Earth, Lava, Castle }
    public Map currentMap = Map.Earth;

    private bool bossDefeated = false;
    private bool skillCollected = false;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //uiManager = GetComponent<UIManager>();
        //enemyManager = GetComponent<EnemyManager>();
        audioManager = GetComponent<AudioManager>();
    }

    public void OnMapCompleted(Map completedMap)
    {
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            player.RecoverHealthAndStamina(1f); // Hồi 50% máu và stamina
        }

        if(completedMap == Map.Castle && bossDefeated)
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
        else
        {
            if (spawnPoint == null) Debug.LogWarning("SpawnPoint not found in scene: " + sceneName);
            if (player == null) Debug.LogWarning("Player not found in scene: " + sceneName);
        }

        AudioManager.instance.PlayBackgroundMusic(currentMap);
        ResetCondition();
    }

    public void OnBossDefeated()
    {
        bossDefeated = true;
        AudioManager.instance.PlayBossDefeatedSound();
        Debug.Log("Terrible Knight has been defeated!");

        CheckMapCompletion();
    }

    private void CheckMapCompletion()
    {

        OnMapCompleted(currentMap);

    }

    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpoint = position;
        AudioManager.instance.PlayCheckpointSound();
        Debug.Log("Checkpoint saved at: " + lastCheckpoint);

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
        AudioManager.instance.PlayBackgroundMusic(currentMap);
    }

    private void EndGame()
    {
        SceneManager.LoadScene("EndGame");
    }

    //save game
    public void SaveGame()
    {
        PlayerPrefs.SetInt("CurrentMap", (int) currentMap);
        PlayerPrefs.SetFloat("CheckpointX", lastCheckpoint.x);
        PlayerPrefs.SetFloat("CheckpointY", lastCheckpoint.y);
        PlayerPrefs.SetFloat("CheckpointZ", lastCheckpoint.z);
        PlayerPrefs.SetInt("bossDefeated", bossDefeated ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("Save game");
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
            Debug.Log("Game Loaded!");
        }
    }

    //new game
    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
        currentMap = Map.Earth;
        lastCheckpoint = Vector3.zero;
        bossDefeated = false;
        SceneManager.LoadScene("Story");
        Debug.Log("New Game started");
    }
}
