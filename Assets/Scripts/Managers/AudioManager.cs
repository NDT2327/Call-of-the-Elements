using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Background music")]
    public AudioSource backgroundMusic;
    public AudioClip earthMusic;
    public AudioClip fireMusic;
    public AudioClip castleMusic;
    public AudioClip victoryMusic;

    [Header("Boss sound")]
    public AudioSource sound;

    

    public void PlayBackgroundMusic(GameManager.Map map)
    {
        backgroundMusic.Stop();
        backgroundMusic.clip = map switch
        {
            GameManager.Map.Earth => earthMusic,
            GameManager.Map.Lava => fireMusic,
            GameManager.Map.Castle => castleMusic,
            _ => null
        };
        backgroundMusic.Play();
    }

    public void PlayVictoryMusic()
    {
        backgroundMusic.Stop();
        backgroundMusic.clip = victoryMusic;
        backgroundMusic.Play();
    }
}
