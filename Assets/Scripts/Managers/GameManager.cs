using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //public EnemyManager enemyManager;
    //public UIManager uiManager;
    //public AudioManager audioManager;

    private Vector3 lastCheckpoint;

    public enum Map { Earth, Lava, Castle }
    public Map currentMap = Map.Earth;

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
        //audioManager = GetComponent<AudioManager>();
    }

    public void OnMapCompleted(Map completedMap)
    {
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            player.RecoverHealthAndStamina(0.5f); // Hồi 50% máu và stamina
            player.UnlockSpecialAttack(completedMap); // Mở khóa SpAttack mới
        }
        if (completedMap == Map.Earth) currentMap = Map.Earth;
        else if (completedMap == Map.Lava) currentMap = Map.Lava;
        else if (completedMap == Map.Castle) EndGame();

        //enemyManager.Initialize(currentMap);
        
    }

    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpoint = position;
    }

    public Vector3 GetCheckpoint()
    {
        return lastCheckpoint;
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
        if (player != null)
        {
            player.transform.position = lastCheckpoint ;
        }
    }

    private void EndGame()
    {
        Debug.Log("Elementia restored");
    }
}
