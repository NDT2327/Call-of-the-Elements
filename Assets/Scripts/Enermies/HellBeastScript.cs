using UnityEngine;
using UnityEngine.Audio;

public class HellBeastScript : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip attackSound;
    public AudioClip attackSound2;
    //public AudioClip jumpSound;
    //public AudioClip runSound; // Âm thanh chạy
    private AudioSource audioSource;

    [Header("Movement & Detection")]
	public Transform target;          // Thường là Player
	public float detectionRange = 5f; // Tầm phát hiện Player
	public float chaseSpeed = 2f;     // Tốc độ di chuyển khi đuổi
	public LayerMask obstacleMask;    // Layer vật cản
	public LayerMask targetMask;      // Layer target (Player)

	[Header("Attack Settings")]
	public float closeAttackRange = 1f;  // Phạm vi cho đòn Burn
	public float breathAttackRange = 3f; // Phạm vi cho đòn Breath
	public float attackCooldown = 2f;    // Thời gian hồi giữa các lần tấn công
	private float lastAttackTime = 0f;   // Thời điểm cuối cùng đã tấn công

	[Header("Fireball Settings")]
	public GameObject fireballPrefab;     // Prefab của fireball
	public Transform fireballSpawnPoint;  // Vị trí spawn fireball

	private Animator anim;
	private Rigidbody2D rb;
    private Health playerHP;
    private enum State { Idle, Chase, Attack }
	private State currentState = State.Idle;

	void Start()
	{
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnermyLayer"), LayerMask.NameToLayer("EnermyLayer"));
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		playerHP = GetComponent<Health>();
        audioSource = GetComponent<AudioSource>();

        // Bỏ qua va chạm giữa enemy và player
        Collider2D enemyCollider = GetComponent<Collider2D>();
        Collider2D playerCollider = target.GetComponent<Collider2D>();
        if (enemyCollider != null && playerCollider != null)
        {
            Physics2D.IgnoreCollision(enemyCollider, playerCollider);
        }
    }

	void Update()
	{
		if (target == null) return;

		if (currentState != State.Attack) // Không đổi sang Chase nếu đang Attack
		{
			// Kiểm tra xem HellBeast có nhìn thấy target không
			if (CanSeeTarget())
			{
				// Nếu thấy target => chuyển sang Chase
				currentState = State.Chase;
			}
			else
			{
				// Nếu không thấy => chuyển về Idle (hoặc tuỳ bạn muốn quay về Patrol v.v.)
				currentState = State.Idle;
			}
		}
		// Tính khoảng cách để quyết định tấn công
		float distanceToTarget = Vector2.Distance(transform.position, target.position);
		bool canAttack = Time.time - lastAttackTime >= attackCooldown;

		if (canAttack && currentState != State.Attack)
		{
			// Nếu ở rất gần => Burn
			if (distanceToTarget <= closeAttackRange)
			{
				currentState = State.Attack;
				AttackBurn();
			}
			// Nếu trong khoảng tầm xa => Breath
			else if (distanceToTarget <= breathAttackRange)
			{
				currentState = State.Attack;
				AttackBreath();
			}
		}
	}

	void FixedUpdate()
	{
		if (currentState == State.Chase)
		{
			ChaseTarget();
		}
		else
		{
			// Nếu Idle hoặc Attack, dừng di chuyển
			rb.linearVelocity = Vector2.zero;
		}
	}

	// Hàm đuổi theo target
	private void ChaseTarget()
	{
		// Tính hướng và di chuyển
		Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y);

        // Flip sprite nếu cần
        if ((direction.x > 0 && transform.localScale.x < 0) ||
			(direction.x < 0 && transform.localScale.x > 0))
		{
			Flip();
		}
	}

	// Hàm tấn công cận chiến
	private void AttackBurn()
	{
		currentState = State.Attack; // Đặt trạng thái thành Attack
		anim.SetTrigger("burnAttack");
		lastAttackTime = Time.time;
		if (audioSource != null && attackSound != null)
		{
			audioSource.PlayOneShot(attackSound);
		}
		Invoke("ResetToChase", 1f); // Đợi 1 giây rồi reset
	}

	// Hàm tấn công xa (thở lửa)
	private void AttackBreath()
	{
		
		currentState = State.Attack;
		anim.SetTrigger("breathAttack");
		lastAttackTime = Time.time;
		// Phát âm thanh khi tấn công Breath
		if (audioSource != null && attackSound2 != null)
		{
			audioSource.PlayOneShot(attackSound2);
		}        // Gọi hàm SpawnFireball để tạo quả cầu lửa
		Invoke("SpawnFireball", 1.1f); // Delay một chút cho hợp với animation
		Invoke("ResetToChase", 1.5f);
	}

	// Reset trạng thái về Chase sau khi tấn công xong
	private void ResetToChase()
	{
		currentState = State.Chase;
	}

	private void SpawnFireball()
	{
		if (fireballPrefab == null || fireballSpawnPoint == null)
		{
			Debug.LogError("Fireball Prefab hoặc Spawn Point chưa được gán!");
			return;
		}

		// Tạo fireball
		GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);

		// Lấy script Fireball và đặt hướng bay
		Fireball fireballScript = fireball.GetComponent<Fireball>();
		if (fireballScript != null)
		{
			// Xác định hướng bay dựa trên hướng của Hell Beast
			Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
			fireballScript.SetDirection(direction);
		}
		else
		{
			Debug.LogError("Fireball không có script Fireball attached!");
		}
	}



	// Gây sát thương cận chiến (nếu dùng Animation Event)
	public void DoDamageCloseRange()
	{
		// Kiểm tra xem có collider nào nằm trong phạm vi closeAttackRange và thuộc targetMask không
		Collider2D hit = Physics2D.OverlapCircle(transform.position, closeAttackRange, targetMask);
		if (hit != null)
		{
            // Tại đây bạn có thể gọi các hàm để gây sát thương lên đối tượng
            playerHP = hit.GetComponent<Health>();
            if (playerHP != null)
			{

				playerHP.TakeDamage(30); // Gây 1 damage
                Debug.Log("HellBeast gây sát thương Burn cận chiến!");
            }
		}
	}

	// Gây sát thương tầm xa (hoặc bắn projectile)
	public void DoDamageLongRange()
	{
		Debug.Log("HellBeast phun lửa tầm xa (Breath)!");
		// Tại đây bạn có thể tạo ra projectile, hoặc đốt lửa AoE, v.v.
	}

	// Kiểm tra xem có nhìn thấy target hay không (bị cản hay không)
	private bool CanSeeTarget()
	{
		float distance = Vector2.Distance(transform.position, target.position);
		if (distance > detectionRange) return false;

		Vector2 direction = (target.position - transform.position).normalized;
		RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, obstacleMask | targetMask);
		if (hit)
		{
			// Nếu collider trúng thuộc layer target => thấy target
			if (((1 << hit.collider.gameObject.layer) & targetMask) != 0)
			{
				return true;
			}
		}
		return false;
	}

	// Đảo hướng sprite
	private void Flip()
	{
		Vector3 localScale = transform.localScale;
		localScale.x *= -1;
		transform.localScale = localScale;
	}

	// Debug vẽ tầm phát hiện trong Scene View
	void OnDrawGizmosSelected()
	{
		// Vẽ vòng tròn detectionRange
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, detectionRange);

		// Vẽ vòng tròn closeAttackRange
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, closeAttackRange);

		// Vẽ vòng tròn breathAttackRange
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.position, breathAttackRange);
	}
}
