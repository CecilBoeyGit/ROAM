using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControlScript : MonoBehaviour
{
    public float moveDistance = 2f; // Distance the door should move when opening
    public float moveSpeed = 2f;
    private Vector3 originalPosition;
    private Vector3 targetPosition; // Position to which the door should move when opening
    private Vector3 moveBackPosition; // Position to which the door should move when moving back
    public bool canOpen = false;
    public bool moveback = false;

    void Start()
    {
        originalPosition = transform.position;
        targetPosition = originalPosition + transform.forward * moveDistance; // Calculate the target position
        moveBackPosition = originalPosition; //- transform.forward * moveDistance; // Calculate the move back position
    }

    public void moveBack()
    {
        moveback = true;
    }
    void Update()
    {
        if (canOpen)
        {
            // Move the door towards the target position until it reaches it
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            // Check if the door has reached or passed the target position
            if (Vector3.Distance(originalPosition, transform.position) >= moveDistance)
            {
                // Set canOpen to false to stop opening the door
                canOpen = false;
            }
        }
        else if (moveback)
        {
            // Move the door back to its original position
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);

            // Check if the door has reached or passed the move back position
            if (Vector3.Distance(moveBackPosition, transform.position) <= 0.01f)
            {
                // Set moveback to false to stop moving the door back
                moveback = false;
            }
        }
    }
}
