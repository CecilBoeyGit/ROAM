using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateObjectAfterDelay : MonoBehaviour
{
    public GameObject objectToActivate;
    public float delayInSeconds = 2f;

    void Start()
    {
        // Start the coroutine to activate the object after a delay
        StartCoroutine(ActivateObjectCoroutine());
    }

    System.Collections.IEnumerator ActivateObjectCoroutine()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delayInSeconds);

        // Activate the object
        objectToActivate.SetActive(true);

        // Deactivate this object
        gameObject.SetActive(false);
    }
}
