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


    void Start()
    {
        if (blockFlash != null) blockFlash.SetActive(false);
        if (dust != null) dust.SetActive(false);
        if (dust2 != null) dust2.SetActive(false);
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

    private void SpAttack()
    {
        if (Input.GetKeyDown(KeyCode.U) && Time.time - lastSpecialAttackTime >= specialAttackCooldown)
        {
            animator.SetTrigger("Attack3"); // Chạy animation Attack3

            // Tìm kẻ địch gần nhất
            GameObject nearestEnemy = FindNearestEnemy();
            if (nearestEnemy != null)
            {
                // Gọi spell ngay dưới chân kẻ địch
                SpawnSpell(spellEarthPrefab, nearestEnemy.transform.position);
                SpawnSpell(spellEarth2Prefab, nearestEnemy.transform.position);
                SpawnSpell(spellEarth3Prefab, nearestEnemy.transform.position);

                // Gây damage cho kẻ địch
                nearestEnemy.GetComponent<EnemyHP>().TakeDamage(50f); // Gây 50 sát thương (tùy chỉnh)

                lastSpecialAttackTime = Time.time; // Cập nhật thời gian sử dụng skill
            }
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
            GameObject spell = Instantiate(spellPrefab, position, Quaternion.identity);
            spell.SetActive(true);

            // Xóa spell sau 1 giây
            Destroy(spell, 1f);
        }
    }

    private void HandleBlock()
    {
        if (Input.GetKeyDown(KeyCode.S) && isGrounded)
        {
            animator.SetTrigger("Block");
            ShowBlockFlash();
        }
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
        if (collision.gameObject.CompareTag("Ground"))
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
}
