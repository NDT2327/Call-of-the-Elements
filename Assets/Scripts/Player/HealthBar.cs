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
    [SerializeField] private Sprite ultimateSprite;
    private int currentElementIndex = 0;
    private Sprite[] elementSprites;

    private float cooldownTime = 3f; 
    private float cooldownTimer = 0f;
    private bool isCooldownActive = false;
    public bool hasSpAttack = false;

    [SerializeField] private Image ultimateCooldownFill;
    private float ultimateCooldown = 15f; 
    private float ultimateTimer = 0f;
    private bool isUltimateCooldown = false;

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
        ultimateCooldownFill.sprite = ultimateSprite;
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

        if (isUltimateCooldown)
        {
            ultimateTimer -= Time.deltaTime;
            float fillValue = Mathf.Clamp01(ultimateTimer / ultimateCooldown);
            ultimateCooldownFill.fillAmount = fillValue;

            Debug.Log($"⏳ Ultimate Cooldown: {ultimateTimer:F2}s - Fill: {fillValue}");

            if (ultimateTimer <= 0)
            {
                isUltimateCooldown = false;
                ultimateCooldownFill.fillAmount = 1f;
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
    public void StartUltimateCooldown()
    {
        isUltimateCooldown = true;
        ultimateTimer = ultimateCooldown;
        ultimateCooldownFill.fillAmount = 1f;
    }

}
