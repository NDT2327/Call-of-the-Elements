using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerCustom : MonoBehaviour
{
    public static SceneManagerCustom instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LoadAudioForCurrentScene();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        PlayAudioForScene(sceneName);
    }

    public void ReloadCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
        PlayAudioForScene(currentScene);
    }

    private void PlayAudioForScene(string sceneName)
    {
        switch (sceneName)
        {

            case "Menu":
                AudioManager.instance.PlayMainBackgroundMusic(); break;
            case "Earth":
                AudioManager.instance.PlayBackgroundMusic(GameManager.Map.Earth);
                break;
            case "Lava":
                AudioManager.instance.PlayBackgroundMusic(GameManager.Map.Lava);
                break;
            case "Castle":
                AudioManager.instance.PlayBackgroundMusic(GameManager.Map.Castle);
                break;
        }
    }

    private void LoadAudioForCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        PlayAudioForScene(currentScene);
    }
}
