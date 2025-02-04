using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRandomIntensity : MonoBehaviour
{
    public float minIntensity = 1f; // Minimum intensity of the light
    public float maxIntensity = 5f; // Maximum intensity of the light
    public float changeInterval = 1f; // Interval for changing intensity

    private Light lightComponent; // Reference to the Light component
    private float timeSinceLastChange = 0f; // Time passed since last intensity change

    // Start is called before the first frame update
    void Start()
    {
        // Get the Light component attached to this GameObject
        lightComponent = GetComponent<Light>();

        // Set the initial intensity of the light
        lightComponent.intensity = Random.Range(minIntensity, maxIntensity);
    }

    // Update is called once per frame
    void Update()
    {
        // Update the time since the last intensity change
        timeSinceLastChange += Time.deltaTime;

        // Check if it's time to change the intensity
        if (timeSinceLastChange >= changeInterval)
        {
            // Change the intensity to a random value within the specified range
            lightComponent.intensity = Random.Range(minIntensity, maxIntensity);

            // Reset the timer for next intensity change
            timeSinceLastChange = 0f;
        }
    }
}
