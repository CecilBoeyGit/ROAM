using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlackScreenFadeOutScript : MonoBehaviour
{
    [SerializeField] float fadeDuration = 1f; // Duration of the fade effect in seconds
    [SerializeField] Image blackScreenImage; // Reference to the image component of the black screen
    public string[] lines_Day1; // Array of lines to print
    public string[] lines_Day2; // Array of lines to print
    private float currentAlpha = 0f; // Current alpha value
    public GameObject blackscreentext;

    Coroutine CO_FadeIn;
    Coroutine CO_FadeOut;

    [SerializeField] bool PlayOnStart = false;

    LetterByLetterWithPause LetterInstance;

    public static BlackScreenFadeOutScript Instance;

    private void OnEnable()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    private void Start()
    {
        LetterInstance = LetterByLetterWithPause.Instance;

        if (PlayOnStart)
        {
            blackScreenImage.color = new Color(0f, 0f, 0f, 1f);
        }
    }
    public void TriggerFadeIn(string LetterFunc)
    {
        if (CO_FadeIn != null)
            StopCoroutine(CO_FadeIn);

        CO_FadeIn = StartCoroutine(FadeIn(LetterFunc));
    }
    public void TriggerFadeOut()
    {
        if (CO_FadeIn != null)
            StopCoroutine(CO_FadeOut);

        CO_FadeOut = StartCoroutine(FadeOut());
    }

    IEnumerator FadeIn(string LetterFunc)
    {
        float timer = 0f;

        // While the timer is less than the fade duration
        while (timer < fadeDuration)
        {
            // Increment the timer by the time elapsed since the last frame
            timer += Time.deltaTime;

            // Calculate the normalized alpha value based on the current timer value and fade duration
            float normalizedAlpha = Mathf.InverseLerp(0, fadeDuration, timer);

            // Update the alpha of the image
            blackScreenImage.color = new Color(0f, 0f, 0f, normalizedAlpha);

            // Yield to the next frame
            yield return null;
        }

        // Ensure the alpha is exactly 1 at the end of the fade
        blackScreenImage.color = new Color(0f, 0f, 0f, 1f);
        blackscreentext.SetActive(true);
        switch (LetterFunc)
        {
            case "End":
                LetterInstance.PrintEndScreen();
                break;
        }
    }
    IEnumerator FadeOut()
    {
        float timer = 0f;

        // While the timer is less than the fade duration
        while (timer < fadeDuration + 5)
        {
            // Increment the timer by the time elapsed since the last frame
            timer += Time.deltaTime;

            // Calculate the normalized alpha value based on the current timer value and fade duration
            float normalizedAlpha = Mathf.InverseLerp(fadeDuration + 5, 0, timer);

            // Update the alpha of the image
            blackScreenImage.color = new Color(0f, 0f, 0f, normalizedAlpha);

            // Yield to the next frame
            yield return null;
        }

        // Ensure the alpha is exactly 1 at the end of the fade
        blackScreenImage.color = new Color(0f, 0f, 0f, 0f);
    }

}
