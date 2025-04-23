using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomActivator : MonoBehaviour
{
    private List<GameObject> objectsToChooseFrom = new List<GameObject>();

    private void OnEnable()
    {
        foreach (Transform child in this.transform)
        {
            objectsToChooseFrom.Add(child.gameObject);
        }
    }
    void Start()
    {
        // Deactivate all first
        foreach (GameObject obj in objectsToChooseFrom)
        {
            obj.SetActive(false);
        }

        // Randomly pick one to activate
        if (objectsToChooseFrom.Count > 0)
        {
            int randomIndex = Random.Range(0, objectsToChooseFrom.Count - 1);
            objectsToChooseFrom[randomIndex].SetActive(true);
        }
    }
}
