using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class EnemySpawnerNewWithoutChasing : MonoBehaviour
{
    [Header("--- SPAWNER ID ---")]
    public int spawnerID;

    [Header("--- ENEMY REFERENCES ---")] [SerializeField]
    GameObject enemyPrefab;

    [Header("--- SPAWN POINTS ---")]
    [SerializeField] Transform spawnPointsParent;
    [SerializeField] List<Transform> spawnPoints = new List<Transform>();

    [Header("--- PLAYER ENEMY COUNT ---")] [SerializeField]
    int currentEnemyCount; // 当前房间敌人计数 X
    [SerializeField] int maxEnemyCount = 10; // 当前房间敌人计数上限
    [SerializeField] int maxInitialEnemyCount;
    [SerializeField] float enemyCountRecoveryRate = 5f; // X每z秒+1的恢复速率

    [Header("--- REFERENCES ---")] [SerializeField]
    BoxCollider triggerZone; // Trigger Zone 的 Box Collider
    ObjectsPoolingDefault EnemiesPool;

    private bool isSpawning = false;
    private Coroutine CO_RecoverEnemyCount;

    IntegrityManager IntegrityInstance;
    CoreLoopManager CLInstance;

    void PopulateSpawnPoints()
    {
        foreach(Transform child in gameObject.transform)
        {
            if (child.name.Equals("SpawnPoints"))
            {
                spawnPointsParent = child;
            }
        }

        if (spawnPointsParent != null)
        {
            foreach (Transform child in spawnPointsParent)
            {
                spawnPoints.Add(child);
                child.gameObject.SetActive(false);
            }
        }
    }
    private void Start()
    {
        IntegrityInstance = IntegrityManager.instance;
        CLInstance = CoreLoopManager.Instance;

        EnemiesPool = GameObject.Find("EnemiesPool")?.GetComponent<ObjectsPoolingDefault>();
        
        PopulateSpawnPoints();
        
        triggerZone = GetComponent<BoxCollider>();
        if (triggerZone == null)
            Debug.LogError("No Trigger Zone!");

        // Init 初始化
        maxEnemyCount = spawnPoints.Count();
        maxInitialEnemyCount = maxEnemyCount;
        currentEnemyCount = EnemyCountProgressionByGameTime();
        triggerZone.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CLInstance.RundownSuccessful) //If rundown is successfully completed, do not spawn more enemies
            return;

        if (other.CompareTag("Player"))
        {
            currentEnemyCount = EnemyCountProgressionByGameTime();
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
    int EnemyCountProgressionByGameTime()
    {
        int NullBufferCount = Mathf.CeilToInt(IntegrityInstance.TimerInitial - 10f);
        int FirstProgressCount = Mathf.CeilToInt(IntegrityInstance.TimerInitial * (2f / 3f));
        int SecondProgressCount = Mathf.CeilToInt(IntegrityInstance.TimerInitial * 0.5f);
        if (IntegrityInstance.TimerMax > NullBufferCount)
        {
            return 0;
        }
        else if (IntegrityInstance.TimerMax <= NullBufferCount && IntegrityInstance.TimerMax > FirstProgressCount)
        {
            return Mathf.FloorToInt(maxInitialEnemyCount * (1f / 3f));
        }
        else if (IntegrityInstance.TimerMax <= FirstProgressCount && IntegrityInstance.TimerMax > SecondProgressCount)
        {
            return Mathf.FloorToInt(maxInitialEnemyCount * (1f / 2f));
        }
        else if (IntegrityInstance.TimerMax <= SecondProgressCount && IntegrityInstance.TimerMax > 0)
        {
            return Mathf.FloorToInt(maxInitialEnemyCount);
        }
        else
        {
            return maxInitialEnemyCount;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (CLInstance.RundownSuccessful) //If rundown is successfully completed, do not spawn more enemies
            return;

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

            maxEnemyCount = EnemyCountProgressionByGameTime();

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
            //GameObject enemyObj = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            if (EnemiesPool == null)
                return;

            GameObject spawnedEnemy = EnemiesPool.GetPooledObject(spawnPoint.position, spawnPoint.rotation);
            spawnedEnemy.GetComponent<EnemyBehavior>().patrolPoints 
                = new List<Transform>(spawnPoints);
            spawnedEnemy.GetComponent<EnemyBehavior>().enemySpawnerTracker 
                = gameObject.GetComponent<EnemySpawnerNewWithoutChasing>();
        }
    }

    private IEnumerator RecoverEnemyCount()
    {
        if (currentEnemyCount <= 1)
        {
            currentEnemyCount = maxEnemyCount;
            yield return null;
        }

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
            int randomIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
            selectedPoints.Add(spawnPoints[randomIndex]);
        }

        return selectedPoints;
    }

    private List<GameObject> GetEnemiesInTriggerZone()
    {
        EnemyBehavior[] remainingEnemies = FindObjectsOfType<EnemyBehavior>();
        var remainEnemiesHolder = remainingEnemies
            .Where(enemy => enemy != null &&
                            enemy.enemySpawnerTracker != null &&
                            enemy.enemySpawnerTracker == this.gameObject.GetComponent<EnemySpawnerNewWithoutChasing>())
            .ToList();
        //Collider[] colliders = Physics.OverlapBox(triggerZone.center, triggerZone.size / 2, triggerZone.transform.rotation);
        List<GameObject> enemies = new List<GameObject>();

        foreach (var child in remainEnemiesHolder)
        {
            enemies.Add(child.gameObject);
        }

        return enemies;
    }

    private int CountEnemiesInTriggerZone()
    {
        EnemyBehavior[] remainingEnemies = FindObjectsOfType<EnemyBehavior>(); 
        var remainEnemiesHolder = remainingEnemies
            .Where(enemy => enemy != null &&
                            enemy.enemySpawnerTracker != null &&
                            enemy.enemySpawnerTracker == this.gameObject.GetComponent<EnemySpawnerNewWithoutChasing>()
                            && enemy.enemyStateControl != EnemyBehavior.enemyStates.ChaseState
                            && enemy.enemyStateControl != EnemyBehavior.enemyStates.AttackState)
            .ToList();

        //Collider[] colliders = Physics.OverlapBox(triggerZone.center, triggerZone.size / 2, triggerZone.transform.rotation);
        int enemyCount = remainEnemiesHolder.Count;

/*        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                enemyCount++;
            }
        }*/
        
        return enemyCount;
    }
    
    private void DestroyEnemies(List<GameObject> enemies)
    {
        foreach (GameObject enemy in enemies)
        {
            if (EnemiesPool == null)
                return;

            enemy.GetComponent<EnemyBehavior>().enemySpawnerTracker = null;
            EnemiesPool.ReturnPooledObject(enemy);
        }
    }
}

