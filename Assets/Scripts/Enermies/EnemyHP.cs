using UnityEngine;
using System.Collections;
using Unity.Jobs;


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
    [SerializeField] private float dropChance = 0.6f; // 60% tỷ lệ rơi Potion
    [SerializeField] private bool isBoss = false; // Nếu là Boss, không rơi Potion
    [Header("Potion Drop Settings")]
    [SerializeField] private GameObject healthPotionPrefab; // Prefab của máu
    [SerializeField] private GameObject manaPotionPrefab;   // Prefab của mana

    [Header("Upgrad item")]
    [SerializeField] private GameObject upgradeItemPrefab;
    [SerializeField] private GameManager.Map map;


    private EnemyHealthBar healthBar;
    private bool isDead = false;
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
            // Kích hoạt trạng thái invincible cho thời gian cho enemy phục hồi
            //StartCoroutine(ActivateInvincibility());
            AudioManager.instance.PlayEnemyHurtSound();
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
            AudioManager.instance.PlayEnemyHurtSound();

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
        // Có thể thêm hiệu ứng hoặc đổi màu để thông báo trạng thái invincible
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
        hitCount = 0; // Reset lại biến đếm sau khi hết trạng thái invincible
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
        if (isDead) return;
        isDead = true;

		Debug.Log(gameObject.name + " has died!");
        // Hoặc có thể trigger animation chết
        anim.SetTrigger("die");
        AudioManager.instance.PlayEnemyDeathSound();

        rb.linearVelocity = Vector2.zero;
        if (!isBoss)
        {
            TrySpawnPotion();
            Destroy(gameObject, 1f);
        }
        else
        {
            StartCoroutine(HandleBossDeath());
        }
    }

    private IEnumerator HandleBossDeath()
    {
        float deathAnimationDuration = 1f;
        yield return new WaitForSeconds(deathAnimationDuration);

        //spawn 
        if (upgradeItemPrefab != null)
        {
            GameObject item = Instantiate(upgradeItemPrefab, transform.position, Quaternion.identity);
            UpgradeItem upgradeItem = item.GetComponent<UpgradeItem>();
            if (upgradeItem != null)
            {
                upgradeItem.unlockMap = map;
            }
            Debug.Log("Upgrad item spawned at: " + transform.position);
        }

        AudioManager.instance.PlayBossDefeatedSound();
        GameManager.Instance.OnBossDefeated();

        if(healthBar != null && healthBar.enemyHealthContainer != null)
        {
            yield return StartCoroutine(HideBossHealthBar(4f));
        }
        Destroy(gameObject);
    }

    private IEnumerator HideBossHealthBar(float delay)
    {
        yield return new WaitForSeconds(delay);
        healthBar.enemyHealthContainer.SetActive(false);
        Debug.Log("🩸 Thanh máu Boss đã bị ẩn!");
    }
    private void TrySpawnPotion()
    {
        float randomValue = Random.value; // Random từ 0 -> 1
        if (randomValue <= dropChance) // 60% cơ hội rơi đồ
        {
            GameObject potionToSpawn = (Random.value < 0.5f) ? healthPotionPrefab : manaPotionPrefab;
            Instantiate(potionToSpawn, transform.position, Quaternion.identity);
            Debug.Log($"🧪 {potionToSpawn.name} đã spawn!");
        }
    }

    // Lấy HP hiện tại
    public float GetCurrentHP()
	{
		return currentHP;
	}

    public bool HasAppeared()
    {
        if (isBoss && currentHP > 0) 
        return gameObject.activeSelf; // Kiểm tra xem boss có đang hoạt động không
        else return false;
    }

}
