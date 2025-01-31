using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FakeLoadBar : MonoBehaviour
{
    public Image image; // Reference to the Image component
    public float targetScale = 8.851736f; // Target scale for the X axis
    public float minPauseDuration = 0.2f; // Minimum pause duration
    public float maxPauseDuration = 1f; // Maximum pause duration
    public float totalTransitionTime = 5f; // Total time for the transition

    void Start()
    {
        // Start the scale animation
        StartCoroutine(ScaleImageOverTime());
    }

    IEnumerator ScaleImageOverTime()
    {
        float elapsedTime = 0f;
        float pauseDuration = 0f;

        while (image.transform.localScale.x < targetScale)
        {
            // Calculate the time remaining for the transition
            float remainingTime = totalTransitionTime - elapsedTime;

            // Randomly set the pause duration within the remaining time
            pauseDuration = Mathf.Clamp(Random.Range(minPauseDuration, maxPauseDuration), 0f, remainingTime);

            // Randomly pause
            yield return new WaitForSeconds(pauseDuration);

            // Incremental scale increase
            float currentScale = image.transform.localScale.x;
            float increment = Random.Range(0.1f, 0.5f);
            float newScale = Mathf.Min(currentScale + increment, targetScale); // Ensure it doesn't exceed targetScale
            image.transform.localScale = new Vector3(newScale, image.transform.localScale.y, image.transform.localScale.z);

            // Update elapsed time
            elapsedTime += pauseDuration;
        }
    }
}
