using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Horror : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private float dashForce = 10f;

    private Collider objectCollider;
    private Rigidbody targetRigidbody;

    private void Start()
    {
        objectCollider = GetComponent<Collider>();
        targetRigidbody = targetObject.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DashForward();
        }
    }

    private void DashForward()
    {
        // Use the target object's forward direction to calculate the force
        Vector3 force = targetObject.transform.forward * dashForce;

        // Apply a one-time impulse force to dash the target object
        targetRigidbody.AddForce(force, ForceMode.Impulse);

        // Optionally disable the collider to prevent re-triggering
        objectCollider.enabled = false;
    }
}
