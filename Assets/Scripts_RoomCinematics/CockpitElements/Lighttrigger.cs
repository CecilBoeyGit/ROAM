using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighttrigger : MonoBehaviour
{
    public GameObject scenelights; // Reference to the GameObject to activate

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // You can change "Player" to the tag of the object you want to trigger the activation
        {
            // Activate the specified GameObject
            scenelights.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
