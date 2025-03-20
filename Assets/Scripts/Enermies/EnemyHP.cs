using UnityEngine;
using System.Collections;


public class EnemyHP : MonoBehaviour
{
	[Header("Health Settings")]
	public float maxHP = 100f;
    public float currentHP;
    public float MaxHealth { get; private set; }
    private Animator anim;
    private Rigidbody2D rb;

	// Biến đếm số lần bị đánh
    private int hitCount = 0;
    public int hitLimit = 5; // Sau 5 lần bị đánh, kích hoạt trạng thái invincible
    private bool isInvincible = false;
    public float invincibleDuration = 3f; // Thời gian không nhận sát thương

    [Header("Potion Drop Settings")]
    [SerializeField] private GameObject potionPrefab; // Prefab của Potion
    [SerializeField] private float dropChance = 0.6f; // 60% tỷ lệ rơi Potion
    [SerializeField] private bool isBoss = false; // Nếu là Boss, không rơi Potion
    void Start()
	{
        currentHP = maxHP;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    void Awake()
    {
        if (maxHP <= 0)
        {
            Debug.LogError("❌ maxHP của Boss phải lớn hơn 0! Kiểm tra giá trị trong Inspector.");
            maxHP = 100f; // Đặt giá trị mặc định nếu chưa có
        }
        MaxHealth = maxHP;
        currentHP = maxHP;
    }

    // Gây sát thương cho quái
    // Gây sát thương cho enemy
    public void TakeDamage(float damage)
    {
        if (isInvincible)
            return;

        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        hitCount++;

        if (currentHP <= 0)
        {
            Die();
            return;
        }

        // Khi bị đánh đủ hitLimit, mới kích hoạt hurt animation
        if (hitCount >= hitLimit)
        {
            anim.SetTrigger("hurt");
            Debug.Log("stun");
            // Kích hoạt trạng thái invincible cho thời gian cho enemy phục hồi
            //StartCoroutine(ActivateInvincibility());
            hitCount = 0; // Reset biến đếm sau khi kích hoạt hurt
        }
    }

    // Gây sát thương cho boss
    public void TakeBossDamage(float damage)
    {
        if (isInvincible)
        {
            // Nếu đang trong trạng thái invincible, bỏ qua sát thương và không kích hoạt hurt
            return;
        }

        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        // Tăng biến đếm khi nhận sát thương
        hitCount++;

        if (currentHP <= 0)
        {
            Die();
            return;
        }

        // Nếu số lần đánh chưa đạt giới hạn, kích hoạt hurt
        if (hitCount < hitLimit)
        {
            anim.SetTrigger("hurt");
        }
        else
        {
            // Khi đạt giới hạn, không kích hoạt hurt nữa và bật trạng thái invincible
            StartCoroutine(ActivateInvincibility());
        }
    }

    // Coroutine kích hoạt trạng thái invincible
    private IEnumerator ActivateInvincibility()
    {
        isInvincible = true;
        Debug.Log(gameObject.name + " is invincible now!");
        // Có thể thêm hiệu ứng hoặc đổi màu để thông báo trạng thái invincible
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
        hitCount = 0; // Reset lại biến đếm sau khi hết trạng thái invincible
        Debug.Log(gameObject.name + " can take damage again.");
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
        if (!isBoss)
        {
            TrySpawnPotion();
        }
        Destroy(gameObject, 1f);
    }

    private void TrySpawnPotion()
    {
        float randomValue = Random.value; // Random từ 0 -> 1
        if (randomValue <= dropChance)
        {
            Instantiate(potionPrefab, transform.position, Quaternion.identity);
            Debug.Log("🧪 Potion đã spawn!");
        }
    }

    // Lấy HP hiện tại
    public float GetCurrentHP()
	{
		return currentHP;
	}

    public bool HasAppeared()
    {
        return gameObject.activeSelf; // Kiểm tra xem boss có đang hoạt động không
    }

}
