using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayerManager : MonoBehaviour
{
    [SerializeField] List<AudioClip> audioClips = new List<AudioClip>(); // List of audio clips
    private AudioSource audioSource; // Reference to the AudioSource component
    public bool playFromBeginning = false; // Variable to control whether to play from the beginning
    private bool hasStartedPlaying = false; // Flag to track whether the audio has started playing
    private int currentClipIndex = 0; // Index of the current audio clip being played

    // Start is called before the first frame update
    void Start()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();

        // Subscribe to the AudioSource's end event
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.volume = 1f;
        audioSource.spatialBlend = 0f;
        audioSource.dopplerLevel = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if playFromBeginning is true, if so, play the first audio clip
        if (playFromBeginning && audioClips.Count > 0 && !hasStartedPlaying)
        {
            PlayAudioClip(0);
            hasStartedPlaying = true;
        }
    }

    // Function to play an audio clip based on its index in the list
    public void PlayAudioClip(int index)
    {
        // Check if the index is within the bounds of the list
        if (index >= 0 && index < audioClips.Count)
        {
            // Set the audio clip to play
            audioSource.clip = audioClips[index];

            // Play the audio
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Invalid audio clip index!");
        }
    }

    // Method to handle the event when an audio clip ends
    private void OnAudioClipEnd()
    {
        // Check if there are more audio clips to play
        if (currentClipIndex + 1 < audioClips.Count)
        {
            // Increment the index to play the next audio clip
            currentClipIndex++;
            // Play the next audio clip
            PlayAudioClip(currentClipIndex);
        }
        else
        {
            // If all clips have been played, reset the index and stop playing
            currentClipIndex = 0;
            hasStartedPlaying = false;
            playFromBeginning = false;
        }
    }

    // Method to handle the boolean controlled by another object
    public void StartPlayingAudio()
    {
        // Set playFromBeginning to true to start playing the audio
        playFromBeginning = true;
    }
}
