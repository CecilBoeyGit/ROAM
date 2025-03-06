using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialLineTrigger : MonoBehaviour
{
    [Header("Audio Settings")]
    public List<AudioClip> audioClips = new List<AudioClip>();

    [Header("Reset Options")]
    [Tooltip("Enable to allow resetting the audio index via the R key.")]
    public bool enableResetKey = true;

    private AudioSource audioSource;
    private int currentClipIndex = 0;
    private const string PREFS_CLIP_INDEX = "CurrentClipIndex";

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;

        // Load the current clip index from PlayerPrefs, if available.
        if (PlayerPrefs.HasKey(PREFS_CLIP_INDEX))
            currentClipIndex = PlayerPrefs.GetInt(PREFS_CLIP_INDEX);
        else
            currentClipIndex = 0;
    }

    private void Update()
    {
        // Only allow resetting if the reset key is enabled.
        if (enableResetKey && Input.GetKeyDown(KeyCode.R))
        {
            ResetAudioIndex();
            Debug.Log("Audio index reset!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player.
        if (other.CompareTag("Player"))
        {
            PlayNextLine();
        }
    }

    private void PlayNextLine()
    {
        // If all clips have been played, do nothing.
        if (currentClipIndex >= audioClips.Count)
            return;

        // Stop any currently playing clip.
        if (audioSource.isPlaying)
            audioSource.Stop();

        // Play the next audio clip.
        AudioClip nextClip = audioClips[currentClipIndex];
        if (nextClip != null)
        {
            audioSource.clip = nextClip;
            audioSource.Play();
            currentClipIndex++;

            // Save the new index so it persists.
            PlayerPrefs.SetInt(PREFS_CLIP_INDEX, currentClipIndex);
            PlayerPrefs.Save();
        }
    }

    public void ResetAudioIndex()
    {
        currentClipIndex = 0;
        PlayerPrefs.DeleteKey(PREFS_CLIP_INDEX);
        PlayerPrefs.Save();
    }
}
