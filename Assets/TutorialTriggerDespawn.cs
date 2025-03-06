using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TutorialTriggerDespawn : MonoBehaviour
{
    private bool hasTriggered = false; // Ensures the trigger only happens once

    public static event Action TutorialTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player")) // Ensure it's only triggered once
        {
            hasTriggered = true; // Set flag to true
            TutorialTriggered?.Invoke();
            Destroy(gameObject); // Despawn the object
        }
    }
}
