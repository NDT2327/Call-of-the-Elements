using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth =100f;
    public float CurrentHealth { get; private set; }
    public float MaxHealth => startingHealth;

    private Animator anim;
    private bool dead;

    private void Awake()
    {
        CurrentHealth = startingHealth;
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(float _damage)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - _damage, 0, startingHealth);

        if (CurrentHealth > 0)
        {
            //player hurt
            anim.SetTrigger("Hurt");
        }
        else if (!dead)
        {
            anim.SetTrigger("Death");
            dead = true;

            // Instead of disabling Player, send an event
            Debug.Log("☠ Player has died.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(1);
        }
    }

    public void Heal(float amount)
    {
        if (!dead)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
            Debug.Log("❤️ Hồi " + amount + " máu. Máu hiện tại: " + CurrentHealth);
        }
    }

}
