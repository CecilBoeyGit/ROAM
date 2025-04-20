using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayRandomSound : MonoBehaviour
{
    [Header("Audio Setup")]
    public AudioSource randomSound;
    public AudioClip[] audioSources;

    [Header("Normal Mode")]
    public int clipDelay = 5;

    [Header("Check for Success Mode")]
    public bool success = false;

    void Start()
    {
        if (success)
        {
            PlayOnceAndRestart();
        }
        else
        {
            StartAudioLoop();
        }
    }

    // Normal looping behavior
    void StartAudioLoop()
    {
        Invoke(nameof(PlayRandomClip), clipDelay);
    }

    void PlayRandomClip()
    {
        randomSound.clip = audioSources[Random.Range(0, audioSources.Length)];
        randomSound.Play();
        StartAudioLoop();
    }

    // Success behavior: one‐shot play, then reload scene
    void PlayOnceAndRestart()
    {
        // pick & play
        randomSound.clip = audioSources[Random.Range(0, audioSources.Length)];
        randomSound.Play();
        // schedule reload when done
        StartCoroutine(RestartAfterClip());
    }

    IEnumerator RestartAfterClip()
    {
        // Wait exactly the clip's length
        yield return new WaitForSeconds(randomSound.clip.length);
        // reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
