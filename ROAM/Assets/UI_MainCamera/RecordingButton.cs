using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for accessing the Image component

public class RecordingButton : MonoBehaviour
{
    public float onTime = 1f; // Duration for which the image will be turned on
    public float offTime = 1f; // Duration for which the image will be turned off

    private float timer = 0f;
    private bool isOn = true;

    private Image imageComponent; // Reference to the Image component

    // Start is called before the first frame update
    void Start()
    {
        // Get the reference to the Image component
        imageComponent = GetComponent<Image>();

        // Start by turning the image on
        SetImageState(true);
        timer = onTime;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        // If the timer reaches zero
        if (timer <= 0)
        {
            // Toggle the image state
            isOn = !isOn;

            // Set the timer for the next state
            timer = isOn ? onTime : offTime;

            // Set the image state
            SetImageState(isOn);
        }
    }

    // Method to turn the image on or off
    void SetImageState(bool state)
    {
        // Enable or disable the Image component based on the state
        imageComponent.enabled = state;
    }
}


