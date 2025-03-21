using UnityEngine;

public class Ultimate : MonoBehaviour
{
    public float speed = 15f;
    public float damage = 15f;
    public float lifeTime = 5f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("❌ Rigidbody2D chưa được gán trên " + gameObject.name);
            return;
        }

        rb.linearVelocity = transform.right * speed; // Đạn bay thẳng về phía trước
        Invoke("DestroySpell", lifeTime); // Hủy sau thời gian tối đa
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHP enemyHP = other.GetComponent<EnemyHP>();
            if (enemyHP != null)
            {
                enemyHP.TakeDamage(damage);
                Debug.Log($"⚔ Ultimate gây {damage} sát thương lên {other.gameObject.name}!");
            }
        }
    }

    void DestroySpell()
    {
        Destroy(gameObject);
    }
}
