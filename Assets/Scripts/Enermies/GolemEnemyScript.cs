using UnityEngine;

public class GolemEnemyScript : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 2f;

    [Header("Movement & Detection")]
    public Transform target;
    public float detectionRange = 5f;
    public float chaseSpeed = 2f;
    public LayerMask obstacleMask;
    public LayerMask targetMask;

    [Header("Attack Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    private float lastAttackTime = 0f;

    private Animator anim;
    private Rigidbody2D rb;
    private EnemyHP enemyHP;
    private bool isDead = false;
    private Health playerHP;

    // Thay Idle thành Patrol
    private enum State { Patrol, Chase, Attack, Hurt, Die }
    private State currentState = State.Patrol;

    private Transform currentPatrolTarget;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        enemyHP = GetComponent<EnemyHP>();
        playerHP = GetComponent<Health>();

        // Bắt đầu tuần tra từ pointA
        currentPatrolTarget = pointA;
    }

    void Update()
    {
        if (isDead) return;
        if (target == null) return;

        // Tính toán khoảng cách tới target
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        bool canAttack = (Time.time - lastAttackTime) >= attackCooldown;

        // Quyết định trạng thái
        if (canAttack && distanceToTarget <= attackRange)
        {
            currentState = State.Attack;
            PerformAttack();
            rb.linearVelocity = Vector2.zero;

        }

        if (currentState != State.Attack) // Không đổi sang Chase nếu đang Attack
        {
            if (distanceToTarget <= detectionRange)
            {
                currentState = State.Chase;
            }
            else
            {
                // Không thấy target => quay về tuần tra
                currentState = State.Patrol;
            }
            UpdateAnimations();

        }
    }

    void FixedUpdate()
    {
        // Xử lý chuyển động tuỳ theo state
        switch (currentState)
        {
            case State.Chase:
                ChaseTarget();
                break;
            case State.Patrol:
                Patrol();
                break;
            default:
                // Attack, Hurt, Die => dừng di chuyển
                rb.linearVelocity = Vector2.zero;
                break;
        }
    }

    // ----- PATROL -----
    void Patrol()
    {
        // Tính hướng di chuyển đến điểm patrol hiện tại
        Vector2 direction = (currentPatrolTarget.position - transform.position).normalized;

        // Cập nhật hướng sprite dựa theo hướng di chuyển
        FlipPatrol(-direction);

        // Di chuyển theo hướng tính được
        rb.linearVelocity = direction * patrolSpeed;

        // Khi gần điểm patrol hiện tại thì chuyển sang điểm còn lại
        if (Vector2.Distance(transform.position, currentPatrolTarget.position) < 0.2f)
        {
            currentPatrolTarget = (currentPatrolTarget == pointA) ? pointB : pointA;
        }
    }


    // Hàm Flip khi đổi hướng tuần tra
    private void FlipPatrol(Vector2 moveDirection)
    {
        // Nếu đang di chuyển sang phải nhưng sprite đang quay trái, hoặc ngược lại
        if ((moveDirection.x > 0 && transform.localScale.x < 0) ||
            (moveDirection.x < 0 && transform.localScale.x > 0))
        {
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }


    // ----- CHASE -----
    private void ChaseTarget()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y); ;

        // Flip dựa theo hướng target
        FlipChase(-direction.x);
    }

    private void FlipChase(float directionX)
    {
        // Nếu directionX > 0 mà localScale.x < 0 => lật
        // Hoặc directionX < 0 mà localScale.x > 0 => lật
        if ((directionX > 0 && transform.localScale.x < 0) ||
            (directionX < 0 && transform.localScale.x > 0))
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    // ----- ATTACK -----
    private void PerformAttack()
    {
        lastAttackTime = Time.time;
        // Random kiểu tấn công 1,2,3
        int attackType = Random.Range(1, 4);
        anim.SetTrigger($"attack{attackType}");
        Invoke("ResetToChase", 1f);

    }

    public void DoDamageCloseRange()
    {
        // Kiểm tra xem có collider nào nằm trong phạm vi closeAttackRange và thuộc targetMask không
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, targetMask);
        if (hit != null)
        {
            Debug.Log("Golem gây sát thương cận chiến!");
            // Kiểm tra xem đối tượng có component Health không
            playerHP = hit.GetComponent<Health>();
            if (playerHP != null)
            {
                playerHP.TakeDamage(1); // Gây 1 damage
            }
        }
    }

    // Gọi từ Animation Event hoặc khi Golem bị tấn công
    //public void TakeDamage(float damage)
    //{
    //    if (isDead) return;

    //    enemyHP.TakeDamage(damage); // Gọi từ EnemyHP

    //    if (enemyHP.GetCurrentHP() <= 0)
    //    {
    //        Die();
    //        return;
    //    }

    //    currentState = State.Hurt;
    //    anim.SetTrigger("hurt");

    //    // Ví dụ: Sau khi bị thương 1s thì quay lại Chase (hoặc Patrol)
    //    Invoke("ResetToChase", 1f);
    //}

    //// Khi chết
    //public void Die()
    //{
    //    if (isDead) return;
    //    isDead = true;
    //    currentState = State.Die;
    //    anim.SetTrigger("die");
    //    rb.linearVelocity = Vector2.zero;
    //    Destroy(gameObject,1f);
    //}

    private void ResetToChase()
    {
        currentState = State.Chase;
    }

    // ----- ANIMATIONS -----
    private void UpdateAnimations()
    {
        // isRunning true nếu ở trạng thái Chase
        bool isRunning = (currentState == State.Chase);
        anim.SetBool("isRunning", isRunning);
    }

    // ----- DEBUG GIZMOS -----
    void OnDrawGizmos()
    {
        // Vẽ vòng tròn detectionRange
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Vẽ vòng tròn attackRange
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(pointA.position, 0.2f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(pointB.position, 0.2f);
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
