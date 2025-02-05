using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonPlaySound : MonoBehaviour
{
    public AudioClip soundEffect; // Sound effect to play
    public float delayBeforeSceneLoad = 1.0f; // Delay before loading the next scene
    public float scaleDuration = 0.5f; // Duration for scaling animation

    private Button button;
    private AudioSource audioSource;
    private Vector3 initialScale;

    void Start()
    {
        // Get the Button component attached to this GameObject
        button = GetComponent<Button>();

        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();

        // Get the initial scale of the button
        initialScale = transform.localScale;

        // Add a listener to the button click event
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        // Play the sound effect
        if (soundEffect != null && audioSource != null)
        {
            audioSource.PlayOneShot(soundEffect);
            StartCoroutine(ScaleButton());
            StartCoroutine(LoadNextSceneAfterDelay());
        }
    }

    IEnumerator LoadNextSceneAfterDelay()
    {
        // Wait for the sound effect to finish playing
        yield return new WaitForSeconds(delayBeforeSceneLoad);

        // Transition to the next scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator ScaleButton()
    {
        float timer = 0f;
        while (timer < scaleDuration)
        {
            timer += Time.deltaTime;
            float scaleFactor = Mathf.Lerp(1f, 0f, timer / scaleDuration);
            transform.localScale = new Vector3(initialScale.x, scaleFactor * initialScale.y, initialScale.z);
            yield return null;
        }
    }
}
