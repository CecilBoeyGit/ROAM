using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElementPointerAreaScript : MonoBehaviour
{
    public GameObject objectToActivate; // Reference to the object to activate

    // Called when another collider enters the trigger zone
    void OnTriggerEnter(Collider other)
    {
        // Check if the other collider belongs to the player
        if (other.CompareTag("Player"))
        {
            // Set the object to active
            objectToActivate.SetActive(true);
        }
    }

    // Called when another collider exits the trigger zone
    void OnTriggerExit(Collider other)
    {
        // Check if the other collider belongs to the player
        if (other.CompareTag("Player"))
        {
            // Set the object to inactive
            objectToActivate.SetActive(false);
        }
    }
}
