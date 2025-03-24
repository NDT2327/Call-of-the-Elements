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

        SceneManager.LoadScene(currentMap.ToString());
        ResetCondition();
    }

    public void OnBossDefeated()
    {
        bossDefeated = true;
        CheckMapCompletion();
    }

    private void CheckMapCompletion()
    {
        if (bossDefeated)
        {
            OnMapCompleted(currentMap);
        }
    }

    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpoint = position;
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
        Debug.Log("RestartGame() called in GameManager");
        StartCoroutine(RestartScene());
    }


    private IEnumerator RestartScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitUntil(() => operation.isDone);
        Debug.Log("Checkpoint loaded at position: " + lastCheckpoint);
        // Sau khi load xong, đặt lại vị trí người chơi
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = lastCheckpoint ;
            Debug.Log("Player repositioned to: " + player.transform.position);
        }
    }

    private void EndGame()
    {
        Debug.Log("Elementia restored");
    }
}
