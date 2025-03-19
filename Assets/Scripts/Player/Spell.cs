using UnityEngine;

public class Spell : MonoBehaviour
{
    public float damage = 20f;        // Sát thương của spell
    public float knockUpForce = 10f;  // Lực hất tung enemy

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Đã va chạm với: " + collision.gameObject.name);
        if (collision.CompareTag("Enemy"))
        {
            // Gây sát thương cho Enemy
            EnemyHP enemyHP = collision.GetComponent<EnemyHP>();
            if (enemyHP != null)
            {
                enemyHP.TakeDamage(damage);
            }

            // Hất tung enemy lên trên
            Rigidbody2D enemyRb = collision.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                enemyRb.linearVelocity = new Vector2(enemyRb.linearVelocity.x, 0); // Reset tốc độ dọc
                enemyRb.AddForce(Vector2.up * knockUpForce, ForceMode2D.Impulse);
            }

            Debug.Log("Spell hất tung " + collision.gameObject.name);
        }
    }
}
