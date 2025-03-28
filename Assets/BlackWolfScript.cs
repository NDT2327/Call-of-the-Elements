using UnityEngine;
using System.Collections;

public class BlackWolfScript : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip attackSound;
    //public AudioClip attackSound2;
    //public AudioClip jumpSound;
    public AudioClip runSound; // Âm thanh chạy
    private AudioSource audioSource;

    [Header("Patrol Settings")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 2f;
    public float idleTime = 2f; // Thời gian đứng yên mỗi lần Idle

    // Khoảng thời gian ngẫu nhiên giữa 2 lần Idle
    public float idleIntervalMin = 3f;
    public float idleIntervalMax = 6f;
    private float nextIdleTime = 0f;

    [Header("Movement & Detection")]
    public Transform target;
    public float detectionRange = 5f;
    public float chaseSpeed = 3f;
    public LayerMask obstacleMask;
    public LayerMask targetMask;

    [Header("Attack Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    private float lastAttackTime = 0f;

    private Animator anim;
    private Rigidbody2D rb;
    private bool isDead = false;
    private bool isIdling = false; // Kiểm soát Idle tạm thời
    private Health playerHP;


    private enum State { Idle, Walk, Run, Attack, Hurt, Die }
    private State currentState = State.Idle;

    private Transform currentPatrolTarget;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerHP = GetComponent<Health>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource was missing, adding one to " + gameObject.name);
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Bắt đầu tuần tra từ pointA
        if (pointA != null) currentPatrolTarget = pointA;

        // Xác định thời điểm Idle đầu tiên (random)
        nextIdleTime = Time.time + Random.Range(idleIntervalMin, idleIntervalMax);
        Collider2D enemyCollider = GetComponent<Collider2D>();
        Collider2D playerCollider = target.GetComponent<Collider2D>();
        if (enemyCollider != null && playerCollider != null)
        {
            Physics2D.IgnoreCollision(enemyCollider, playerCollider);
        }
    }

    void Update()
    {
        if (isDead) return;
        if (target == null) return;

        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        bool canAttack = (Time.time - lastAttackTime) >= attackCooldown;

        // Nếu đang Idle thì không ghi đè trạng thái
        if (isIdling)
        {
            UpdateAnimations();
            return;
        }

        // Nếu có thể tấn công
        if (canAttack && distanceToTarget <= attackRange)
        {
            currentState = State.Attack;
            anim.SetBool("isRunning", false);
            PerformAttack();
            rb.linearVelocity = Vector2.zero;
        }

        // Nếu đang không Attack
        if (currentState != State.Attack)
        {
            // Nếu trong tầm phát hiện
            if (distanceToTarget <= detectionRange)
            {
                currentState = State.Run;
            }
            else
            {
                // Không thấy target => quay về tuần tra
                currentState = State.Walk;
            }
            UpdateAnimations();
        }
    }

    void FixedUpdate()
    {
        if (isDead || isIdling) return;

        switch (currentState)
        {
            case State.Walk:
                Patrol();
                break;
            case State.Run:
                ChaseTarget();
                break;
            case State.Attack:
            case State.Hurt:
            case State.Die:
            case State.Idle:
                rb.linearVelocity = Vector2.zero;
                break;
        }
    }

    // ----------------- PATROL -----------------
    void Patrol()
    {
		if (audioSource.clip == runSound)
		{
			audioSource.Stop();
		}

		if (pointA == null || pointB == null) return;

        // 1) Kiểm tra xem đã đến lúc Idle ngẫu nhiên chưa
        if (Time.time >= nextIdleTime)
        {
            StartCoroutine(RandomIdle());
            return;
        }

        // 2) Nếu chưa tới lúc Idle => tiếp tục di chuyển
        Vector2 direction = (currentPatrolTarget.position - transform.position).normalized;
        rb.linearVelocity = direction * patrolSpeed;
        FlipSprite(direction.x);

        // Khi tới gần điểm tuần tra
        if (Vector2.Distance(transform.position, currentPatrolTarget.position) < 0.2f)
        {
            // Chuyển sang điểm còn lại
            currentPatrolTarget = (currentPatrolTarget == pointA) ? pointB : pointA;
        }
    }

    IEnumerator RandomIdle()
    {
        isIdling = true;
        currentState = State.Idle;
        rb.linearVelocity = Vector2.zero;
        UpdateAnimations();

        // Idle trong idleTime giây
        yield return new WaitForSeconds(idleTime);

        // Sau khi Idle xong, tính lại thời điểm Idle tiếp theo
        nextIdleTime = Time.time + Random.Range(idleIntervalMin, idleIntervalMax);

        // Quay lại state Walk
        isIdling = false;
        currentState = State.Walk;
        UpdateAnimations();
    }

    // ----------------- CHASE -----------------
    void ChaseTarget()
    {
        if (target == null) return;
        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y);
        if (!audioSource.isPlaying || audioSource.clip != runSound)
        {
            audioSource.clip = runSound;
            audioSource.loop = true;
            audioSource.pitch = 3f;
            audioSource.Play();
        }
        FlipSprite(direction.x);
    }

    // ----------------- ATTACK -----------------
    void PerformAttack()
    {
        if (audioSource.clip == runSound)
        {
            audioSource.Stop();
        }
        lastAttackTime = Time.time;
        int attackType = Random.Range(1, 5);
        anim.SetTrigger($"attack{attackType}");
        audioSource.PlayOneShot(attackSound);

        // Sau 1 giây, quay lại chạy
        Invoke(nameof(ResetToRun), 1f);
    }

    public void DoDamageCloseRange()
    {
        // Kiểm tra xem có collider nào nằm trong phạm vi closeAttackRange và thuộc targetMask không
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, targetMask);
        if (hit != null)
        {

            // Kiểm tra xem đối tượng có component Health không
            playerHP = hit.GetComponent<Health>();
            if (playerHP != null)
            {
                playerHP.TakeDamage(10); // Gây 10 damage
                Debug.Log("Golem gây sát thương cận chiến!");
            }
        }
    }

    private void ResetToRun()
    {
        currentState = State.Run;
    }

    // ----------------- ANIMATIONS -----------------
    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", currentState == State.Walk);
        anim.SetBool("isRunning", currentState == State.Run);
    }

    private void FlipSprite(float moveX)
    {
        if (moveX > 0 && transform.localScale.x < 0 ||
            moveX < 0 && transform.localScale.x > 0)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    // ----- DEBUG GIZMOS -----
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(pointA.position, 0.2f);
            Gizmos.DrawWireSphere(pointB.position, 0.2f);
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
