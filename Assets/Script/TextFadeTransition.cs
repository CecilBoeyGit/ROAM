using System.Collections;
using UnityEngine;
using TMPro; // Make sure you include the TextMesh Pro namespace

public class TextFadeTransition : MonoBehaviour
{
    public TextMeshProUGUI textToFadeOut;
    public TextMeshProUGUI textToFadeIn;
    public float fadeDuration = 2f; // Duration in seconds for the fade out and fade in effects

    Coroutine StartFadeText;

    public void ReplayFromBeginning()
    {
        if (StartFadeText != null)
            StopCoroutine(StartFadeText);

        textToFadeIn.gameObject.SetActive(false);
        StartFadeText = StartCoroutine(FadeTextToZeroAlpha(textToFadeOut, fadeDuration));
    }

    private IEnumerator FadeTextToZeroAlpha(TextMeshProUGUI text, float duration)
    {
        // Fade out
        float startAlpha = 1;
        float rate = 1.0f / duration;
        float progress = 0.0f;

        while (progress < 1.0)
        {
            Color tmpColor = text.color;
            tmpColor.a = Mathf.Lerp(startAlpha, 0, progress);
            text.color = tmpColor;

            progress += rate * Time.deltaTime;

            yield return null;
        }

        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);

        // Ensure the text to fade in is active
        textToFadeIn.gameObject.SetActive(true);

        // Start fading in the next text
        StartCoroutine(FadeTextToFullAlpha(textToFadeIn, fadeDuration));
    }

    private IEnumerator FadeTextToFullAlpha(TextMeshProUGUI text, float duration)
    {
        // Fade in
        float startAlpha = 0;
        float rate = 1.0f / duration;
        float progress = 0.0f;

        while (progress < 1.0)
        {
            Color tmpColor = text.color;
            tmpColor.a = Mathf.Lerp(startAlpha, 1, progress);
            text.color = tmpColor;

            progress += rate * Time.deltaTime;

            yield return null;
        }

        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
    }
}
