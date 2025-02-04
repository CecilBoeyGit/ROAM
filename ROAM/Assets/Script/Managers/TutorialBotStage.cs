using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TutorialBotStage : MonoBehaviour
{

    Collider col;

    public static event Action PlayerInTrigger;
    private void Start()
    {
        col = GetComponent<Collider>();
        col.enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TutorialElementPlayScript.isPlayingAudio = false;
            PlayerInTrigger?.Invoke();
            col.enabled = false;
        }
    }
}
