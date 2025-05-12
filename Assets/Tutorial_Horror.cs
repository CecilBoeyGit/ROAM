using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Horror : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private AudioClip collisionSound; // Sound to play on collision

    private Collider objectCollider;
    private Rigidbody targetRigidbody;
    private AudioSource audioSource;

    private void Start()
    {
        objectCollider = GetComponent<Collider>();
        if(targetObject != null)
            targetRigidbody = targetObject.GetComponent<Rigidbody>();

        // Get or add an AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Play the collision sound
            if (collisionSound != null)
            {
                audioSource.PlayOneShot(collisionSound);
            }
            DashForward();
        }
    }

    private void DashForward()
    {
        if (targetObject == null)
            return;

        // Calculate force based on the target object's forward direction
        Vector3 force = targetObject.transform.forward * dashForce;

        // Apply a one-time impulse force to dash the target object
        targetRigidbody.AddForce(force, ForceMode.Impulse);

        // Disable the collider to prevent re-triggering
        objectCollider.enabled = false;
    }
}
