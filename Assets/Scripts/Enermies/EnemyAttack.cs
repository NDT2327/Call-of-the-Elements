using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
	[Header("Attack Settings")]
	public Transform target; // Player
	public float attackRange = 1f;
	public float damage = 10f;
	public float attackCooldown = 1f;

	private float lastAttackTime;

	void Update()
	{
		if (target == null)
			return;

		float distance = Vector2.Distance(transform.position, target.position);
		if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
		{
			//Attack();
		}
	}

	//void Attack()
	//{
	//	// Giả sử Player có script PlayerHealth với phương thức TakeDamage
	//	PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
	//	if (playerHealth != null)
	//	{
	//		playerHealth.TakeDamage(damage);
	//	}
	//	lastAttackTime = Time.time;
	//}
}
