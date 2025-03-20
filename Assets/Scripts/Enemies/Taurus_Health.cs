using UnityEngine;

public class Taurus_Health : MonoBehaviour
{
    //Health setting
    public int maxHealth = 500;
    public int currentHealth;
    public float enrageThreshold = 0.3f;


    //enrage modifiers
    public float speedMultiplier = 1.5f;
    public float attackCooldownReduction = 0.5f;

    //effects
    public ParticleSystem enrageEyeEffect;

    private bool isEnraged = false;
    private Taurus_Walk walkState;
    private Animator animator;
    private bool isEnragedEffectActive = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        walkState = animator.GetBehaviour<Taurus_Walk>();
    }

    private void Update()
    {
        if (!isEnragedEffectActive && isEnraged) {
            ActivateEnrageEffect();
        }else if(isEnragedEffectActive && !isEnraged)
        {
            DeactivateEnrageEffect();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
            Die();
        }
        else if (currentHealth <= maxHealth * enrageThreshold && !isEnraged)
        {
            Enrage();
        }
    }

    private void Enrage()
    {
        isEnraged = true;
        Debug.Log("Taurus is now enraged!");

        //speed
        if (walkState != null)
        {
            walkState.speed *= speedMultiplier;
        }

        animator.SetBool("IsEnraged", true);
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        // Xử lý chết (ví dụ: phá hủy Taurus)
        Destroy(gameObject, 1f); // Hủy sau 1 giây
    }

    // Getter để các script khác truy cập trạng thái
    public bool IsEnraged() => isEnraged;
    public float GetCooldownReduction() => attackCooldownReduction;

    private void ActivateEnrageEffect()
    {
        if (enrageEyeEffect != null)
        {
            enrageEyeEffect.Play();
            isEnragedEffectActive = true;
            Debug.Log("Enrage effect activated: Eyes glowing red!");
        }
    }

    private void DeactivateEnrageEffect()
    {
        if (enrageEyeEffect != null)
        {
            enrageEyeEffect.Stop();
            isEnragedEffectActive = false;
            Debug.Log("Enrage effect deactivated.");
        }
    }
}
