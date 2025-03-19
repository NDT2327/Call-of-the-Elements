using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currentHealthBar;
    [SerializeField] private Image element;

    [SerializeField] private Sprite windSprite;
    [SerializeField] private Sprite fireSprite;
    [SerializeField] private Sprite earthSprite;
    [SerializeField] private Sprite waterSprite;

    private int currentElementIndex = 1; // Mặc định là Lửa (Fire)
    private Sprite[] elementSprites;

    void Start()
    {
        if (playerHealth == null)
            Debug.LogError("❌ playerHealth chưa được gán trong Inspector!", this);

        if (totalHealthBar == null)
            Debug.LogError("❌ totalHealthBar chưa được gán trong Inspector!", this);

        if (currentHealthBar == null)
            Debug.LogError("❌ currentHealthBar chưa được gán trong Inspector!", this);

        if (element == null)
            Debug.LogError("❌ element chưa được gán trong Inspector!", this);

        if (windSprite == null || fireSprite == null || earthSprite == null || waterSprite == null)
            Debug.LogError("❌ Một trong các Sprite (wind, fire, earth, water) chưa được gán trong Inspector!", this);

        totalHealthBar.fillAmount = playerHealth.CurrentHealth / 100;
        elementSprites = new Sprite[] { windSprite, fireSprite, earthSprite, waterSprite };
        element.sprite = elementSprites[currentElementIndex]; // Set mặc định là Lửa
    }

    void Update()
    {
        currentHealthBar.fillAmount = playerHealth.CurrentHealth / 100;

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
        switch (currentElementIndex)
        {
            case 0: return "Gió";
            case 1: return "Lửa";
            case 2: return "Đất";
            case 3: return "Nước";
            default: return "Không xác định";
        }
    }
}
