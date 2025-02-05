using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerCorePooling : MonoBehaviour
{
  
    public GameObject powerCorePrefab;
    public int poolSize;

    private List<GameObject> pCorePool;

    public static PowerCorePooling instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        poolSize = CountGameObjectsWithNameContaining("PowerCore");

        pCorePool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject powerCoreSpawned = Instantiate(powerCorePrefab, transform.position, Quaternion.identity);
            powerCoreSpawned.transform.SetParent(this.transform);
            powerCoreSpawned.SetActive(false);
            pCorePool.Add(powerCoreSpawned);
        }
    }
    int CountGameObjectsWithNameContaining(string subName)
    {
        int count = 0;

        foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (obj.name.Contains(subName))
            {
                count++;
            }
        }

        return count;
    }
    public GameObject GetPowerCore()
    {
        foreach (GameObject bullet in pCorePool)
        {
            if (!bullet.activeInHierarchy)
            {
                bullet.SetActive(true);
                return bullet;
            }
        }

        GameObject newPowerCore = Instantiate(powerCorePrefab, transform.position, Quaternion.identity);
        pCorePool.Add(newPowerCore);
        return newPowerCore;
    }
    public void ReturnPowerCore(GameObject powerCore)
    {
        powerCore.SetActive(false);
    }

}
