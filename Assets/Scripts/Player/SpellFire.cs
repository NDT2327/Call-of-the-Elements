using UnityEngine;

public class SpellFire : MonoBehaviour
{
    public float duration = 3f; // Thời gian tồn tại của vòng lửa
    public float damage = 20f; // Lượng sát thương gây ra khi kẻ địch chạm vào

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Tìm Player
        if (player == null)
        {
            Debug.LogError("❌ Không tìm thấy Player!");
            Destroy(gameObject);
            return;
        }

        transform.position = player.position; // Đặt vòng lửa ở vị trí của Player
        Invoke("DestroySpell", duration); // Hủy vòng lửa sau thời gian tồn tại
    }

    void Update()
    {
        if (player != null)
        {
            transform.position = player.position; // Giữ vòng lửa theo Player
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // Nếu kẻ địch chạm vào vòng lửa
        {
            EnemyHP enemyHP = other.GetComponent<EnemyHP>();
            if (enemyHP != null)
            {
                enemyHP.TakeDamage(damage);
                Debug.Log("🔥 Gây " + damage + " sát thương lên " + other.gameObject.name);
            }
        }
    }

    void DestroySpell()
    {
        Destroy(gameObject); // Xóa vòng lửa sau thời gian tồn tại
    }
}
