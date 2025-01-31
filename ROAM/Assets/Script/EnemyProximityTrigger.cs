using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProximityTrigger : MonoBehaviour
{
    [SerializeField] string type = "Enemy";
    public LightController mylight;



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == type)
        {
            mylight.CallBlinkIEnum();
        }
    }
}