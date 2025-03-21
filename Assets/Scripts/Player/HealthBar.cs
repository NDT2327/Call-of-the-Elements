using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] public Health playerHealth;
    [SerializeField] public Stamina playerStamina;

    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currentHealthBar;
    [SerializeField] private Image totalStamina;
    [SerializeField] private Image currentStamina;

    [SerializeField] private Image element;
    [SerializeField] private Sprite fireSprite;
    [SerializeField] private Sprite earthSprite;
    [SerializeField] private Sprite normalSprite;
    private int currentElementIndex = 0;
    private Sprite[] elementSprites;

    private float cooldownTime = 3f; 
    private float cooldownTimer = 0f;
    private bool isCooldownActive = false;
    public bool hasSpAttack = false;

    void Start()
    {
        Debug.Log($"📊 [DEBUG] playerHealth: {playerHealth}, MaxHealth: {playerHealth.MaxHealth}, CurrentHealth: {playerHealth.CurrentHealth}");
        Debug.Log($"⚡ [DEBUG] playerStamina: {playerStamina}, MaxStamina: {playerStamina.MaxStamina}, CurrentStamina: {playerStamina.CurrentStamina}");
       
        // Khởi tạo thanh máu Player
        totalHealthBar.fillAmount = playerHealth.CurrentHealth / playerHealth.MaxHealth;
        totalStamina.fillAmount = playerStamina.CurrentStamina / playerStamina.MaxStamina;

        // Khởi tạo nguyên tố
        elementSprites = new Sprite[] { earthSprite, fireSprite  };
        element.sprite = normalSprite;
    }

    void Update()
    {
        currentHealthBar.fillAmount = playerHealth.CurrentHealth / playerHealth.MaxHealth;
        currentStamina.fillAmount = playerStamina.CurrentStamina / playerStamina.MaxStamina;

        if (isCooldownActive)
        {
            cooldownTimer -= Time.deltaTime;
            element.fillAmount = cooldownTimer / cooldownTime; 

            if (cooldownTimer <= 0)
            {
                isCooldownActive = false;
                element.fillAmount = 1f; 
            }
        }
    }


    public void UnlockSpAttack()
    {
        hasSpAttack = true;
        SetElementSprite(currentElementIndex);
    }

    public void SetElementSprite(int elementIndex)
    {
        if (!hasSpAttack)
        {
            element.sprite = normalSprite;
            return;
        }
        if (elementIndex < 0 || elementIndex >= elementSprites.Length)
        {
            Debug.LogError("⚠ Element index không hợp lệ!");
            return;
        }
        element.sprite = elementSprites[elementIndex];
    }

    public void StartElementCooldown()
    {
        isCooldownActive = true;
        cooldownTimer = cooldownTime;
        element.fillAmount = 1f; 
    }

}
