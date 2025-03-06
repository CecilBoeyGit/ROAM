using System.Collections;
using UnityEngine;

public class MusicTransitionScript : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource1;
    [SerializeField] private AudioSource audioSource2;
    float aud1StartVolume, aud2StartVolume;
    public float transitionDuration = 0.5f; // Duration of the transition in seconds
    //private bool isTransitioning = false;
    bool currentTransitionBoolean = false;

    // Start is called before the first frame update
    void Start()
    {
        // Play audios by default
        aud1StartVolume = audioSource1.volume;
        audioSource1.Play();
        aud2StartVolume = audioSource2.volume;
        audioSource2.volume = 0;
        audioSource2.Play();
    }

    public void StartTransition(bool activateSource2)
    {
        if (activateSource2 != currentTransitionBoolean)
        {
            currentTransitionBoolean = activateSource2;
            StartCoroutine(TransitionAudio(activateSource2));
        }
    }

    private IEnumerator TransitionAudio(bool activateSource2)
    {
        float elapsedTime = 0f;
        float aud1Start = audioSource1.volume;
        float aud2Start = audioSource2.volume;

        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            audioSource1.volume = Mathf.Lerp(aud1Start, activateSource2 ? 0f : aud1StartVolume, t);
            audioSource2.volume = Mathf.Lerp(aud2Start, activateSource2 ? aud2StartVolume : 0f, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure volumes are set correctly at the end of the transition
        audioSource1.volume = activateSource2 ? 0f : aud1StartVolume;
        audioSource2.volume = activateSource2 ? aud2StartVolume : 0f;
    }
}
