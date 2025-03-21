using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private List<EnemyHP> bossHealthList = new List<EnemyHP>(); // Danh sách Boss
    [SerializeField] private Image totalEnemyHealthBar;
    [SerializeField] private Image currentEnemyHealthBar;
    [SerializeField] public GameObject enemyHealthContainer;

    private EnemyHP currentBoss; // Boss đang được theo dõi

    void Start()
    {
        enemyHealthContainer.SetActive(false); // Ẩn ban đầu

        if (bossHealthList.Count > 0)
        {
            // Chỉ lấy boss đầu tiên trong danh sách để hiển thị ban đầu
            EnemyHP firstBoss = bossHealthList[0];
            totalEnemyHealthBar.fillAmount = firstBoss.GetCurrentHP() / firstBoss.MaxHealth;
        }
    }


    void Update()
    {
        UpdateBossHealthBar();
    }

    private void UpdateBossHealthBar()
    {
        // Kiểm tra xem có boss nào xuất hiện không
        foreach (EnemyHP boss in bossHealthList)
        {
            if (boss != null && boss.HasAppeared())
            {
                if (currentBoss != boss)
                {
                    currentBoss = boss;
                    totalEnemyHealthBar.fillAmount = currentBoss.GetCurrentHP() / currentBoss.MaxHealth;
                    enemyHealthContainer.SetActive(true);
                }

                // Cập nhật máu của Boss hiện tại
                currentEnemyHealthBar.fillAmount = currentBoss.GetCurrentHP() / currentBoss.MaxHealth;
                return;
            }
        }

        // Nếu không có boss nào xuất hiện, ẩn thanh máu
        enemyHealthContainer.SetActive(false);
        currentBoss = null;
    }
}
