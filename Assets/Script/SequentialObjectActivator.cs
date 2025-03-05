using System.Collections;
using UnityEngine;
using TMPro;

public class SequentialObjectActivatorWithText : MonoBehaviour
{
    public GameObject[] objectsToActivate; // List of objects to cycle through
    public float[] switchDelays; // Custom delays after text has finished

    private int currentIndex = 0;

    void Start()
    {
        // Ensure only the first object is active initially
        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            objectsToActivate[i].SetActive(i == 0);
        }

        if (objectsToActivate.Length > 0)
        {
            StartCoroutine(SwitchObjects());
        }
        else
        {
            Debug.LogError("No objects assigned in the list!");
        }
    }

    private IEnumerator SwitchObjects()
    {
        while (currentIndex < objectsToActivate.Length - 1)
        {
            // Check if the object has the LetterByLetterWithPause script
            LetterByLetterWithPause textScript = objectsToActivate[currentIndex].GetComponent<LetterByLetterWithPause>();

            if (textScript != null)
            {
                // Wait for the text to finish before continuing
                yield return new WaitUntil(() => textScript.IsTextFinished());
            }

            // Get delay time (or use the last one if out of range)
            float delay = (currentIndex < switchDelays.Length) ? switchDelays[currentIndex] : switchDelays[switchDelays.Length - 1];

            yield return new WaitForSeconds(delay); // Apply delay after text finishes

            // Deactivate current object
            objectsToActivate[currentIndex].SetActive(false);

            // Move to the next object
            currentIndex++;

            // Activate the next object
            objectsToActivate[currentIndex].SetActive(true);
        }
    }
}
