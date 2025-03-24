using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private List<EnemyHP> bossHealthList = new List<EnemyHP>(); 
    [SerializeField] private Image totalEnemyHealthBar;
    [SerializeField] private Image currentEnemyHealthBar;
    [SerializeField] public GameObject enemyHealthContainer;

    private EnemyHP currentBoss;
    private Camera mainCamera;

    void Start()
    {
        enemyHealthContainer.SetActive(false); // Ẩn ban đầu
        mainCamera = Camera.main;
        if (bossHealthList.Count > 0)
        {
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

        foreach (EnemyHP boss in bossHealthList)
        {
            if (boss != null && boss.HasAppeared() && IsEnemyVisible(boss.gameObject))
            {
                if (currentBoss != boss)
                {
                    currentBoss = boss;
                    totalEnemyHealthBar.fillAmount = currentBoss.GetCurrentHP() / currentBoss.MaxHealth;
                    enemyHealthContainer.SetActive(true);
                }


                currentEnemyHealthBar.fillAmount = currentBoss.GetCurrentHP() / currentBoss.MaxHealth;
                return;
            }
        }

        enemyHealthContainer.SetActive(false);
        currentBoss = null;
    }
    private bool IsEnemyVisible(GameObject enemy)
    {
        Renderer renderer = enemy.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.isVisible;
        }

        // Nếu không có Renderer, dùng phương pháp khác để kiểm tra
        Plane[] cameraPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        Collider enemyCollider = enemy.GetComponent<Collider>(); // Hoặc Collider2D nếu là game 2D
        if (enemyCollider != null)
        {
            return GeometryUtility.TestPlanesAABB(cameraPlanes, enemyCollider.bounds);
        }

        return false;
    }
}
