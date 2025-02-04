using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevatorscript : MonoBehaviour
{
    public float difference = 10.0f; // Difference to subtract from the starting Y position
    public float moveSpeed = 2.0f; // Speed of the movement

    private Vector3 originalPosition; // Original position of the object
    public bool ElevatorReach=false;
    public bool shouldShiftHue = false;

    // Start is called before the first frame update
    void Start()
    {
        // Store the original position
        originalPosition = transform.position;

        // Subtract the difference from the current Y position
        transform.position = new Vector3(transform.position.x, transform.position.y - difference, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        // Move the object back to its original position
        transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
        if (transform.position == originalPosition)
        {
            StartCoroutine(WaitForHandle());
        }

    }
    IEnumerator WaitForHandle()
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);
        ElevatorReach = true;
        shouldShiftHue = true;
    }
}
