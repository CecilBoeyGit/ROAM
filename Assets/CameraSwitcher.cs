using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera playerVCam;  // Player's Virtual Camera
    public CinemachineVirtualCamera targetVCam;  // Target Virtual Camera

    [Header("Timing")]
    public float transitionDuration = 2f; // Countdown duration before action

    [Header("Object Activation Options")]
    public GameObject objectToEnable; // Object to activate after transition
    public bool activateObjectAfterTransition = true; // Toggle for object activation

    private bool isPaused = false; // Track whether the countdown is paused
    private bool hasTriggered = false; // Ensures trigger only activates once

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(SwitchCameraAndHandleObject());
        }
    }

    private IEnumerator SwitchCameraAndHandleObject()
    {
        // Switch to target camera
        targetVCam.Priority = 20;
        playerVCam.Priority = 10;

        // Wait for the countdown duration before handling object activation or continuing
        yield return StartCoroutine(CountdownTimer(transitionDuration));

        if (activateObjectAfterTransition && objectToEnable != null)
        {
            // Activate the object and pause the countdown while it is active
            objectToEnable.SetActive(true);
            isPaused = true;

            // Wait until the object is deactivated (e.g., via a button or other script)
            yield return new WaitUntil(() => objectToEnable == null || !objectToEnable.activeSelf);
            isPaused = false;
        }
        else
        {
            // If not activating the object, wait for an additional countdown duration
            yield return StartCoroutine(CountdownTimer(transitionDuration));
        }

        // Switch back to the player camera
        targetVCam.Priority = 10;
        playerVCam.Priority = 20;
    }

    private IEnumerator CountdownTimer(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (!isPaused)
            {
                elapsedTime += Time.deltaTime;
            }
            yield return null;
        }
    }
}
