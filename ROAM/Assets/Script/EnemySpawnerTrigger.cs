using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerTrigger : MonoBehaviour
{

    public bool PlayerInZone = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
            PlayerInZone = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
            PlayerInZone = false;
    }
}
