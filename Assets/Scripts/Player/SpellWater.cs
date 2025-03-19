using UnityEngine;

public class SpellWater : MonoBehaviour
{
    public float speed = 15f;
    public float damage = 20f;
    public float lifeTime = 4f;

    private bool hasExploded = false;
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (rb == null)
        {
            Debug.LogError("❌ Rigidbody2D chưa được gán trên " + gameObject.name);
            return;
        }

        if (animator == null)
        {
            Debug.LogError("❌ Animator chưa được gán trên " + gameObject.name);
            return;
        }

        rb.linearVelocity = transform.right * speed;
        Invoke("DestroySpell", lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !hasExploded)
        {
            hasExploded = true;

            // Gây sát thương cho kẻ địch trúng trực tiếp
            EnemyHP enemyHP = other.GetComponent<EnemyHP>();
            if (enemyHP != null)
            {
                enemyHP.TakeDamage(damage);
                Debug.Log("💧 SpellWater trúng địch, gây " + damage + " sát thương!");
            }
            else
            {
                Debug.LogError("❌ Enemy không có script EnemyHP!");
            }

            Explode();
        }
    }

    void Explode()
    {
        animator.SetTrigger("Hit");
        rb.linearVelocity = Vector2.zero; // Ngừng di chuyển
        Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
    }

    void DestroySpell()
    {
        if (!hasExploded)
        {
            Destroy(gameObject);
        }
    }
}
