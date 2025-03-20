using UnityEngine;

public class TaurusAI : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 5f;

    [Header("Enrage Settings")]
    public ParticleSystem enrageEyeEffect;
    public float enrageThreshold = 0.5f;
    public float damageMuliplier = 2f;

    private EnemyHP enemyHP;
    private bool isEnraged = false;
    private bool isFliped = false;

    void Start()
    {
        enemyHP = GetComponent<EnemyHP>();    
    }

    private void Update()
    {
        if (enemyHP != null) { 
            float currentHP = enemyHP.GetCurrentHP();
            float maxHP = enemyHP.maxHP;

            //activae enrage
            if (currentHP <= maxHP * enrageThreshold && !isEnraged) {
                ActivateEnrage();
            
            }
        }
    }
    public bool IsPlayerInRange()
    {
        return Vector2.Distance(transform.position, player.position) < detectionRange;
    }

    public bool IsEnraged()
    {
        return isEnraged;
    }

    public float GetDamageMultiplier()
    {
        return isEnraged ? damageMuliplier : 1f;
    }

    private void ActivateEnrage()
    {
        isEnraged = true;
        if(enrageEyeEffect != null)
        {
            enrageEyeEffect.Play();
        }
        Debug.Log("Taurus is ENRAGED!");
    }

    public void LookAtPlayer()
    {
        Vector3 flippeed = transform.localScale;
        flippeed.z *= -1f;

        //
        if (transform.position.x > player.position.x && isFliped)
        {
            transform.localScale = flippeed;
            transform.Rotate(0f, 180f, 0f);
            isFliped = false;
        }
        else if (transform.position.x < player.position.x && !isFliped)
        {
            transform.localScale = flippeed;
            transform.Rotate(0f, 180f, 0f);
            isFliped = true;
        }
        //
    }

    // 🛠 Vẽ phạm vi phát hiện trong Scene View
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; // Màu vòng tròn
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
