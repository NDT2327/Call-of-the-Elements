using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private EnemyHP bossHealth; // Thêm biến quản lý máu Boss
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currentHealthBar;
    [SerializeField] private Image totalEnemyHealthBar;
    [SerializeField] private Image currentEnemyHealthBar;
    [SerializeField] private Image element;

    [SerializeField] private Sprite fireSprite;
    [SerializeField] private Sprite earthSprite;

    private int currentElementIndex = 0; // Mặc định là Lửa (Fire)
    private Sprite[] elementSprites;

    void Start()
    {
        Debug.Log("🏁 Health Bar Script Started!");
        Debug.Log($"📌 PlayerHealth: {playerHealth}, BossHealth: {bossHealth}");
        if (playerHealth == null)
            Debug.LogError("❌ playerHealth chưa được gán trong Inspector!", this);

        if (bossHealth == null)
            Debug.LogError("❌ bossHealth chưa được gán trong Inspector!", this);

        if (totalHealthBar == null)
            Debug.LogError("❌ totalHealthBar chưa được gán trong Inspector!", this);

        if (currentHealthBar == null)
            Debug.LogError("❌ currentHealthBar chưa được gán trong Inspector!", this);

        if (totalEnemyHealthBar == null)
            Debug.LogError("❌ totalEnemyHealthBar chưa được gán trong Inspector!", this);

        if (currentEnemyHealthBar == null)
            Debug.LogError("❌ currentEnemyHealthBar chưa được gán trong Inspector!", this);

        if (element == null)
            Debug.LogError("❌ element chưa được gán trong Inspector!", this);

        if (fireSprite == null || earthSprite == null)
            Debug.LogError("❌ Một trong các Sprite (fire, earth) chưa được gán trong Inspector!", this);

        // Khởi tạo thanh máu
        totalHealthBar.fillAmount = playerHealth.CurrentHealth / playerHealth.MaxHealth;

        totalEnemyHealthBar.fillAmount = bossHealth.GetCurrentHP() / bossHealth.MaxHealth;
        Debug.Log($"🛠️ Boss HP: {bossHealth.GetCurrentHP()} / {bossHealth.MaxHealth}");


        // Khởi tạo nguyên tố
        elementSprites = new Sprite[] { fireSprite, earthSprite };
        element.sprite = elementSprites[currentElementIndex];
    }

    void Update()   
    {
        // Cập nhật thanh máu Player
        currentHealthBar.fillAmount = playerHealth.CurrentHealth / playerHealth.MaxHealth;

        // Cập nhật thanh máu Boss
        currentEnemyHealthBar.fillAmount = bossHealth.GetCurrentHP() / bossHealth.MaxHealth;
        Debug.Log($"🛠️ Boss HP: {bossHealth.GetCurrentHP()} / {bossHealth.MaxHealth}");

        if (Input.GetKeyDown(KeyCode.O))
        {
            ChangeElementSprite();
        }
    }

    private void ChangeElementSprite()
    {
        currentElementIndex = (currentElementIndex + 1) % elementSprites.Length;
        element.sprite = elementSprites[currentElementIndex];

        Debug.Log("Nguyên tố hiện tại: " + GetElementName());
    }

    private string GetElementName()
    {
        return currentElementIndex == 0 ? "Lửa" : "Đất";
    }
}
