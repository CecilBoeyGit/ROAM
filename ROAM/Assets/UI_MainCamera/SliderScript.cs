using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderScript : MonoBehaviour
{
    public float minScale = 0.1f; // Minimum scale value
    public float maxScale = 1f; // Maximum scale value

    private float targetScale; // Target scale value
    private float animationDuration; // Duration of the animation
    private float timer; // Timer for the animation

    // Start is called before the first frame update
    void Start()
    {
        // Start the animation immediately
        SetNewScale();
    }

    // Update is called once per frame
    void Update()
    {
        // Update the scale based on the animation progress
        timer += Time.deltaTime;
        float progress = Mathf.Clamp01(timer / animationDuration);
        transform.localScale = new Vector3(Mathf.Lerp(transform.localScale.x, targetScale, progress), transform.localScale.y, transform.localScale.z);

        // If the animation is complete, start a new animation
        if (progress >= 1f)
        {
            SetNewScale();
        }
    }

    // Method to set a new target scale and animation duration
    private void SetNewScale()
    {
        // Randomize the target scale between minScale and maxScale
        targetScale = Random.Range(minScale, maxScale);

        // Randomize the animation duration between 1 and 3 seconds
        animationDuration = Random.Range(1f, 3f);

        // Reset the timer
        timer = 0f;
    }
}
