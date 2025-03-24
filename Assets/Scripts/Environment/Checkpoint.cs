using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isActivated)
        {
            isActivated = true;
            Debug.Log("Checkpoint activated at position: " + transform.position);
            GameManager.Instance.SetCheckpoint(transform.position);
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.RecoverFullHealth();
                Debug.Log("Player health fully recovered.");
            }
        }
    }
}
