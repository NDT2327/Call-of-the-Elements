using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth;
    public float CurrentHealth { get; private set; }
    public float MaxHealth { get; private set; }
    private Animator anim;
    private bool dead;

    private void Awake()
    {
        CurrentHealth = startingHealth;
        MaxHealth = startingHealth;
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
        else
        {
            //player dead
            if (!dead)
            {
                anim.SetTrigger("Death");
                GetComponent<Player>().enabled = false;
                dead = true;
            }
            
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
