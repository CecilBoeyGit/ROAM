using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySwarmSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    [SerializeField] float numberToSpawn = 10;
    [SerializeField] float radius = 5;
    [SerializeField] float navMeshCheckDistance = 1;
    [SerializeField] float delayTime = 3;
    float mainTimer = 0;

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
        mainTimer = 0;

        PlayerInstance = PlayerController.instance;

        EnemiesPool = GameObject.Find("EnemiesPool")?.GetComponent<ObjectsPoolingDefault>();
    }

    public void SpawnSwarm()
    {

        if(mainTimer < delayTime)
        {
            mainTimer += Time.deltaTime;
            return;
        }

        Vector3 playerCurrentPos = PlayerInstance.transform.position;

        if (prefabToSpawn == null || playerCurrentPos == null)
        {
            Debug.LogWarning("Prefab or CenterPoint not assigned.");
            return;
        }

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
                Debug.Log($"Skipped spawn at {spawnPos} — not on NavMesh.");
            }
        }
    }
}
