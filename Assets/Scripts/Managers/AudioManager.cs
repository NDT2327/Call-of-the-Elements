using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance { get; private set; }

	[Header("Audio Source")]
	[SerializeField] AudioSource backgroundMusic;
	[SerializeField] AudioSource sfxSource;


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
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void Start()
	{
		backgroundMusic.clip = mainBackgroundMusic;
		backgroundMusic.Play();
	}

	public void PlayBackgroundMusic(GameManager.Map map)
	{
		AudioClip clip = map switch
		{
			GameManager.Map.Earth => earthMusic,
			GameManager.Map.Lava => fireMusic,
			GameManager.Map.Castle => castleMusic,
			_ => null
		};

		if (clip != null && backgroundMusic.clip != clip)
		{
			backgroundMusic.clip = clip;
			backgroundMusic.loop = true;

			backgroundMusic.Play();
		}
	}

	public void PlayMainBackgroundMusic()
	{
		if (mainBackgroundMusic != null && backgroundMusic.clip != mainBackgroundMusic)
		{
			backgroundMusic.clip = mainBackgroundMusic;
			backgroundMusic.loop = true;
			backgroundMusic.Play();
		}
	}

	//play victory
	public void PlayVictoryMusic() => PlayMusic(victoryMusic, false);
	public void PlayGameOverMusic()
	{
		if (backgroundMusic != null)
		{
			backgroundMusic.Stop();
			backgroundMusic.clip = gameOverMusic;
			backgroundMusic.loop = false; // Không lặp game over music
			backgroundMusic.Play();
		}
	}
	public void StopGameOverMusic()
	{
		if (backgroundMusic != null && backgroundMusic.clip == gameOverMusic && backgroundMusic.isPlaying)
		{
			backgroundMusic.Stop();
		}
	}

	private void PlayMusic(AudioClip clip, bool loop)
	{
		if (clip != null && backgroundMusic.clip != clip)
		{
			backgroundMusic.clip = clip;
			backgroundMusic.loop = loop;
			backgroundMusic.Play();
			Debug.Log($"Playing music: {clip.name}");
		}
	}

	public void StopBackgroundMusic()
	{
		if (backgroundMusic != null && backgroundMusic.isPlaying)
		{
			backgroundMusic.Stop();
		}
	}

	private void PlaySFX(AudioClip clip)
	{
		if (clip != null) sfxSource.PlayOneShot(clip);
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
