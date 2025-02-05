using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LightController : MonoBehaviour
{
    public Light  lightspot;
    //public float blinkInterval = 0.5f; // Adjust the blink interval as needed
    //public float blinkDuration = 5f; // Duration of blinking in seconds

    private float elapsedTime = 0f; // Tracks the elapsed time
    private bool isBlinking = true; // Flag to control the blinking state

    bool BlinkingActive = false;

    Coroutine CO_Blink;

    void Start()
    {
        // Make sure to assign the Light component in the Inspector
        if (lightspot == null)
        {
            lightspot = GetComponent<Light>();
        }

     
    }

    public void CallBlinkIEnum()
    {
        if(!BlinkingActive)
            CO_Blink = StartCoroutine(Blink(0.5f));
    }

    IEnumerator Blink(float blinkInterval)
    {
        BlinkingActive = true;
        while (!Blink_ExitCond())
        {
            lightspot.enabled = true;
            yield return new WaitForSeconds(blinkInterval);
            lightspot.enabled = false;
            yield return new WaitForSeconds(blinkInterval);

            elapsedTime += blinkInterval * 2;
        }

        lightspot.enabled = false;
        BlinkingActive = false;
    }
    bool Blink_ExitCond()
    {
        return isBlinking == false ;
    }

    // Method to stop the blinking
    public void StopBlinking()
    {
        isBlinking = false;
    }
}
