using UnityEngine;

public class Stamina : MonoBehaviour
{
    [SerializeField] private float maxStamina = 100f;
    public float CurrentStamina { get; private set; }

    public float MaxStamina => maxStamina;

    private void Awake()
    {
        CurrentStamina = maxStamina;
    }

    public void UseStamina(float amount)
    {
        CurrentStamina -= amount;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0, maxStamina);
    }

    public void RegainStamina(float amount)
    {
        CurrentStamina += amount;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0, maxStamina);
    }
}
