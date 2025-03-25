using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [Header("Audio Source")]
    public AudioSource backgroundMusic;
    public AudioSource sfxSource;


    [Header("Background music")]
    public AudioClip mainBackgroundMusic;
    public AudioClip earthMusic;
    public AudioClip fireMusic;
    public AudioClip castleMusic;
    public AudioClip victoryMusic;
    public AudioClip gameOverMusic;

    [Header("Player sound")]
    public AudioClip playerAttackSound;
    public AudioClip playerDeathSound;
    public AudioClip playerHurtSound;
    public AudioClip playerJumpSound;
    public AudioClip playerSpecial1;
    public AudioClip playerSpecial2;
    public AudioClip changeElement;
    public AudioClip playerMoveSound;

    [Header("Enemy sound")]
    public AudioClip enemyAttackSound1;
    public AudioClip enemyAttackSound2;
    public AudioClip emyDeathSound;
    public AudioClip enemyHurtSound;
    public AudioClip enemyMoveSound;

    [Header("Event Sound")]
    public AudioClip checkpointSound;
    public AudioClip itemPickupSound;
    public AudioClip bossDefeatedSound;
    public AudioClip clickButton;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(instance);
        }

        if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
        if (backgroundMusic == null) backgroundMusic = gameObject.AddComponent<AudioSource>();

        sfxSource.playOnAwake = false;
        backgroundMusic.playOnAwake = false;
        backgroundMusic.loop = true;
    }



    public void PlayBackgroundMusic(GameManager.Map map)
    {
        AudioClip clip = null;
        switch (map)
        {

            case GameManager.Map.Earth:
                clip = earthMusic; break;
            case GameManager.Map.Lava:
                clip = fireMusic; break;
            case GameManager.Map.Castle:
                clip = castleMusic; break;
        }

        if (clip != null && backgroundMusic.clip != clip)
        {
            backgroundMusic.clip = clip;
            backgroundMusic.Play();
            Debug.Log("Playing background music for map: " + map);
        }
    }

    public void PlayMainBackgroundMusic()
    {
        if (mainBackgroundMusic != null)
        {
            backgroundMusic.clip = mainBackgroundMusic;
            backgroundMusic.loop = true;
            backgroundMusic.Play();
            Debug.Log("Playing main background music");
        }
    }

    //play victory
    public void PlayVictoryMusic()
    {
        if (victoryMusic != null)
        {
            backgroundMusic.clip = victoryMusic;
            backgroundMusic.loop = false;
            backgroundMusic.Play();
            Debug.Log("Playing victory music");
        }
    }

    //play game over
    public void PlayGameOverMusic()
    {
        if (gameOverMusic != null)
        {
            backgroundMusic.clip = gameOverMusic;
            backgroundMusic.loop = false;
            backgroundMusic.Play();
            Debug.Log("Playing game over music");
        }
    }

    public void StopBackgroundMusic()
    {
        backgroundMusic.Stop();
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && !sfxSource.isPlaying) sfxSource.PlayOneShot(clip);
    }

    // Player Sound Methods
    public void PlayPlayerAttackSound() => PlaySFX(playerAttackSound);
    public void PlayPlayerDeathSound() => PlaySFX(playerDeathSound);
    public void PlayPlayerHurtSound() => PlaySFX(playerHurtSound);
    public void PlayPlayerJumpSound() => PlaySFX(playerJumpSound);
    public void PlayPlayerSpecial1Sound() => PlaySFX(playerSpecial1);
    public void PlayPlayerSpecial2Sound() => PlaySFX(playerSpecial2);
    public void PlayChangeElementSound() => PlaySFX(changeElement);
    public void PlayPlayerMoveSound() => PlaySFX(playerMoveSound);

    // Enemy Sound Methods
    public void PlayEnemyAttackSound1() => PlaySFX(enemyAttackSound1);
    public void PlayEnemyAttackSound2() => PlaySFX(enemyAttackSound2);
    public void PlayEnemyDeathSound() => PlaySFX(emyDeathSound);
    public void PlayEnemyHurtSound() => PlaySFX(enemyHurtSound);
    public void PlayEnemyMoveSound() => PlaySFX(enemyMoveSound);

    // Event Sound Methods
    public void PlayCheckpointSound() => PlaySFX(checkpointSound);
    public void PlayItemPickupSound() => PlaySFX(itemPickupSound);
    public void PlayBossDefeatedSound() => PlaySFX(bossDefeatedSound);
    public void PlayClickButtonSound() => PlaySFX(clickButton);
}
