using UnityEngine;

public class Potion : MonoBehaviour
{
    private float healPercentage = 0.15f; // Hồi 15% tổng máu
   
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                float healAmount = playerHealth.MaxHealth * healPercentage;
                playerHealth.Heal(healAmount); // Luôn hồi máu, kể cả khi đã đầy
                Debug.Log($"🧪 Nhân vật đã uống potion và hồi {healAmount} máu!");

                Destroy(gameObject); // Luôn xóa potion sau khi ăn
            }
        }
    }
}
