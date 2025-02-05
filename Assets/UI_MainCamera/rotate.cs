using UnityEngine;

public class rotate : MonoBehaviour
{
    public float rotationSpeed = 50f; // Adjust the rotation speed in degrees per second

    void Update()
    {
        // Rotate the object around its central axis
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}
