using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Cinemachine;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject[] earthEnemies;
    public GameObject golem;
    public GameObject[] fireEnemies;
    public GameObject fireDemon;
    public GameObject taurus;
    public GameObject conqueror;

    [Header("Camera Settings")]
    public CinemachineVirtualCamera virtualCamera;
    public PolygonCollider2D mapBoudary;
    public float bossAreaWidth = 10f;
    public float bossAreaHeight = 8f;

    private Transform player;
    private PolygonCollider2D bossAreaConfiner;
    private bool isActive = false;

    public void Initialize(GameManager.Map currentMap)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        bossAreaConfiner = gameObject.AddComponent<PolygonCollider2D>();
        bossAreaConfiner.isTrigger = true;
        bossAreaConfiner.enabled = false;
        virtualCamera.GetComponent<CinemachineConfiner>().m_BoundingShape2D = bossAreaConfiner;

        //all enenmies
        SetEnemiesActive(earthEnemies, false);
        golem.gameObject.SetActive(false);
        SetEnemiesActive(fireEnemies, false);
        fireDemon.gameObject.SetActive(false);
        taurus.gameObject.SetActive(false);
        conqueror.gameObject.SetActive(false);

        //active base on map
        switch (currentMap)
        {
            case GameManager.Map.Earth:
                SetEnemiesActive(earthEnemies, true);
                golem.gameObject.SetActive(true);
                break;
            case GameManager.Map.Lava:
                SetEnemiesActive(fireEnemies, true);
                fireDemon.gameObject.SetActive(true);
                break;
            case GameManager.Map.Castle:
                taurus.SetActive(true);
                conqueror.SetActive(true);
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (player == null || isActive) return;

        CheckBossActivation(golem);
        CheckBossActivation(fireDemon);
        CheckBossActivation(taurus);
        CheckBossActivation(conqueror);
    }

    private void CheckBossActivation(GameObject boss)
    {
        if (boss != null && boss.gameObject.activeSelf)
        {
            float distance = Vector2.Distance(player.position, boss.transform.position);
            if (distance <= 10f)
            {
                ActivateBoss(boss);
            }
        }

    }
    private void ActivateBoss(GameObject boss)
    {
        isActive = true;
        //game manager
        boss.SetActive(true);
        
        LockCameraToBossArea(boss.transform.position);
        Debug.Log($"Boss {boss.name} activated -Camera locked!");
    }

    public void DeactiveBoss(GameObject boss)
    {
        isActive = false;
        UnlockCamera();
        //game manager

    }

    private void LockCameraToBossArea(Vector3 bossPos)
    {
        Vector2[] points = new Vector2[] {
            new Vector2(bossPos.x - bossAreaWidth/2, bossPos.y - bossAreaHeight/2),
            new Vector2(bossPos.x + bossAreaWidth/2, bossPos.y - bossAreaHeight/2),
            new Vector2(bossPos.x + bossAreaWidth/2, bossPos.y + bossAreaHeight/ 2),
            new Vector2(bossPos.x - bossAreaWidth/2, bossPos.y + bossAreaHeight/2)
        };
        bossAreaConfiner.points = points;
        bossAreaConfiner.enabled = true;
        virtualCamera.GetComponent<CinemachineConfiner>().m_BoundingShape2D = bossAreaConfiner;

    }

    private void UnlockCamera()
    {
        virtualCamera.GetComponent<CinemachineConfiner>().m_BoundingShape2D = mapBoudary;
        bossAreaConfiner.enabled = false;
    }

    public bool IsBossActive(MonoBehaviour boss)
    {
        return isActive == true && boss.gameObject.activeSelf;
    }

    private void SetEnemiesActive(GameObject[] enemies, bool active)
    {
        foreach (var enemy in enemies) enemy.SetActive(active);
    }


}
