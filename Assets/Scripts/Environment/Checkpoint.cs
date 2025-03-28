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
            UIManager.Instance.ShowNotification();
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
				player.RecoverHealthAndStamina(1f);
                Debug.Log("Player health fully recovered.");        
            }
        }
    }
}
