using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PlayRandomSound : MonoBehaviour
{
    [Header("Audio Setup")]
    public AudioSource randomSound;
    public AudioClip[] audioSources;

    [Header("Subtitles Setup")]
    [Tooltip("Drag in your TextMeshProUGUI component here")]
    public TextMeshProUGUI subtitleText;
    [Tooltip("One subtitle string for each clip above")]
    [TextArea]
    public string[] subtitles;

    [Header("Normal Mode")]
    public int clipDelay = 5;

    [Header("If to restart OR transition to next scene")]
    public bool restartCurrentScene = false;

    [Header("Check for Success Mode")]
    public bool success = false;

    PlayerController PlayerInstance;

    void Start()
    {
        PlayerInstance = PlayerController.instance;

        // clear any old subtitle
        subtitleText.text = "";

        if (success)
            PlayOnceAndRestart();
        else
            StartAudioLoop();
    }

    // Normal looping behavior
    void StartAudioLoop()
    {
        Invoke(nameof(PlayRandomClip), clipDelay);
    }

    void PlayRandomClip()
    {
        int idx = Random.Range(0, audioSources.Length);
        PlayClipAtIndex(idx);

        // schedule next iteration
        StartAudioLoop();
    }

    // Success behavior: one‐shot play, then reload scene
    void PlayOnceAndRestart()
    {
        int idx = Random.Range(0, audioSources.Length);
        PlayClipAtIndex(idx);

        StartCoroutine(RestartAfterClip(restartCurrentScene));
    }

    // Shared logic to play a clip + show subtitle
    void PlayClipAtIndex(int idx)
    {
        // set clip & play
        randomSound.clip = audioSources[idx];
        randomSound.Play();

        // set subtitle if available
        if (idx < subtitles.Length && subtitleText != null)
            subtitleText.text = subtitles[idx];
        else
            subtitleText.text = "";

        // clear subtitle when done
        Invoke(nameof(ClearSubtitle), randomSound.clip.length);
    }

    IEnumerator RestartAfterClip(bool restartCurrent)
    {
        if (PlayerInstance != null)
            PlayerInstance.PlayerConstrained = true;

        // wait until the clip is done
        yield return new WaitForSeconds(randomSound.clip.length);

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int totalSceneIndex = SceneManager.sceneCountInBuildSettings;

        if (restartCurrent)
        {
            // reload current scene
            SceneManager.LoadScene(currentSceneIndex);
        }
        else
        {
            SceneManager.LoadScene((currentSceneIndex + 1) % totalSceneIndex);
        }
    }

    void ClearSubtitle()
    {
        subtitleText.text = "";
    }
}
