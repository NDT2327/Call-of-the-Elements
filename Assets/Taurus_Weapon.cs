using UnityEngine;

public class Taurus_Weapon : MonoBehaviour
{
    [Header("Attack Settings")]
    public int attack1Dmg = 20;
    public int attack2Dmg = 25;

    public Vector3 attackOffset;
    public float attackRange = 6f;
    public LayerMask playerLayer;

    [Header("Enrage settings")]
    public float baseCooldown = 2f;
    public float enrageCooldownReduction = 0.5f;
    

    private TaurusAI taurus;
    private float lastAttackTime;

    void Start()
    {
        taurus = GetComponent<TaurusAI>();
        lastAttackTime = -baseCooldown;
    }

    //perform attack1 
    public void Attack1()
    {
        if (Time.time >= lastAttackTime + GetCurrentCooldown())
        {
            PerformAttack(attack1Dmg, "Attack1");
            AudioManager.instance.PlayEnemyAttackSound1();
        }
    }

    //perform attack 2
    public void Attack2()
    {
        if (Time.time >= lastAttackTime + GetCurrentCooldown())
        {
            PerformAttack(attack2Dmg, "Attack2");
            AudioManager.instance.PlayEnemyAttackSound2();
        }
    }

    private void PerformAttack(int baseDamge, string attackName)
    {
        AudioManager.instance.PlayEnemyAttackSound1();
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;

        Collider2D coliInfo = Physics2D.OverlapCircle(pos, attackRange, playerLayer);
        if (coliInfo != null)
        {
            // Giả sử PlayerHealth là script quản lý máu của người chơi
            int finalDmg = Mathf.RoundToInt(baseDamge * taurus.GetDamageMultiplier());
            coliInfo.GetComponent<Health>().TakeDamage(finalDmg);
        }
        lastAttackTime = Time.time; // Cập nhật thời gian tấn công cuối
    }

    private float GetCurrentCooldown()
    {
        return (taurus != null && taurus.IsEnraged() ? baseCooldown - enrageCooldownReduction : baseCooldown);
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
