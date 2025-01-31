using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerManager : MonoBehaviour
{
    public List<VideoClip> videoClips = new List<VideoClip>(); // List of video clips
    private VideoPlayer videoPlayer; // Reference to the VideoPlayer component
    public bool playFromBeginning = false; // Variable to control whether to play from the beginning
    private bool hasStartedPlaying = false; // Flag to track whether the video has started playing
    private int currentClipIndex = 0; // Index of the current video clip being played

    // Start is called before the first frame update
    void Start()
    {
        // Get the VideoPlayer component attached to this GameObject
        videoPlayer = GetComponent<VideoPlayer>();

        // Subscribe to the VideoPlayer's loopPointReached event
        videoPlayer.loopPointReached += OnVideoClipEnd;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if playFromBeginning is true, if so, play the first video clip
        if (playFromBeginning && videoClips.Count > 0 && !hasStartedPlaying)
        {
            PlayVideoClip(0);
            hasStartedPlaying = true;
        }
    }

    // Function to play a video clip based on its index in the list
    public void PlayVideoClip(int index)
    {
        // Check if the index is within the bounds of the list
        if (index >= 0 && index < videoClips.Count)
        {
            // Set the video clip to play
            videoPlayer.clip = videoClips[index];

            // Play the video
            videoPlayer.Play();
        }
        else
        {
            Debug.LogWarning("Invalid video clip index!");
        }
    }

    // Method to handle the event when a video clip ends
    private void OnVideoClipEnd(VideoPlayer vp)
    {
        // Check if there are more video clips to play
        if (currentClipIndex + 1 < videoClips.Count)
        {
            // Increment the index to play the next video clip
            currentClipIndex++;
            // Play the next video clip
            PlayVideoClip(currentClipIndex);
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
    public void StartPlayingVideo()
    {
        // Set playFromBeginning to true to start playing the video
        playFromBeginning = true;
    }
}
