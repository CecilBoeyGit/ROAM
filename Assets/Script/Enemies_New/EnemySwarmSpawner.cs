using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySwarmSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    [SerializeField] GameObject ManualSpawnPointsParent;
    List<Transform> manualSpawnPoints = new List<Transform>();

    [SerializeField] float numberToSpawn = 10;
    [SerializeField] float radius = 5;
    [SerializeField] float navMeshCheckDistance = 1;
    [SerializeField] float delayTime = 3;

    ObjectsPoolingDefault EnemiesPool;

    PlayerController PlayerInstance;

    public static EnemySwarmSpawner instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void OnEnable()
    {
        IntegrityManager.MeterNull += SpawnSwarm;
    }

    private void OnDisable()
    {
        IntegrityManager.MeterNull -= SpawnSwarm;
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerInstance = PlayerController.instance;

        EnemiesPool = GameObject.Find("EnemiesPool")?.GetComponent<ObjectsPoolingDefault>();

        if (ManualSpawnPointsParent != null)
        {
            foreach (Transform child in ManualSpawnPointsParent.transform)
            {
                manualSpawnPoints.Add(child);
            }
        }
    }

    Coroutine CO_SpawnSwarm;

    public void SpawnSwarm()
    {
        Vector3 playerCurrentPos = PlayerInstance.transform.position;

        if (prefabToSpawn == null || playerCurrentPos == null)
        {
            Debug.LogWarning("Prefab or CenterPoint not assigned.");
            return;
        }

        if (CO_SpawnSwarm != null)
            StopCoroutine(CO_SpawnSwarm);
        else
            CO_SpawnSwarm = StartCoroutine(StartSpawningSwarm(delayTime, playerCurrentPos));    
    }

    IEnumerator StartSpawningSwarm(float delayTime, Vector3 playerCurrentPos)
    {

        yield return new WaitForSeconds(delayTime);

        float angleStep = 360f / numberToSpawn;

        for (int i = 0; i < numberToSpawn; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Vector3 spawnPos = playerCurrentPos + offset;

            // Check if the spawn position is on the NavMesh
            if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, navMeshCheckDistance, NavMesh.AllAreas))
            {
                GameObject enemyObj = EnemiesPool.GetPooledObject(hit.position, Quaternion.identity);
                EnemyBehavior enemyBhv = enemyObj.GetComponent<EnemyBehavior>();
                if (enemyBhv != null)
                {
                    enemyBhv.ForceChaseState();
                    enemyBhv.AttackSpoolTime = 0;
                    enemyBhv.attackColliderRadius = 10;
                    enemyBhv.nmAgent.speed = 15;
                    enemyBhv.nmAgent.acceleration = 12;
                }
            }
            else
            {
                int randomSpawnPos = Mathf.RoundToInt(Random.Range(0, manualSpawnPoints.Count));

                if(randomSpawnPos < manualSpawnPoints.Count)
                {
                    GameObject enemyObj = EnemiesPool.GetPooledObject(manualSpawnPoints[randomSpawnPos].position, Quaternion.identity);
                    EnemyBehavior enemyBhv = enemyObj.GetComponent<EnemyBehavior>();
                    if (enemyBhv != null)
                    {
                        enemyBhv.ForceChaseState();
                        enemyBhv.AttackSpoolTime = 0;
                        enemyBhv.attackColliderRadius = 10;
                        enemyBhv.nmAgent.speed = 15;
                        enemyBhv.nmAgent.acceleration = 12;
                    }
                }

                Debug.Log($"Skipped spawn at {spawnPos} — not on NavMesh.");
            }
        }
    }
}
