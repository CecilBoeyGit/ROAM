using UnityEngine;

public class ActivateOnTrigger : MonoBehaviour
{
    // The GameObject to be activated when the player enters the trigger.
    public GameObject objectToActivate;

    // This method is called when another collider enters the trigger collider attached to the object.
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider that entered belongs to the player.
        if (other.CompareTag("Player"))
        {
            // Activate the target object.
            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
            }
            else
            {
                Debug.LogWarning("objectToActivate is not assigned in the inspector.");
            }
        }
    }
}
