using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsPoolingDefault : MonoBehaviour
{

    public GameObject ObjectsToSpawn;
    public int poolSize;

    private List<GameObject> PrefabPool;

    [SerializeField] bool KinematicObjects = false;

    void Start()
    {
        PrefabPool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject objectsSpawned = Instantiate(ObjectsToSpawn, transform.position, Quaternion.identity);
            objectsSpawned.transform.SetParent(this.transform);
            if(KinematicObjects)
            {
                Rigidbody Rbd = objectsSpawned.GetComponent<Rigidbody>();
                Rbd.isKinematic = true;
            }
            objectsSpawned.SetActive(false);
            PrefabPool.Add(objectsSpawned);
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
    public GameObject GetPooledObject(Vector3 SpawnedPosition, Quaternion SpawnedRotation)
    {
        foreach (GameObject child in PrefabPool)
        {
            if (!child.activeInHierarchy)
            {
                child.transform.position = SpawnedPosition;
                child.transform.rotation = SpawnedRotation;
                child.SetActive(true);
                if (KinematicObjects)
                {
                    Rigidbody Rbd = child.GetComponent<Rigidbody>();
                    Rbd.isKinematic = false;
                }
                return child;
            }
        }

        GameObject overDrawnObject = Instantiate(ObjectsToSpawn, SpawnedPosition, SpawnedRotation);
        PrefabPool.Add(overDrawnObject);
        return overDrawnObject;
    }
    public void ReturnPooledObject(GameObject pooledObject)
    {
        pooledObject.SetActive(false);
        if (KinematicObjects)
        {
            Rigidbody Rbd = pooledObject.GetComponent<Rigidbody>();
            Rbd.isKinematic = true;
        }
        pooledObject.transform.position = transform.position;
    }
}
