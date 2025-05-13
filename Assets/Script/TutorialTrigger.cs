using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class TutorialTrigger : MonoBehaviour
{

    Collider col;

    public UnityEvent onEventTriggered;

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
            onEventTriggered?.Invoke();
            col.enabled = false;
        }
    }
}
