using UnityEngine;

public class UpgradeItem : MonoBehaviour
{
    public GameManager.Map unlockMap;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.UnlockSpecialAttack(unlockMap);
                Destroy(gameObject);
            }
        }
    }
}
