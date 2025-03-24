using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public float damage = 20f;
    private bool isPlayerOnTrap = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerOnTrap = true;
            collision.GetComponent<Health>().TakeDamage(damage);
            Debug.Log("Take damage");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerOnTrap = false;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider2D>().bounds.size);
    }

}
