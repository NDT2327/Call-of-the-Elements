using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private EnemyHP bossHealth; // Cho phép null
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currentHealthBar;
    [SerializeField] private Image totalStamina;
    [SerializeField] private Image currentStamina;
    [SerializeField] private Image totalEnemyHealthBar;
    [SerializeField] private Image currentEnemyHealthBar;
    [SerializeField] private GameObject enemyHealthContainer; // Chứa thanh máu của boss
    [SerializeField] private Image element;
    [SerializeField] private GameObject frame;
    [SerializeField] private Sprite fireSprite;
    [SerializeField] private Sprite earthSprite;

    private int currentElementIndex = 0;
    private Sprite[] elementSprites;

    private float cooldownTime = 3f; // Thời gian hồi chiêu
    private float cooldownTimer = 0f;
    private bool isCooldownActive = false;

    void Start()
    {
        Debug.Log("🏁 Health Bar Script Started!");

        if (playerHealth == null)
            Debug.LogError("❌ playerHealth chưa được gán trong Inspector!", this);

        if (totalHealthBar == null)
            Debug.LogError("❌ totalHealthBar chưa được gán trong Inspector!", this);

        if (currentHealthBar == null)
            Debug.LogError("❌ currentHealthBar chưa được gán trong Inspector!", this);

        if (enemyHealthContainer == null)
            Debug.LogError("❌ enemyHealthContainer chưa được gán trong Inspector!", this);

        if (element == null)
            Debug.LogError("❌ element chưa được gán trong Inspector!", this);

        if (fireSprite == null || earthSprite == null)
            Debug.LogError("❌ Một trong các Sprite (fire, earth) chưa được gán trong Inspector!", this);

        // Khởi tạo thanh máu Player
        totalHealthBar.fillAmount = playerHealth.CurrentHealth / playerHealth.MaxHealth;

        // Nếu bossHealth không null, khởi tạo máu của boss và ẩn đi nếu boss chưa xuất hiện
        if (bossHealth != null)
        {
            totalEnemyHealthBar.fillAmount = bossHealth.GetCurrentHP() / bossHealth.MaxHealth;
            enemyHealthContainer.SetActive(false); // Ẩn ban đầu
        }

        // Khởi tạo nguyên tố
        elementSprites = new Sprite[] { fireSprite, earthSprite };
        element.sprite = elementSprites[currentElementIndex];
    }

    void Update()
    {
        // Cập nhật thanh máu Player
        currentHealthBar.fillAmount = playerHealth.CurrentHealth / playerHealth.MaxHealth;

        // Nếu bossHealth không null, cập nhật thanh máu boss
        if (bossHealth != null)
        {
            // Kiểm tra boss có xuất hiện hay không
            if (bossHealth.HasAppeared()) // Giả sử có phương thức kiểm tra boss đã xuất hiện
            {
                enemyHealthContainer.SetActive(true); // Hiện thanh máu Boss
                currentEnemyHealthBar.fillAmount = bossHealth.GetCurrentHP() / bossHealth.MaxHealth;
            }
            else
            {
                enemyHealthContainer.SetActive(false); // Ẩn nếu boss chưa xuất hiện
            }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ChangeElementSprite();
        }
        if (isCooldownActive)
        {
            cooldownTimer -= Time.deltaTime;
            element.fillAmount = cooldownTimer / cooldownTime; // Giảm dần

            if (cooldownTimer <= 0)
            {
                isCooldownActive = false;
                element.fillAmount = 1f; // Reset lại trạng thái đầy sau cooldown
            }
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

    public void StartElementCooldown()
    {
        isCooldownActive = true;
        cooldownTimer = cooldownTime;
        element.fillAmount = 1f; // Bắt đầu đầy
    }

}
