using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    public AudioPlayerManager audioplayermanager; // Reference to the AudioPlayerManager script

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the collider belongs to the player
        {
            // Start playing audio from the beginning
            audioplayermanager.StartPlayingAudio();
        }
    }
}
