using UnityEngine;

public class EnemyHP : MonoBehaviour
{
	[Header("Health Settings")]
	public float maxHP = 100f;
	private float currentHP;

    private Animator anim;
    private Rigidbody2D rb;


    void Start()
	{
		currentHP = maxHP;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

	// Gây sát thương cho quái
	public void TakeDamage(float damage)
	{
		currentHP -= damage;
		currentHP = Mathf.Clamp(currentHP, 0, maxHP); // Đảm bảo HP không nhỏ hơn 0

		if (currentHP <= 0)
		{
			Die();
		}
        anim.SetTrigger("hurt");
    }

	// Hồi máu cho quái
	public void Heal(float amount)
	{
		currentHP += amount;
		currentHP = Mathf.Clamp(currentHP, 0, maxHP); // Đảm bảo HP không vượt quá maxHP
	}

	// Xử lý khi quái chết
	private void Die()
	{
		Debug.Log(gameObject.name + " has died!");
        // Hoặc có thể trigger animation chết
        anim.SetTrigger("die");
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject, 1f);
    }

	// Lấy HP hiện tại
	public float GetCurrentHP()
	{
		return currentHP;
	}
}
