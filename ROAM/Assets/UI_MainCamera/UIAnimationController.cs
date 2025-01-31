using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimationController : MonoBehaviour
{
    public float targetScaleY = 0.36f; // Target scale for the Y axis
    public float targetAlpha = 0.5f; // Target alpha value
    public float animationDuration = 1.0f; // Duration of the animation

    public float initialScaleY = 0.1f; // Initial scale for the Y axis
    private float initialAlpha = 1.0f; // Initial alpha value
    private float timer = 0f; // Timer for the animation

    private Image image; // Reference to the Image component
    private bool animationComplete = false; // Flag to indicate if the animation is complete

    void Start()
    {
        // Get the Image component attached to this UI element
        image = GetComponent<Image>();

        // Set initial scale and alpha
        transform.localScale = new Vector3(transform.localScale.x, initialScaleY, 1f);
        SetAlpha(initialAlpha);
    }

    void Update()
    {
        // If animation is complete, do nothing
        if (animationComplete)
            return;

        // Update the timer
        timer += Time.deltaTime;

        // Calculate the progress of the animation
        float progress = Mathf.Clamp01(timer / animationDuration);

        // Apply easing function to the progress
        float easedProgress = EaseOut(progress);

        // Interpolate Y scale based on eased progress
        float scaleY = Mathf.Lerp(initialScaleY, targetScaleY, easedProgress);

        // Apply scale change only to Y axis
        transform.localScale = new Vector3(transform.localScale.x, scaleY, 1f);

        // If Y scale is within a small range around the target scale, mark animation as complete
        if (Mathf.Abs(scaleY - targetScaleY) < 0.02f)
        {
            animationComplete = true;
            timer = 0f; // Reset timer to prevent further updates
        }
    }

    // Easing function (Ease Out)
    float EaseOut(float t)
    {
        return Mathf.Sin(t * Mathf.PI * 0.5f);
    }

    // Function to set the alpha of the UI element
    void SetAlpha(float value)
    {
        // Ensure that the Image component exists
        if (image != null)
        {
            // Get the color of the image
            Color color = image.color;

            // Update the alpha value of the color
            color.a = value;

            // Assign the updated color back to the image
            image.color = color;
        }
    }
}
