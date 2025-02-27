using UnityEngine;

public class enableTutorial : MonoBehaviour
{
    // The GameObject to enable when the player enters the trigger area
    [SerializeField] private GameObject objectToEnable;

    // This method is called when another collider enters the trigger attached to this GameObject
    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is tagged as "Player"
        if (other.CompareTag("Player"))
        {
            // Enable the specified object
            objectToEnable.SetActive(true);
        }
    }
}
