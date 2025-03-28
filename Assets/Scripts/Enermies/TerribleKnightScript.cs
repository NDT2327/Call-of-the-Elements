using UnityEngine;
using System.Collections;

public class TerribleKnightScript : MonoBehaviour
{
    //[Header("Audio Clips")]
    //public AudioClip attackSound;
    //public AudioClip attackSound2;
    //public AudioClip jumpSound;
    //public AudioClip runSound; // Âm thanh chạy
    //private AudioSource audioSource;

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

    [Header("Enraged Settings")]
    [Tooltip("Multiplier applied when HP below 50%")]
    public float enragedMultiplier = 1.5f;

    private float currentMultiplier = 1f;

    private Animator anim;
    private Rigidbody2D rb;
    private bool isJumping = false;

    // Lưu lại kiểu tấn công hiện tại để xác định sát thương
    private int currentAttackType = 0;

    private Health playerHP; // Dùng để gây sát thương cho player
    private EnemyHP enemyHP; // Component quản lý HP của enemy

    private enum State { Idle, Run, Attack, Jump }
    private State currentState = State.Idle;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        enemyHP = GetComponent<EnemyHP>(); // Giả sử EnemyHP được gắn trên cùng GameObject
        playerHP = GetComponent<Health>();
        //audioSource = GetComponent<AudioSource>();


        // Loại bỏ va chạm giữa enemy và player nếu có
        Collider2D enemyCollider = GetComponent<Collider2D>();
        Collider2D playerCollider = target.GetComponent<Collider2D>();
        if (enemyCollider != null && playerCollider != null)
        {
            Physics2D.IgnoreCollision(enemyCollider, playerCollider);
        }
    }

    void Update()
    {
        // Cập nhật multiplier dựa trên HP hiện tại
        if (enemyHP != null && enemyHP.GetCurrentHP() < enemyHP.maxHP * 0.5f)
        {
            currentMultiplier = enragedMultiplier;
        }
        else
        {
            currentMultiplier = 1f;
        }

        if (isJumping) return; // Nếu đang nhảy, không đổi trạng thái


        float distanceToTarget = target ? Vector2.Distance(transform.position, target.position) : Mathf.Infinity;
        bool canAttack = (Time.time - lastAttackTime) >= attackCooldown;

        // So sánh sử dụng tầm tấn công và tầm phát hiện đã được nhân với multiplier
        if (canAttack && distanceToTarget <= attackRange * currentMultiplier)
        {
            currentState = State.Attack;
            PerformAttack();
        }
        else if (distanceToTarget <= detectionRange * currentMultiplier)
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
                // Giữ nguyên vận tốc khi nhảy
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
        rb.linearVelocity = new Vector2(direction.x * moveSpeed * currentMultiplier, rb.linearVelocity.y);
        //if (!audioSource.isPlaying || audioSource.clip != runSound)
        //{
        //    audioSource.clip = runSound;
        //    audioSource.loop = true;
        //    audioSource.Play();
        //}
        FlipSprite(direction.x);
    }

    void PerformAttack()
    {
        //if (audioSource.clip == runSound)
        //{
        //    audioSource.Stop();
        //}
        lastAttackTime = Time.time;
        // Chọn kiểu tấn công từ 1 đến 5
        currentAttackType = Random.Range(1, 6);
        anim.SetTrigger($"attack{currentAttackType}");
        PlayAttackSound(currentAttackType);
        Invoke(nameof(ResetToRun), 1f);
    }

    private void PlayAttackSound(int attackType)
    {
        switch (attackType)
        {
            case 1:
            case 2:
            case 5:
                break;
            case 3:
            case 4:
				break;
            default:
                Debug.LogWarning("Không có âm thanh cho kiểu tấn công này!");
                break;
        }
    }

    private void ResetToRun()
    {
        currentState = State.Run;
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isRunning", currentState == State.Run);
        //anim.SetBool("isJumping", currentState == State.Jump);
    }

    private void FlipSprite(float moveX)
    {
        if ((moveX > 0 && transform.localScale.x < 0) ||
            (moveX < 0 && transform.localScale.x > 0))
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    public void OnPlayerJump()
    {
        if (isJumping) return;
        Debug.Log("jump");
        if (Random.value < 0.3f) // 30% cơ hội nhảy khi Player nhảy
        {
            Jump();
        }
    }

    private void Jump()
    {
        //if (audioSource.clip == runSound)
        //{
        //    audioSource.Stop();
        //}
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

    // Hàm được gọi qua Animation Event tại thời điểm gây sát thương trong animation tấn công
    public void DoDamageCloseRange()
    {
        // Kiểm tra collider trong phạm vi attackRange (có nhân multiplier)
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange * currentMultiplier, targetMask);
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

                // Áp dụng multiplier nếu enemy đang trong trạng thái Enraged
                damage = Mathf.RoundToInt(damage * currentMultiplier);

                playerHP.TakeDamage(damage);
                Debug.Log($"TerribleKnight sử dụng attack{currentAttackType} gây {damage} damage!");
            }
        }
    }

    // Vẽ Gizmos cho detection và attack range (cho mục đích Debug trong Editor)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
