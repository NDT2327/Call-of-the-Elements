using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //public EnemyManager enemyManager;
    //public UIManager uiManager;
    //public AudioManager audioManager;

    private Vector2 lastCheckpoint;

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

    public void OnMapCompleted(Map complatedMap)
    {
        if (complatedMap == Map.Earth) currentMap = Map.Earth;
        else if (complatedMap == Map.Lava) currentMap = Map.Lava;
        else if (complatedMap == Map.Castle) EndGame();

        //enemyManager.Initialize(currentMap);
        
    }

    public void SetCheckpoint(Vector2 position)
    {
        lastCheckpoint = position;
    }

    public Vector2 GetCheckpoint()
    {
        return lastCheckpoint;
    }

    private void EndGame()
    {
        Debug.Log("Elementia restored");
    }
}
