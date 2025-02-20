using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerNewWithoutChasing : MonoBehaviour
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
            List<GameObject> enemiesInZone = GetEnemiesInTriggerZone();

            // 过滤掉处于 ChaseState 和 AttackState 的敌人
            List<GameObject> enemiesToDestroy = new List<GameObject>();
            int validEnemyCount = 0;

            foreach (GameObject enemy in enemiesInZone)
            {
                EnemyBehavior enemyBehavior = enemy.GetComponent<EnemyBehavior>();
                if (enemyBehavior != null)
                {
                    // 若敌人处于 ChaseState 或 AttackState，跳过
                    if (enemyBehavior.enemyStateControl == EnemyBehavior.enemyStates.ChaseState ||
                        enemyBehavior.enemyStateControl == EnemyBehavior.enemyStates.AttackState)
                    {
                        continue;
                    }

                    // 否则，计入 validEnemyCount 并加入销毁列表
                    validEnemyCount++;
                    enemiesToDestroy.Add(enemy);
                }
            }

            // 更新 currentEnemyCount
            currentEnemyCount = validEnemyCount;
            Debug.Log("Enemies in zone before destruction: " + currentEnemyCount);

            // 销毁不处于 ChaseState 和 AttackState 的敌人
            DestroyEnemies(enemiesToDestroy);
            
            if (CO_RecoverEnemyCount != null)
                StopCoroutine(CO_RecoverEnemyCount);
            CO_RecoverEnemyCount = StartCoroutine(RecoverEnemyCount());
        }
    }

    private void SpawnEnemies(int count)
    {
        List<Transform> selectedSpawnPoints = GetRandomSpawnPoints(currentEnemyCount);
        
        foreach (Transform spawnPoint in selectedSpawnPoints)
        {
            GameObject enemyObj = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    private IEnumerator RecoverEnemyCount()
    {
        while (currentEnemyCount < maxEnemyCount)
        {
            yield return new WaitForSeconds(enemyCountRecoveryRate);
            currentEnemyCount++;
            Debug.Log("Recovered enemy count: " + currentEnemyCount);
        }
    }

    private List<Transform> GetRandomSpawnPoints(int count)
    {
        List<Transform> selectedPoints = new List<Transform>();
        
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, spawnPoints.Count);
            selectedPoints.Add(spawnPoints[randomIndex]);
        }

        return selectedPoints;
    }

    private List<GameObject> GetEnemiesInTriggerZone()
    {
        Collider[] colliders = Physics.OverlapBox(triggerZone.center, triggerZone.size / 2, triggerZone.transform.rotation);
        List<GameObject> enemies = new List<GameObject>();

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                enemies.Add(collider.gameObject);
            }
        }

        return enemies;
    }

    private int CountEnemiesInTriggerZone()
    {
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
    
    private void DestroyEnemies(List<GameObject> enemies)
    {
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }
}

