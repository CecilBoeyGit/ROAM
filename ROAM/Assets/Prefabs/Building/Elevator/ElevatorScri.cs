using UnityEngine;
using System.Collections;

public class ElevatorScri : MonoBehaviour
{
    public Transform[] floors;  // Array of floor positions
    public float speed = 5f;    // Speed at which the elevator moves

    private bool moving = false;
    private bool movingUp = true; // Flag to track movement direction
    public bool canCall = false;  // Flag to check if elevator is moving

    void Update()
    {
        // Check for user input to move the elevator
        if (Input.GetKeyDown(KeyCode.F) && !moving)
        {
            ToggleMovementDirection();
            MoveElevator();
        }
    }

    // Method to toggle the movement direction
    void ToggleMovementDirection()
    {
        movingUp = !movingUp;

        // Move the elevator immediately after toggling the direction
        MoveElevator();
    }

    // Method to move the elevator
    void MoveElevator()
    {
        if (movingUp)
            MoveToNextFloor();
        else
            MoveToPreviousFloor();
    }

    // Method to move the elevator to the next floor
    void MoveToNextFloor()
    {
        Vector3 targetPosition = transform.position;
        targetPosition.y = floors[1].position.y; // Update only Y position
        StartCoroutine(MoveElevatorCoroutine(targetPosition));
    }

    // Method to move the elevator to the previous floor
    void MoveToPreviousFloor()
    {
        Vector3 targetPosition = transform.position;
        targetPosition.y = floors[0].position.y; // Update only Y position
        StartCoroutine(MoveElevatorCoroutine(targetPosition));
    }

    // Coroutine to move the elevator smoothly
    IEnumerator MoveElevatorCoroutine(Vector3 targetPosition)
    {
        moving = true;
        while (transform.position != targetPosition)
        {
            // Move elevator towards target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        moving = false;
    }

    // Collision detection with the floor collider to stop the elevator
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            // Stop elevator when colliding with the floor
            moving = false;
        }
    }
}
