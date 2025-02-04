using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField] GameObject enemyPrefab;
    [SerializeField] int spawnNumber;
    [SerializeField] float spawnDelay;
    [SerializeField] int MaxEntityNumber;

    [Header("--- REFERENCES ---")]
    [SerializeField] EnemySpawnerTrigger triggerZone;

    bool isSpawning = false;

    [SerializeField] Transform Entities;

    Coroutine CO_SpawnEnemies;

    private void Start()
    {
        Entities = GameObject.Find("--- Entities ---").transform;
        if (Entities == null)
            Debug.LogError("No Entities holder found");

        triggerZone = GetComponentInChildren<EnemySpawnerTrigger>();
        if (triggerZone == null)
            return;
    }
    private void Update()
    {
        if (CountGameObjectsWithName("Enemy") >= MaxEntityNumber)
        {
            StopCoroutine(CO_SpawnEnemies);
            isSpawning = false;
        }
        else
        {
            if (!isSpawning && triggerZone.PlayerInZone)
            {
                if (CO_SpawnEnemies != null)
                    StopCoroutine(CO_SpawnEnemies);
                CO_SpawnEnemies = StartCoroutine(SpawnEnemies(spawnDelay));
            }
        }
    }
    int CountGameObjectsWithName(string name)
    {
        GameObject[] objects = FindObjectsOfType<GameObject>();
        int count = 0;

        foreach (GameObject obj in objects)
        {
            if (obj.activeInHierarchy && obj.name.Contains(name))
            {
                count++;
            }
        }

        return count;
    }

    IEnumerator SpawnEnemies(float duration)
    {
        isSpawning = true;

        while (true)
        {
            for (int i = 0; i < spawnNumber; i++)
            {
                GameObject enemyObj = Instantiate(enemyPrefab, transform.position, transform.rotation);
                enemyObj.transform.SetParent(Entities);
            }
            yield return new WaitForSeconds(duration);
        }
    }
}
