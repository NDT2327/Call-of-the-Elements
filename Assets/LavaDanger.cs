using System.Collections;
using UnityEngine;

public class LavaDanger : MonoBehaviour
{
    public int damagePerSecond = 10;
    public float damagerInterval = 1f;
    private bool isPlayerInLava = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInLava = true;
            StartCoroutine(DealDamageOverTime(collision.gameObject.GetComponent<Health>()));
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInLava = false;
        }
    }

    IEnumerator DealDamageOverTime(Health health)
    {
        while(isPlayerInLava && health != null)
        {
            health.TakeDamage(damagePerSecond);
            yield return new WaitForSeconds(damagerInterval);
        }
    }
}
