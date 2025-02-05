using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObjectScript : MonoBehaviour
{
    public bool startRotating = false; // Set this to true to start rotating the object
    public float rotationSpeed = 30f; // Speed of rotation in degrees per second

    // Update is called once per frame
    void Update()
    {
        if (startRotating)
        {
            // Rotate the object around its central axis
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
}
