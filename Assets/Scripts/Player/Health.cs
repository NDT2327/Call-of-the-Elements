using UnityEngine;

public class Health : MonoBehaviour
{
	[SerializeField] private float startingHealth = 100f;
	public float CurrentHealth { get; set; }
	public float MaxHealth => startingHealth;

	private Animator anim;
	private bool dead;

	private void Awake()
	{
		CurrentHealth = startingHealth;
		anim = GetComponent<Animator>();
	}

	public void TakeDamage(float _damage)
	{
		CurrentHealth = Mathf.Clamp(CurrentHealth - _damage, 0, startingHealth);

		if (CurrentHealth > 0)
		{
			//player hurt
			anim.SetTrigger("Hurt");
			AudioManager.instance.PlayPlayerHurtSound();
		}
		else if (!dead)
		{
			anim.SetTrigger("Death");
			AudioManager.instance.PlayPlayerDeathSound();
			dead = true;
			GetComponent<Player>().Die();

			//// Instead of disabling Player, send an event
			//Debug.Log("☠ Player has died.");
			//if (GameManager.Instance != null)
			//{
			//	GameManager.Instance.();
			//}
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			TakeDamage(1);
		}
	}

	public void Heal(float amount)
	{
		if (!dead)
		{
			CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
			Debug.Log("❤️ Hồi " + amount + " máu. Máu hiện tại: " + CurrentHealth);
		}
	}

	public void RecoverFullHealth()
	{
		CurrentHealth = MaxHealth;
	}

	public void RestartFromCheckpoint()
	{
		// Reset vị trí
		transform.position = GameManager.Instance.GetCheckpoint();
		Time.timeScale = 1f;

		CurrentHealth = MaxHealth;
		dead = false;

		// Reset Animator
		if (anim == null)
		{
			anim = GetComponent<Animator>();
			if (anim == null)
			{
				Debug.LogError("Animator not found during restart on " + gameObject.name);
				return;
			}
		}
		anim.ResetTrigger("Hurt");
		anim.ResetTrigger("Death");
		anim.ResetTrigger("Attack1");
		anim.ResetTrigger("Attack2");
		anim.ResetTrigger("Attack3");
		anim.ResetTrigger("Jump");
		anim.ResetTrigger("Roll");
		anim.SetBool("Grounded", true); // Đặt lại trạng thái grounded
		anim.Play("Idle", -1, 0f); // Chơi "Idle" từ frame đầu
		anim.Update(0f); // Cập nhật ngay lập tức

		// Reset trạng thái Player
		Player player = GetComponent<Player>();
		if (player != null)
		{
			player.ResetState(); // Gọi phương thức reset của Player
		}

		Debug.Log("Restarted at checkpoint: " + transform.position + ", Animation reset to Idle, Dead = " + dead);
	}
}
