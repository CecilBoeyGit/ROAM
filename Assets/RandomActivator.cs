using UnityEngine;

public class RandomActivator : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToChooseFrom;

    void Start()
    {
        // Deactivate all first
        foreach (GameObject obj in objectsToChooseFrom)
        {
            obj.SetActive(false);
        }

        // Randomly pick one to activate
        if (objectsToChooseFrom.Length > 0)
        {
            int randomIndex = Random.Range(0, objectsToChooseFrom.Length);
            objectsToChooseFrom[randomIndex].SetActive(true);
        }
    }
}
