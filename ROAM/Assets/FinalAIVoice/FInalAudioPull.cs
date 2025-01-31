using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;


public class FinalAudioPull : MonoBehaviour
{
    public AudioClip[] soundClips; // Array to hold your sound clips
    public TextMeshProUGUI textMeshProObject;
    //public Dictionary<string, float> myDictionary = new Dictionary<string, float>();
    private AudioSource audioSource; // Reference to the AudioSource component
    private List<int> playedIndices = new List<int>(); // List to keep track of played indices
    public List<string> Script = new List<string>(); // List to hold script lines for each audio clip

    void Start()
    {
        textMeshProObject.gameObject.SetActive(false);
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component attached to the GameObject

        // Populate the dictionary with file names and their corresponding durations
       
    }

    private void Update()
    {
        if (audioSource.isPlaying == false)
        {
            textMeshProObject.gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {

            PlayRandomSound();
        }
    }

    public void PlayRandomSound()
    {
        if (soundClips.Length > 0) // Ensure there are sound clips in the array
        {
            int availableClipCount = soundClips.Length - playedIndices.Count;
            if (availableClipCount <= 0)
            {
                Debug.LogWarning("All sound clips have been played!");
                return;
            }

            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, soundClips.Length); // Generate a random index
            } while (playedIndices.Contains(randomIndex)); // Ensure the clip has not been played before

            AudioClip randomClip = soundClips[randomIndex]; // Get the random AudioClip from the array
            audioSource.clip = randomClip; // Set the AudioClip to play
            audioSource.Play(); // Play the sound

           
            // Display the script line associated with the random clip
            if (randomIndex < Script.Count)
            {
                textMeshProObject.gameObject.SetActive(true);
                textMeshProObject.text = Script[randomIndex];
            }
            else
            {
                Debug.LogWarning("Script line not available for the selected sound clip!");
            }

            playedIndices.Add(randomIndex); // Add the index to the list of played indices
         
        }
        else
        {
            Debug.LogWarning("No sound clips assigned to play!");
        }
    }
}
