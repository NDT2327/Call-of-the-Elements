using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
	[Header("Target & Range")]
	public Transform target; // Thường là Player
	public float detectionRange = 5f;

	[Header("Layer Settings")]
	public LayerMask obstacleMask; // Lớp vật cản (Obstacle)
	public LayerMask targetMask;   // Lớp target (Player)

	// Kiểm tra xem enemy có nhìn thấy target không
	public bool CanSeeTarget()
	{
		if (target == null)
			return false;

		Vector2 direction = (target.position - transform.position).normalized;
		float distance = Vector2.Distance(transform.position, target.position);

		if (distance <= detectionRange)
		{
			// Dùng Raycast để kiểm tra nếu có vật cản giữa enemy và target
			RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, obstacleMask | targetMask);
			if (hit)
			{
				// Nếu collider va chạm thuộc lớp target thì trả về true
				if (((1 << hit.collider.gameObject.layer) & targetMask) != 0)
				{
					return true;
				}
			}
		}
		return false;
	}
}
