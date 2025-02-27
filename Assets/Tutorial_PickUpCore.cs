using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_PickUpCore : MonoBehaviour
{
    [SerializeField] private DoorControlScriptTrigger doorControlScriptTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                PowerCores powerCore = player.GetComponentInChildren<PowerCores>();
                if (powerCore != null && powerCore.PCStates == PowerCores.PowerCoreState.Equipped)
                {
                    doorControlScriptTrigger.Day01Door = true;
                }
            }
        }
    }
}