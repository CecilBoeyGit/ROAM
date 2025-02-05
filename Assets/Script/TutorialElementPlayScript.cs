using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialElementPlayScript : MonoBehaviour
{
    public AudioClip[] audioClips;
    public string[] subtitles;
    private AudioSource audioSource;
    public TextMeshProUGUI subtitleText;
    private int currentIndex = 0;

    Coroutine CO_PlayingElements;

    public static bool isPlayingAudio = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        subtitleText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void PlayAudioWithSubtitles()
    {
        if (!isPlayingAudio)
        {
            if (CO_PlayingElements != null)
                StopCoroutine(CO_PlayingElements);
            
            CO_PlayingElements = StartCoroutine(PlayAudioWithSubtitlesCoroutine());
        }
    }

    IEnumerator PlayAudioWithSubtitlesCoroutine()
    {
        isPlayingAudio = true;

        audioSource.clip = audioClips[currentIndex];
        subtitleText.text = subtitles[currentIndex];
        audioSource.Play();

        while (audioSource.isPlaying)
        {
            yield return new WaitForSeconds(audioSource.clip.length);
        }

        // After playing all subtitles, clear the TextMeshProUGUI text
        subtitleText.text = "";
    }
}
