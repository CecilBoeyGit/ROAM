using UnityEngine;

public class ElevatorControlInput : MonoBehaviour
{
    public ElevatorScri elevator; // Reference to the Elevator script attached to the elevator GameObject
    public GameObject elevatorLight; // Reference to the Light component attached to the elevator GameObject
    public GameObject instructionText; // Reference to the TextMesh component

    void OnTriggerEnter(Collider other)
    {
        // Check if the collider entering is tagged as "Player"
        if (other.CompareTag("Player"))
        {
            // Activate the elevator light
            elevatorLight.SetActive(true);
            // Set the variable in Elevator script to true
            elevator.canCall = true;

            // Display the instruction text
            instructionText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the collider exiting is tagged as "Player"
        if (other.CompareTag("Player"))
        {
            // Deactivate the elevator light
            elevatorLight.SetActive(false);
            // Set the variable in Elevator script to false
            elevator.canCall = false;

            // Hide the instruction text
            instructionText.SetActive(false);
        }
    }
}
