using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorFenceScript : MonoBehaviour
{
    public Elevatorscript elevatorscript;
    public float moveSpeed = 2.0f; // Speed of downward movement
    public float delayBeforeMove = 20.0f; // Delay before moving down
    private bool hasDelayed = false; // Flag to track whether the delay has occurred

    // Update is called once per frame
    void Update()
    {
        if (elevatorscript.ElevatorReach)
        {
            // Check if delay has occurred
            if (!hasDelayed)
            {
                StartCoroutine(DelayBeforeMove());
               
            }
            else
            {
                // Move the object down
                transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
            }
        }
    }

    IEnumerator DelayBeforeMove()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delayBeforeMove);
        // Set the flag to true to indicate that the delay has occurred
        hasDelayed = true;
    }
}
