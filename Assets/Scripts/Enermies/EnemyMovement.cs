using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
	[Header("Patrol Settings")]
	public Transform patrolPointA;
	public Transform patrolPointB;
	public float patrolSpeed = 2f;

	[Header("Chase Settings")]
	public float chaseSpeed = 3f;
	public Transform target; // Player

	[Header("Detection")]
	public EnemyDetection detection; // Tham chi?u ??n script EnemyDetection

	private Transform currentPatrolTarget;
	private Rigidbody2D rb;

	// C�c tr?ng th�i c?a enemy
	private enum State { Patrol, Chase, Return }
	private State currentState = State.Patrol;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		currentPatrolTarget = patrolPointB;
		if (detection == null)
		{
			detection = GetComponent<EnemyDetection>();
		}
	}

	void Update()
	{
		// N?u c� target v� ???c ph�t hi?n, chuy?n tr?ng th�i sang Chase
		if (target != null && detection != null && detection.CanSeeTarget())
		{
			currentState = State.Chase;
		}
		else
		{
			// N?u tr??c ?� ?ang ?u?i theo nh?ng b�y gi? m?t target th� chuy?n sang Return
			if (currentState == State.Chase)
			{
				currentState = State.Return;
			}
		}

		// Th?c hi?n h�nh vi d?a theo tr?ng th�i hi?n t?i
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

	// H�nh vi tu?n tra
	void Patrol()
	{
		if (Vector2.Distance(transform.position, currentPatrolTarget.position) < 0.2f)
		{
			// Chuy?n ??i ?i?m tu?n tra
			currentPatrolTarget = currentPatrolTarget == patrolPointA ? patrolPointB : patrolPointA;
			Flip();
		}
		Vector2 direction = (currentPatrolTarget.position - transform.position).normalized;
		rb.linearVelocity = direction * patrolSpeed;
	}

	// H�nh vi ?u?i theo Player
	void Chase()
	{
		Vector2 direction = (target.position - transform.position).normalized;
		rb.linearVelocity = direction * chaseSpeed;
		// ??o chi?u sprite d?a tr�n h??ng di chuy?n
		if ((direction.x > 0 && transform.localScale.x < 0) || (direction.x < 0 && transform.localScale.x > 0))
		{
			Flip();
		}
	}

	// Quay l?i ch? ?? tu?n tra khi m?t target
	void ReturnToPatrol()
	{
		// Ch?n ?i?m tu?n tra g?n nh?t ?? quay v?
		Transform nearest = Vector2.Distance(transform.position, patrolPointA.position) < Vector2.Distance(transform.position, patrolPointB.position) ? patrolPointA : patrolPointB;
		if (Vector2.Distance(transform.position, nearest.position) < 0.2f)
		{
			currentState = State.Patrol;
			return;
		}
		Vector2 direction = (nearest.position - transform.position).normalized;
		rb.linearVelocity = direction * patrolSpeed;
		if ((direction.x > 0 && transform.localScale.x < 0) || (direction.x < 0 && transform.localScale.x > 0))
		{
			Flip();
		}
	}

	// H�m ??o chi?u sprite khi chuy?n h??ng
	void Flip()
	{
		Vector3 localScale = transform.localScale;
		localScale.x *= -1;
		transform.localScale = localScale;
	}
}
