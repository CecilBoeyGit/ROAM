using System.Collections;
using UnityEngine;

public class MusicTransitionScript : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource1;
    [SerializeField] private AudioSource audioSource2;
    public float transitionDuration = 1.0f; // Duration of the transition in seconds
    private bool isTransitioning = false;

    // Start is called before the first frame update
    void Start()
    {
        // Play audioSource1 by default
        audioSource1.Play();
    }

    public void StartTransition(bool activateSource2)
    {
        if (isTransitioning==false)
        {
            isTransitioning = true;
            StartCoroutine(TransitionAudio(activateSource2));
        }
    }

    private IEnumerator TransitionAudio(bool activateSource2)
    {
        float elapsedTime = 0f;
        float startVolume1 = audioSource1.volume;
        float startVolume2 = audioSource2.volume;

        // Only play audioSource2 if transitioning to it
        if (activateSource2)
            audioSource2.Play();

        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            audioSource1.volume = Mathf.Lerp(startVolume1, activateSource2 ? 0f : startVolume1, t);
            audioSource2.volume = Mathf.Lerp(startVolume2, activateSource2 ? startVolume2 : 0f, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure volumes are set correctly at the end of the transition
        audioSource1.volume = activateSource2 ? 0f : 1f;
        audioSource2.volume = activateSource2 ? 1f : 0f;

        isTransitioning = false;
    }
}
