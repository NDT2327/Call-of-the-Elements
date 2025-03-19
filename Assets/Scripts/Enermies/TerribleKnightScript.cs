using UnityEngine;
using System.Collections;

public class TerribleKnightScript : MonoBehaviour
{
    [Header("Detection Settings")]
    public Transform target;
    public float detectionRange = 7f;
    public float attackRange = 1.5f;
    public LayerMask targetMask;

    [Header("Movement & Attack")]
    public float moveSpeed = 3f;
    public float jumpForce = 7f;
    public float attackCooldown = 2f;
    private float lastAttackTime = 0f;

    private Animator anim;
    private Rigidbody2D rb;
    private bool isJumping = false;

    // Lưu lại kiểu tấn công hiện tại để xác định sát thương
    private int currentAttackType = 0;

    private Health playerHP; // Dùng để gây sát thương cho player

    private enum State { Idle, Run, Attack, Jump }
    private State currentState = State.Idle;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        Collider2D enemyCollider = GetComponent<Collider2D>();
        Collider2D playerCollider = target.GetComponent<Collider2D>();
        if (enemyCollider != null && playerCollider != null)
        {
            Physics2D.IgnoreCollision(enemyCollider, playerCollider);
        }
    }

    void Update()
    {
        if (isJumping) return; // Nếu đang nhảy, không đổi trạng thái

        float distanceToTarget = target ? Vector2.Distance(transform.position, target.position) : Mathf.Infinity;
        bool canAttack = (Time.time - lastAttackTime) >= attackCooldown;

        if (canAttack && distanceToTarget <= attackRange)
        {
            currentState = State.Attack;
            PerformAttack();
        }
        else if (distanceToTarget <= detectionRange)
        {
            currentState = State.Run;
        }
        else
        {
            currentState = State.Idle;
        }

        UpdateAnimations();
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Run:
                MoveTowardsTarget();
                break;
            case State.Jump:
                break;
            case State.Idle:
            case State.Attack:
                rb.linearVelocity = Vector2.zero;
                break;
        }
    }

    void MoveTowardsTarget()
    {
        if (target == null) return;

        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        FlipSprite(direction.x);
    }

    void PerformAttack()
    {
        lastAttackTime = Time.time;
        // Lưu lại kiểu tấn công được chọn (1 đến 5)
        currentAttackType = Random.Range(1, 6);
        anim.SetTrigger($"attack{currentAttackType}");

        Invoke(nameof(ResetToRun), 1f);
    }

    private void ResetToRun()
    {
        currentState = State.Run;
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isRunning", currentState == State.Run);
        anim.SetBool("isJumping", currentState == State.Jump);
    }

    private void FlipSprite(float moveX)
    {
        if ((moveX > 0 && transform.localScale.x < 0) || (moveX < 0 && transform.localScale.x > 0))
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    public void OnPlayerJump()
    {
        if (isJumping) return;

        if (Random.value < 0.3f) // 30% cơ hội nhảy khi Player nhảy
        {
            Jump();
        }
    }

    private void Jump()
    {
        currentState = State.Jump;
        isJumping = true;
        anim.SetTrigger("jump");
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        StartCoroutine(JumpAttackChance());
    }

    private IEnumerator JumpAttackChance()
    {
        yield return new WaitForSeconds(0.3f);

        if (Random.value < 0.5f) // 50% cơ hội tấn công trên không
        {
            PerformAttack();
        }

        yield return new WaitForSeconds(0.5f);
        isJumping = false;
        currentState = State.Run;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            currentState = State.Run;
        }
    }

    // Hàm này được gọi qua Animation Event tại thời điểm gây sát thương trong animation tấn công
    public void DoDamageCloseRange()
    {
        // Kiểm tra xem có collider nào nằm trong phạm vi attackRange và thuộc targetMask không
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, targetMask);
        if (hit != null)
        {
            // Lấy component Health của đối tượng bị tấn công
            playerHP = hit.GetComponent<Health>();
            if (playerHP != null)
            {
                int damage = 0;
                // Xác định sát thương theo kiểu attack
                if (currentAttackType == 1 || currentAttackType == 2)
                {
                    damage = 20;
                }
                else if (currentAttackType == 3 || currentAttackType == 4)
                {
                    damage = 15;
                }
                else if (currentAttackType == 5)
                {
                    damage = 30;
                }

                playerHP.TakeDamage(damage);
                Debug.Log($"TerribleKnight sử dụng attack{currentAttackType} gây {damage} damage!");
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
