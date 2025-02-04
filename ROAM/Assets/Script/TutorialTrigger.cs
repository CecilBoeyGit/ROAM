using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TutorialTrigger : MonoBehaviour
{

    Collider col;

    public static event Action TutorialTriggered;

    private void Start()
    {
        col = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            TutorialTriggered?.Invoke();
            col.enabled = false;
        }
    }
}
