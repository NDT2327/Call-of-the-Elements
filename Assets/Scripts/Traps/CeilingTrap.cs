using UnityEngine;

public class CeilingTrap : MonoBehaviour
{
    public float fallSpeed = 5f;
    public float damage = 20f;
    private bool isFalling = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isFalling)
        {
            isFalling = true;
            rb.gravityScale = 1;
            collision.GetComponent<Health>().TakeDamage(damage);
            Debug.Log("Touch");
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject, 3f);
        }
    }
}
