using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelTrigger : MonoBehaviour
{
    [SerializeField] private float delayBeforeNextLevel = 5f; // Time in seconds before loading next level
    private bool hasTriggered = false; // Ensures it only triggers once

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player")) // Ensure it's the player and only triggers once
        {
            hasTriggered = true; // Set flag to true
            StartCoroutine(LoadNextLevelAfterDelay());
        }
    }

    private IEnumerator LoadNextLevelAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeNextLevel); // Wait for the designated time

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1; // Get next scene index
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings) // Ensure there's a next level
        {
            SceneManager.LoadScene(nextSceneIndex); // Load next level
        }
        else
        {
            Debug.Log("No more levels to load!");
        }
    }
}
