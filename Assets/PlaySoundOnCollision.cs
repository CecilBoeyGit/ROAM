using UnityEngine;

public class PlaySoundOnCollision : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip collisionSound; // Assign the sound in the Inspector
    private AudioSource audioSource;
    private bool hasPlayed = false; // Ensures sound only plays once per collision

    private void Start()
    {
        // Add an AudioSource component dynamically if not assigned
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false; // Prevent auto-play
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasPlayed && other.CompareTag("Player")) // Ensure it's the player and hasn't played yet
        {
            PlaySound();
            hasPlayed = true; // Prevents replay on re-collision
        }
    }

    private void PlaySound()
    {
        if (collisionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collisionSound);
        }
        else
        {
            Debug.LogWarning("No collision sound assigned or missing AudioSource!");
        }
    }
}
