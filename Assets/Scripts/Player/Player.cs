using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb2d;
    public Animator animator;
    public GameObject blockFlash;
    public GameObject dust;
    public GameObject dust2;
    public GameObject spellEarthPrefab;
    public GameObject spellEarth2Prefab;
    public GameObject spellEarth3Prefab;

    public float speed = 5f;
    public float jumpHeight = 5f;
    public bool isGrounded = false;
    private bool canDoubleJump = false;

    private float movement;
    private bool facingRight = true;
    private int attackCount = 0;
    private float lastAttackTime;
    public float attackResetTime = 0.5f;
    private float specialAttackCooldown = 3f; // Thời gian chờ giữa mỗi lần dùng SpAttack
    private float lastSpecialAttackTime = -Mathf.Infinity;

    private int currentElementIndex = 0; // Mặc định là Lửa (1)
    private string[] elements = { "Earth", "Fire" };
    public GameObject spellFirePrefab;
    private TerribleKnightScript terribleKnightScript;
    private int currentLevel = 1; // Giả sử bắt đầu từ màn 1

    private HealthBar healthBar;


    void Start()
    {
        if (blockFlash != null) blockFlash.SetActive(false);
        if (dust != null) dust.SetActive(false);
        if (dust2 != null) dust2.SetActive(false);
        terribleKnightScript = GameObject.FindGameObjectWithTag("Enemy").GetComponent<TerribleKnightScript>();
        healthBar = FindFirstObjectByType<HealthBar>();
    }

    void Update()
    {
        movement = Input.GetAxis("Horizontal");
        Move();
        HandleJump();
        HandleAttack();
        HandleBlock();
        HandleRoll();
        SpAttack();
        HandleElementChange();

        //if out of map
        if (transform.position.y < -10)
        {
            Die();
        }

    }

    private void Move()
    {
        if ((movement < 0f && facingRight) || (movement > 0f && !facingRight))
        {
            Flip();
        }
        animator.SetInteger("AnimState", Mathf.Abs(movement) > 0f ? 1 : 0);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.eulerAngles = new Vector3(0f, facingRight ? 0f : 180f, 0f);
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (isGrounded)
            {
                Jump();
                isGrounded = false;
                canDoubleJump = true;
                if (terribleKnightScript != null)
                {
                    Debug.Log("JUMP");
                    terribleKnightScript.OnPlayerJump();
                    
                }
            }
            else if (canDoubleJump)
            {
                Jump();
                canDoubleJump = false;
            }
        }
    }

    private void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (Time.time - lastAttackTime > attackResetTime)
            {
                attackCount = 0;
            }

            attackCount++;
            lastAttackTime = Time.time;

            if (attackCount == 1)
            {
                animator.SetTrigger("Attack1");
            }
            else if (attackCount == 2)
            {
                animator.SetTrigger("Attack2");
            }
            else if (attackCount >= 3)
            {
                animator.SetTrigger("Attack3");
                attackCount = 0;
            }
        }
    }

    private void HandleElementChange()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            currentElementIndex = (currentElementIndex + 1) % elements.Length;
            Debug.Log("Hệ hiện tại: " + elements[currentElementIndex]);
        }
    }

    private void SpAttack()
    {
        if (Input.GetKeyDown(KeyCode.U) && Time.time - lastSpecialAttackTime >= specialAttackCooldown)
        {
            if (healthBar != null && healthBar.playerStamina != null)
            {
                float staminaCost = healthBar.playerStamina.MaxStamina * 0.2f;
                if (healthBar.playerStamina.CurrentStamina < staminaCost)
                {
                    Debug.Log("⚠ Không đủ Stamina để sử dụng SpAttack!");
                    return;
                }
                healthBar.playerStamina.UseStamina(staminaCost);
            }

            // Kiểm tra màn chơi hiện tại
            if ((elements[currentElementIndex] == "Fire" && currentLevel < 3) ||
                (elements[currentElementIndex] == "Earth" && currentLevel < 2))
            {
                Debug.Log("⚠ Chưa mở khóa chiêu này ở màn hiện tại!");
                return;
            }

            animator.SetTrigger("Attack3");

            GameObject spellPrefab = GetSpellByElement();
            if (spellPrefab == null) return;

            Vector3 spawnPosition = transform.position;

            switch (elements[currentElementIndex])
            {
                case "Fire":
                    SpawnSpell(spellFirePrefab, transform.position);
                    break;

                case "Earth":
                    GameObject nearestEnemy = FindNearestEnemy();
                    if (nearestEnemy != null)
                    {
                        spawnPosition = nearestEnemy.transform.position - new Vector3(0, 1f, 0);
                    }
                    SpawnSpell(spellEarthPrefab, spawnPosition);
                    SpawnSpell(spellEarth2Prefab, spawnPosition);
                    SpawnSpell(spellEarth3Prefab, spawnPosition);
                    break;
            }

            lastSpecialAttackTime = Time.time;

            if (healthBar != null)
            {
                healthBar.StartElementCooldown();
            }
        }
    }
    
    private GameObject GetSpellByElement()
    {
        switch (elements[currentElementIndex])
        {
            case "Fire": return spellFirePrefab;
            case "Earth": return spellEarthPrefab;
            default: return null;
        }
    }

    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }

    private void SpawnSpell(GameObject spellPrefab, Vector3 position)
    {
        if (spellPrefab != null)
        {
            Quaternion rotation = facingRight ? Quaternion.identity : Quaternion.Euler(0, 180, 0);
            GameObject spell = Instantiate(spellPrefab, position, rotation);
            spell.SetActive(true);
            Destroy(spell, 1f);
        }
    }

    private void HandleBlock()
    {
        //if (Input.GetKeyDown(KeyCode.S) && isGrounded)
        //{
        //    animator.SetTrigger("Block");
        //    ShowBlockFlash();
        //}
        //if (Input.GetKeyUp(KeyCode.S))
        //{

        //}
    }


    private void HandleRoll()
    {
        if (Input.GetKeyDown(KeyCode.L) && isGrounded)
        {
            animator.SetTrigger("Roll");
        }
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(movement, 0f, 0f) * speed * Time.fixedDeltaTime;
    }

    void Jump()
    {
        rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, jumpHeight);
        animator.SetBool("Grounded", false);
        animator.SetTrigger("Jump");

        CreateDustEffects(dust);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Danger"))
        {
            isGrounded = true;
            animator.SetBool("Grounded", true);
            canDoubleJump = false;
            CreateDustEffects(dust2);
        }
    }

    void ShowBlockFlash()
    {
        if (blockFlash != null)
        {
            blockFlash.SetActive(true);
            Invoke("HideBlockFlash", 0.3f);
        }
    }

    void HideBlockFlash()
    {
        if (blockFlash != null)
        {
            blockFlash.SetActive(false);
        }
    }

    void CreateDustEffects(GameObject dustEffect)
    {
        if (dustEffect != null)
        {
            GameObject dustInstance = Instantiate(dustEffect, transform.position - new Vector3(0f, 0.5f, 0f), Quaternion.identity);
            dustInstance.SetActive(true);
            Destroy(dustInstance, 0.5f);
        }
    }

    void DealDamage()
    {
        float attackRange = 1.5f;
        float attackDamage = 20f;
        Vector2 attackPosition = transform.position + new Vector3(facingRight ? attackRange : -attackRange, 0, 0);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, 0.5f);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<EnemyHP>().TakeDamage(attackDamage);
                Debug.Log("Gây " + attackDamage + " sát thương lên " + enemy.gameObject.name);
            }
        }
    }

    private void Die()
    {
        UIManager.Instance.ShowGameOverScreen();
    }

    public void RecoverHealthAndStamina(float percentage)
    {
        if (healthBar != null)
        {
            float healAmount = healthBar.playerHealth.MaxHealth * percentage;
            healthBar.playerHealth.CurrentHealth = Mathf.Min(healthBar.playerHealth.MaxHealth, healthBar.playerHealth.CurrentHealth + healAmount);

            float staminaAmount = healthBar.playerStamina.MaxStamina * percentage;
            healthBar.playerStamina.CurrentStamina = Mathf.Min(healthBar.playerStamina.MaxStamina, healthBar.playerStamina.CurrentStamina + staminaAmount);

            Debug.Log($"Hồi {percentage * 100}% máu và stamina!");
        }
    }

    public void UnlockSpecialAttack(GameManager.Map completedMap)
    {
        if (completedMap == GameManager.Map.Earth)
        {
            currentLevel = 2; // Mở khóa SpAttack Earth
            Debug.Log("🌱 Đã mở khóa Special Attack Earth!");
        }
        else if (completedMap == GameManager.Map.Lava)
        {
            currentLevel = 3; // Mở khóa SpAttack Fire
            Debug.Log("🔥 Đã mở khóa Special Attack Fire!");
        }
    }

}
