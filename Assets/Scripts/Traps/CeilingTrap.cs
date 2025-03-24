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
        rb.gravityScale = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isFalling)
        {
            isFalling = true;
            rb.gravityScale = 1;
        }

    }

    public void ActivateTrap()
    {
        isFalling = true;
        rb.gravityScale = 1;
        Debug.Log("Bay tran da duoc kich hoat");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {

                playerHealth.TakeDamage(damage);
                Debug.Log("Touch");
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider2D>().bounds.size);
    }

}
