using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cinematic_UI_Openning : MonoBehaviour
{
    public Image image; // Reference to the UI Image object
    public float duration = 30f; // Duration in seconds for the scale animation

    void Start()
    {
        // Start the vertical unfold animation
        StartCoroutine(ScaleImageOverTime());
    }

    IEnumerator ScaleImageOverTime()
    {
        float elapsedTime = 0f;
        // For vertical unfolding:
        // - initialScale has full width (x = 1) but zero height (y = 0)
        // - targetScale has full width and nearly full height (y = 0.98)
        Vector3 initialScale = new Vector3(1f, 0f, 1f); // Starting scale (x: 1, y: 0, z: 1)
        Vector3 targetScale = new Vector3(1f, 0.98f, 1f); // Target scale (x: 1, y: 0.98, z: 1)

        // Set initial scale
        image.rectTransform.localScale = initialScale;

        // Animation loop
        while (elapsedTime < duration)
        {
            // Calculate the progress of the animation
            float t = elapsedTime / duration;
            // Use an easing function to slow down as it approaches the target scale
            float easedT = EaseOutCubic(t);

            // Interpolate the scale using the eased progress
            image.rectTransform.localScale = Vector3.Lerp(initialScale, targetScale, easedT);

            // Update elapsed time
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure the scale reaches the exact target value at the end of the animation
        image.rectTransform.localScale = targetScale;
    }

    // Cubic easing out function for a decelerating effect
    float EaseOutCubic(float t)
    {
        t--;
        return t * t * t + 1;
    }
}
