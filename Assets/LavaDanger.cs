using System.Collections;
using UnityEngine;

public class LavaDanger : MonoBehaviour
{
    public int damagePerSecond = 3;
    public float damagerInterval = 1f;
    private bool isPlayerInLava = false;
    private Health health;
    private void Awake()
    {
        health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        Debug.Log("Player entered lava!");

    //        isPlayerInLava = true;
    //        StartCoroutine(DealDamageOverTime());
    //    }
    //}

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        isPlayerInLava = false;
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log("Player entered lava!");

            isPlayerInLava = true;
            StartCoroutine(DealDamageOverTime());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInLava = false;
        }
    }

    public IEnumerator DealDamageOverTime()
    {
        //Debug.Log("burn");
        //health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        while(isPlayerInLava && health != null)
        {
            health.TakeDamage(damagePerSecond);
            Debug.Log("Player take damage");
            yield return new WaitForSeconds(damagerInterval);
        }
    }
}
