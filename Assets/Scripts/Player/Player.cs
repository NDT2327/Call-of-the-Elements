using System.Collections;
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

    public float rollDistance = 5f; // Khoảng cách lướt có thể điều chỉnh
    public float rollDuration = 0.3f; // Thời gian lướt
    private bool isRolling = false;

	private float lastMovement = 0f;

	void Start()
    {
        if (blockFlash != null) blockFlash.SetActive(false);
        if (dust != null) dust.SetActive(false);
        if (dust2 != null) dust2.SetActive(false);
        GameObject terribleKnight = GameObject.FindGameObjectWithTag("Enemy");
        if (terribleKnight != null)
        {
            terribleKnightScript = terribleKnight.GetComponent<TerribleKnightScript>();
        }

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


        //HandleUltimate();
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
		// Chỉ phát âm thanh khi bắt đầu di chuyển
		if (Mathf.Abs(movement) > 0f && Mathf.Abs(lastMovement) == 0f)
		{
			AudioManager.instance.PlayPlayerMoveSound();
		}
		lastMovement = movement;

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
				AudioManager.instance.PlayPlayerAttackSound();

				animator.SetTrigger("Attack1");
            }
            else if (attackCount == 2)
            {
				AudioManager.instance.PlayPlayerAttackSound();

				animator.SetTrigger("Attack2");

            }
            else if (attackCount >= 3)
            {
				AudioManager.instance.PlayPlayerAttackSound();
				animator.SetTrigger("Attack3");
                attackCount = 0;
            }
        }
    }

    private void HandleElementChange()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
			AudioManager.instance.PlayChangeElementSound();

			currentElementIndex = (currentElementIndex + 1) % elements.Length;
            Debug.Log("Hệ hiện tại: " + elements[currentElementIndex]);
        }
    }

    private void SpAttack()
    {
        //if (!HasUnlockedSpAttack()) return;

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


            animator.SetTrigger("Attack3");

            GameObject spellPrefab = GetSpellByElement();
            if (spellPrefab == null) return;

            Vector3 spawnPosition = transform.position;

            switch (elements[currentElementIndex])
            {
                case "Fire":
					AudioManager.instance.PlayPlayerSpecial1Sound();

					SpawnSpell(spellFirePrefab, transform.position);
                    break;

                case "Earth":
					AudioManager.instance.PlayPlayerSpecial2Sound();
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
                healthBar.SetElementSprite(currentElementIndex);
                healthBar.StartElementCooldown();
            }
        }
    }

    //private void HandleUltimate()
    //{
    //    if (Input.GetKeyDown(KeyCode.I) && Time.time - lastUltimateTime >= ultimateCooldown)
    //    {
    //        if (healthBar != null && healthBar.playerStamina != null)
    //        {
    //            float staminaCost = healthBar.playerStamina.MaxStamina * 0.4f; // Tốn 40% Stamina
    //            if (healthBar.playerStamina.CurrentStamina < staminaCost)
    //            {
    //                Debug.Log("⚠ Không đủ Stamina để dùng Ultimate!");
    //                return;
    //            }
    //            healthBar.playerStamina.UseStamina(staminaCost);
    //        }

    //        animator.SetTrigger("Attack1"); // Kích hoạt animation Ultimate

    //        // Triệu hồi Ultimate
    //        Vector3 spawnPosition = transform.position + new Vector3(facingRight ? 1.5f : -1.5f, 0, 0);
    //        Quaternion rotation = facingRight ? Quaternion.identity : Quaternion.Euler(0, 180, 0);
    //        GameObject ultimate = Instantiate(ultimatePrefab, spawnPosition, rotation);
    //        ultimate.SetActive(true);

    //        lastUltimateTime = Time.time; // Lưu thời gian dùng Ultimate

    //        Debug.Log("🔥 Ultimate được kích hoạt!");
    //        if (healthBar != null)
    //        {
    //            healthBar.StartUltimateCooldown();
    //        }
    //    }
    //}

    private GameObject GetSpellByElement()
    {
        return elements[currentElementIndex] switch
        {
            "Fire" => spellFirePrefab,
            "Earth" => spellEarthPrefab,
            _ => null,
        };
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
        if (Input.GetKeyDown(KeyCode.L) && isGrounded && !isRolling)
        {
            StartCoroutine(Roll());
        }
    }
    private IEnumerator Roll()
    {
        isRolling = true;
        animator.SetTrigger("Roll");

        float rollDirection = facingRight ? 1f : -1f; // Hướng lướt dựa vào mặt nhân vật
        float startTime = Time.time;

        while (Time.time < startTime + rollDuration)
        {
            transform.position += new Vector3(rollDirection * rollDistance * Time.deltaTime, 0, 0);
            yield return null;
        }

        isRolling = false;
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(movement, 0f, 0f) * speed * Time.fixedDeltaTime;
    }

    void Jump()
    {
        AudioManager.instance.PlayPlayerJumpSound();
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

    public void Die()
    {
        AudioManager.instance.PlayPlayerDeathSound();
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

	// Thêm phương thức reset trạng thái của Player
	public void ResetState()
	{
		rb2d.linearVelocity = Vector2.zero; // Reset vận tốc
		rb2d.angularVelocity = 0f;
		rb2d.constraints = RigidbodyConstraints2D.FreezeRotation; // Đảm bảo không khóa X/Y
		isGrounded = true;
		canDoubleJump = false;
		isRolling = false;
		attackCount = 0;
		lastAttackTime = 0f;
		lastSpecialAttackTime = -Mathf.Infinity;
		movement = 0f;
		lastMovement = 0f;

		Debug.Log("Player state reset: Grounded = " + isGrounded + ", Velocity = " + rb2d.linearVelocity);
	}
	//public bool HasUnlockedSpAttack()
	//{
	//    bool unlocked = (elements[currentElementIndex] == "Fire" && currentLevel >= 3) ||
	//                    (elements[currentElementIndex] == "Earth" && currentLevel >= 2);

	//    if (unlocked && healthBar != null && !healthBar.hasSpAttack)
	//    {
	//        healthBar.UnlockSpAttack();
	//    }
	//    return unlocked;
	//}


	//public void UnlockSpecialAttack(GameManager.Map completedMap)
	//{
	//    if (completedMap == GameManager.Map.Earth)
	//    {
	//        currentLevel = 2; // Mở khóa SpAttack Earth
	//        Debug.Log("🌱 Đã mở khóa Special Attack Earth!");
	//    }
	//    else if (completedMap == GameManager.Map.Lava)
	//    {
	//        currentLevel = 3; // Mở khóa SpAttack Fire
	//        Debug.Log("🔥 Đã mở khóa Special Attack Fire!");
	//    }
	//}

}
