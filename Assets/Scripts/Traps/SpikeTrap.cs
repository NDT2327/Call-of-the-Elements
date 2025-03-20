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
}
