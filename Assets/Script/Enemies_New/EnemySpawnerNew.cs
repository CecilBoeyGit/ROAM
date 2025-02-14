using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerNew : MonoBehaviour
{
    [Header("--- ENEMY REFERENCES ---")] [SerializeField]
    GameObject enemyPrefab;

    [Header("--- SPAWN POINTS ---")] [SerializeField]
    List<Transform> spawnPoints = new List<Transform>(); // 生成点

    [Header("--- PLAYER ENEMY COUNT ---")] [SerializeField]
    int currentEnemyCount; // 当前房间敌人计数 X
    [SerializeField] int maxEnemyCount = 10; // 当前房间敌人计数上限
    [SerializeField] float enemyCountRecoveryRate = 5f; // X每z秒+1的恢复速率

    [Header("--- REFERENCES ---")] [SerializeField]
    BoxCollider triggerZone; // Trigger Zone 的 Box Collider

    private bool isSpawning = false;
    private Coroutine CO_RecoverEnemyCount;



    private void Start()
    {
        triggerZone = GetComponentInChildren<BoxCollider>();
        if (triggerZone == null)
            Debug.LogError("No Trigger Zone (Box Collider) found in children!");

        // Init 初始化
        currentEnemyCount = maxEnemyCount;
        triggerZone.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 检查当前 Trigger Zone 中的敌人数量
            int enemiesInZone = CountEnemiesInTriggerZone();

            // 计算需要生成的敌人数量
            int enemiesToSpawn = currentEnemyCount - enemiesInZone;

            // 如果需要生成的敌人数量大于 0，则生成敌人
            if (enemiesToSpawn > 0)
            {
                SpawnEnemies(enemiesToSpawn);
            }

            // 停止恢复敌人计数的协程
            if (CO_RecoverEnemyCount != null)
                StopCoroutine(CO_RecoverEnemyCount);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 玩家离开Trigger Zone时，销毁所有敌人
            DestroyAllEnemiesInTriggerZone();
            
            if (CO_RecoverEnemyCount != null)
                StopCoroutine(CO_RecoverEnemyCount);
            CO_RecoverEnemyCount = StartCoroutine(RecoverEnemyCount());
        }
    }

    private void SpawnEnemies(int count)
    {
        // 随机选择X个生成点
        List<Transform> selectedSpawnPoints = GetRandomSpawnPoints(currentEnemyCount);

        // 在每个选中的生成点生成敌人
        foreach (Transform spawnPoint in selectedSpawnPoints)
        {
            GameObject enemyObj = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    private IEnumerator RecoverEnemyCount()
    {
        while (true)
        {
            yield return new WaitForSeconds(enemyCountRecoveryRate);

            // 恢复敌人计数，直到达到上限
            if (currentEnemyCount < maxEnemyCount)
                currentEnemyCount++;
            else
                break;
        }
    }

    private List<Transform> GetRandomSpawnPoints(int count)
    {
        List<Transform> selectedPoints = new List<Transform>();

        // 随机选择生成点
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, spawnPoints.Count);
            selectedPoints.Add(spawnPoints[randomIndex]);
        }

        return selectedPoints;
    }

    private int CountEnemiesInTriggerZone()
    {
        // 获取 Trigger Zone 中的所有敌人
        Collider[] colliders = Physics.OverlapBox(triggerZone.center, triggerZone.size / 2, triggerZone.transform.rotation);
        int enemyCount = 0;

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                enemyCount++;
            }
        }

        return enemyCount;
    }
    
    private void DestroyAllEnemiesInTriggerZone()
    {
        Collider[] colliders =
            Physics.OverlapBox(triggerZone.center, triggerZone.size / 2, triggerZone.transform.rotation);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Destroy(collider.gameObject);
                currentEnemyCount--;
            }
        }
    }
}
