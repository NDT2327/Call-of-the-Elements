using UnityEngine;

public class FireSkullEnemy : MonoBehaviour
{
	[Header("Patrol Settings")]
	public Transform pointA;
	public Transform pointB;
	public float patrolSpeed = 2f;

	[Header("Chase Settings")]
	public Transform target;         // Th??ng l� transform c?a Player
	public float chaseSpeed = 3f;
	public float detectionRange = 5f;
	public LayerMask obstacleMask;   // Layer ch?a v?t c?n
	public LayerMask targetMask;     // Layer ch?a target (Player)

	private Rigidbody2D rb;
	private Animator anim;
	private Transform currentPatrolTarget;
    private Health playerHP;


    // C�c tr?ng th�i c?a enemy
    private enum State { Patrol, Chase, Return }
	private State currentState = State.Patrol;

	void Start()
	{
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnermyLayer"), LayerMask.NameToLayer("EnermyLayer"));
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		playerHP = GetComponent<Health>();
		// B?t ??u tu?n tra t? ?i?m B (ho?c b?n c� th? ch?n ?i?m A)
		currentPatrolTarget = pointB;
		anim.SetBool("isFight", false);
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
		// N?u c� target v� ???c ph�t hi?n th� chuy?n tr?ng th�i sang Chase
		if (target != null && CanSeeTarget())
		{
			currentState = State.Chase;
			anim.SetBool("isFight", true);
		}
		else
		{
			// N?u tr??c ?� ?ang chase nh?ng b�y gi? kh�ng ph�t hi?n ???c target th� chuy?n sang Return
			if (currentState == State.Chase)
			{
				currentState = State.Return;
				anim.SetBool("isFight", false);
			}
		}

		// Th?c hi?n h�nh ??ng theo tr?ng th�i hi?n t?i
		switch (currentState)
		{
			case State.Patrol:
				Patrol();
				break;
			case State.Chase:
				Chase();
				break;
			case State.Return:
				ReturnToPatrol();
				break;
		}
	}

	// H�nh vi tu?n tra gi?a 2 ?i?m
	void Patrol()
	{
		if (Vector2.Distance(transform.position, currentPatrolTarget.position) < 0.2f)
		{
			// Chuy?n ??i ?i?m tu?n tra
			currentPatrolTarget = currentPatrolTarget == pointA ? pointB : pointA;
			Flip();
		}
		Vector2 direction = (currentPatrolTarget.position - transform.position).normalized;
		rb.linearVelocity = direction * patrolSpeed;
	}

	// H�nh vi ?u?i theo target (Player)
	void Chase()
	{
		Vector2 direction = (target.position - transform.position).normalized;
		rb.linearVelocity = direction * chaseSpeed;

		// ??o h??ng sprite n?u c?n
		if ((direction.x > 0 && transform.localScale.x < 0) ||
			(direction.x < 0 && transform.localScale.x > 0))
		{
			Flip();
		}
	}

	// Quay l?i ?i?m tu?n tra khi m?t target
	void ReturnToPatrol()
	{
		// Ch?n ?i?m tu?n tra g?n nh?t ?? quay v?
		Transform nearest = Vector2.Distance(transform.position, pointA.position) < Vector2.Distance(transform.position, pointB.position) ? pointA : pointB;
		if (Vector2.Distance(transform.position, nearest.position) < 0.2f)
		{
			currentState = State.Patrol;
			return;
		}
		Vector2 direction = (nearest.position - transform.position).normalized;
		rb.linearVelocity = direction * patrolSpeed;
		if ((direction.x > 0 && transform.localScale.x < 0) ||
			(direction.x < 0 && transform.localScale.x > 0))
		{
			Flip();
		}
	}

	// H�m ki?m tra c� th? nh�n th?y target kh�ng (kh�ng b? che b?i v?t c?n)
	bool CanSeeTarget()
	{
		Vector2 direction = (target.position - transform.position).normalized;
		float distance = Vector2.Distance(transform.position, target.position);
		if (distance <= detectionRange)
		{
			RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, obstacleMask | targetMask);
			if (hit)
			{
				// N?u collider tr�ng thu?c l?p target th� tr? v? true
				if (((1 << hit.collider.gameObject.layer) & targetMask) != 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	// H�m ??o chi?u sprite khi thay ??i h??ng di chuy?n
	void Flip()
	{
		Vector3 localScale = transform.localScale;
		localScale.x *= -1;
		transform.localScale = localScale;
	}

    public void DoDamageCloseRange()
    {
        // Kiểm tra xem có collider nào nằm trong phạm vi closeAttackRange và thuộc targetMask không
        Collider2D hit = Physics2D.OverlapCircle(transform.position, targetMask);
        if (hit != null)
        {
            // Tại đây bạn có thể gọi các hàm để gây sát thương lên đối tượng
            playerHP = hit.GetComponent<Health>();
            if (playerHP != null)
            {

                playerHP.TakeDamage(3); // Gây 1 damage
                Debug.Log("HellBeast gây sát thương Burn cận chiến!");
            }
        }
    }

    // V? Gizmos ?? hi?n th? v�ng ?i?m trong Scene view (ch? hi?n th? khi ch?a ch?y game)
    void OnDrawGizmos()
	{
		// Vẽ vòng tròn detectionRange
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, detectionRange);

		if (pointA != null && pointB != null)
		{
			Gizmos.DrawWireSphere(pointA.position, 0.2f);
			Gizmos.DrawWireSphere(pointB.position, 0.2f);
			Gizmos.DrawLine(pointA.position, pointB.position);
		}
	}
}
