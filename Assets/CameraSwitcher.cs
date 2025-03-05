using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera playerVCam;  // Player's Virtual Camera
    public CinemachineVirtualCamera targetVCam;  // Target Virtual Camera
    public float transitionDuration = 2f; // Time before switching back

    [Header("Object Activation")]
    public GameObject objectToEnable; // Object to activate after transition

    private bool isPaused = false; // Track whether the countdown is paused
    private bool hasTriggered = false; // Ensures trigger only activates once

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player")) // Ensure trigger only happens once
        {
            hasTriggered = true;
            StartCoroutine(SwitchCameraAndActivateObject());
        }
    }

    private IEnumerator SwitchCameraAndActivateObject()
    {
        // Switch to target camera
        targetVCam.Priority = 20;
        playerVCam.Priority = 10;

        // Wait for transition duration before activating the object
        yield return StartCoroutine(CountdownTimer(transitionDuration));

        // Enable the object
        if (objectToEnable != null)
        {
            objectToEnable.SetActive(true);
            isPaused = true; // Pause countdown
        }

        // Wait until the object is deactivated
        yield return new WaitUntil(() => objectToEnable == null || !objectToEnable.activeSelf);

        // Resume countdown before switching back
        isPaused = false;
        yield return StartCoroutine(CountdownTimer(transitionDuration));

        // Switch back to player camera
        targetVCam.Priority = 10;
        playerVCam.Priority = 20;
    }

    private IEnumerator CountdownTimer(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (!isPaused) // Only count if not paused
            {
                elapsedTime += Time.deltaTime;
            }
            yield return null; // Wait for next frame
        }
    }
}
