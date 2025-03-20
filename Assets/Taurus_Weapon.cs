using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Taurus_Weapon : MonoBehaviour
{
    [Header("Attack Settings")]
    public int attack1Dmg = 20;
    public int attack2Dmg = 25;

    public Vector3 attackOffset;
    public float attackRange = 6f;
    public LayerMask playerLayer;

    [Header("Cooldown settings")]
    public float baseCooldown = 2f;
    public float enrageCooldownReduction = 0.5f;

    [Header("Effects")]
    public ParticleSystem enrageEffect;

    private Taurus_Health health;
    private float lastAttackTime;
    private bool isEnragedEffectActive = false;

    void Start()
    {
        health = GetComponent<Taurus_Health>();
        lastAttackTime = -baseCooldown;
    }

    //perform attack1 
    public void Attack1()
    {
        if (Time.time >= lastAttackTime + GetCurrentCooldown())
        {
            Vector3 pos = transform.position;
            pos += transform.right * attackOffset.x;
            pos += transform.up * attackOffset.y;

            Collider2D coliInfo = Physics2D.OverlapCircle(pos, attackRange, playerLayer);
            if (coliInfo != null)
            {
                // Giả sử PlayerHealth là script quản lý máu của người chơi
                // coliInfo.GetComponent<PlayerHealth>().TakeDamage(attack1Dmg);
                Debug.Log("Player hit by Attack1 for " + attack1Dmg + " damage");
            }
            lastAttackTime = Time.time; // Cập nhật thời gian tấn công cuối
        }
    }

    //perform attack 2
    public void Attack2()
    {
        if (Time.time >= lastAttackTime + GetCurrentCooldown())
        {
            Vector3 pos = transform.position;
            pos += transform.right * attackOffset.x;
            pos += transform.up * attackOffset.y;

            Collider2D coliInfo = Physics2D.OverlapCircle(pos, attackRange, playerLayer);
            if (coliInfo != null)
            {
                // Giả sử PlayerHealth là script quản lý máu của người chơi
                // coliInfo.GetComponent<PlayerHealth>().TakeDamage(attack2Dmg);
                Debug.Log("Player hit by Attack2 for " + attack2Dmg + " damage");
            }
            lastAttackTime = Time.time; // Cập nhật thời gian tấn công cuối
        }
    }

    private float GetCurrentCooldown()
    {
        return (health != null && health.IsEnraged()) ? baseCooldown * health.GetCooldownReduction() : baseCooldown;
    }

    //gizmo to modify attack range
    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, attackRange);
    }
}
