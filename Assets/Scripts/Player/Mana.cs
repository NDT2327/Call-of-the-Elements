using UnityEngine;

public class Mana : MonoBehaviour
{
    private float manaPercentage = 0.15f; // Hồi 15% tổng máu

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Stamina playerMana = other.GetComponent<Stamina>();
            if (playerMana != null)
            {
                float healAmount = playerMana.MaxStamina * manaPercentage;
                playerMana.RegainStamina(healAmount); // Luôn hồi máu, kể cả khi đã đầy
                Debug.Log($"🧪 Nhân vật đã uống potion và hồi {healAmount} máu!");

                Destroy(gameObject); // Luôn xóa potion sau khi ăn
            }
        }
    }
}
