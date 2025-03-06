using UnityEngine;

public class ActivateObjectBool : MonoBehaviour
{
    [SerializeField] private GameObject targetObject; // Object to activate
    [SerializeField] private bool isActivated = false; // Boolean flag
    [SerializeField] private string playerTag = "Player"; // Player tag for detection

    private bool playerInTrigger = false; // Checks if player is inside the trigger
    private LetterByLetterWithPause letterScript; // Reference to LetterByLetterWithPause

    void Start()
    {
        // Get reference to LetterByLetterWithPause Singleton
        letterScript = LetterByLetterWithPause.Instance;
    }

    void Update()
    {
        // Activate the object if both conditions are met
        if (isActivated && playerInTrigger)
        {
            targetObject.SetActive(true);

            // Call a function from LetterByLetterWithPause (e.g., PrintEndScreen)
            if (letterScript != null)
            {
                letterScript.PrintEndScreen();
            }
        }
    }

    // Public method to toggle activation
    public void SetActivation(bool state)
    {
        isActivated = state;
    }

    // Detect when the player enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag)) // Ensure it's the player
        {
            playerInTrigger = true;
        }
    }

    // Detect when the player exits the trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInTrigger = false;
        }
    }
}
