﻿using UnityEngine;

public class Fireball : MonoBehaviour
{
	[Header("Fireball Settings")]
	public float speed = 5f;       // Tốc độ bay của fireball
	public float damage = 2f;     // Lượng sát thương gây ra
	public float lifetime = 5f;    // Thời gian tồn tại của fireball
    private Health playerHP;
    private Vector2 moveDirection;
	private Animator animator;

	void Start()
	{
		// Tự hủy fireball sau một khoảng thời gian
		Destroy(gameObject, lifetime);
		playerHP = GetComponent<Health>();
		animator = GetComponent<Animator>();
	}

	void Update()
	{
		// Fireball tự bay theo hướng đã thiết lập
		transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
	}

	// Được gọi từ HellBeast để thiết lập hướng bay
	public void SetDirection(Vector2 direction)
	{
		moveDirection = direction.normalized;

		// Đảo hướng fireball nếu cần
		float scaleX = Mathf.Abs(transform.localScale.x);
		transform.localScale = new Vector3(direction.x > 0 ? scaleX : -scaleX, transform.localScale.y, transform.localScale.z);
	}

	// Xử lý va chạm
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
            animator.SetTrigger("Explore");

            var playerHP = collision.gameObject.GetComponent<Health>();
			if (playerHP != null)
			{
                Debug.Log("Gây " + damage);
				playerHP.TakeDamage(damage);
            }

            Destroy(gameObject, 0.2f); // Hủy fireball sau khi va chạm
		}
		//else if (collision.gameObject.CompareTag("Obstacle")) // Nếu va vào vật cản, cũng hủy
		//{
		//	Destroy(gameObject);
		//}
	}
}
