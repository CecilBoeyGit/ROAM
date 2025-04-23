using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlackScreenFadeOutScript : MonoBehaviour
{
    [SerializeField] float fadeDuration = 1f; // Duration of the fade effect in seconds
    [SerializeField] Image blackScreenImage; // Reference to the black screen image

    [SerializeField] GameObject[] blackscreentexts; // Array of black screen texts

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

        // Ensure all black screen texts are initially disabled
        SetBlackScreenTextsActive(false);
    }

    public void TriggerFadeIn(string LetterFunc)
    {
        if (CO_FadeIn != null)
            StopCoroutine(CO_FadeIn);

        CO_FadeIn = StartCoroutine(FadeIn(LetterFunc));
    }

    public void TriggerFadeOut()
    {
        if (CO_FadeOut != null)
            StopCoroutine(CO_FadeOut);

        CO_FadeOut = StartCoroutine(FadeOut());
    }

    IEnumerator FadeIn(string LetterFunc)
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            print("FadeIn: " + timer);
            timer += Time.deltaTime;
            float normalizedAlpha = Mathf.InverseLerp(0, fadeDuration, timer);
            blackScreenImage.color = new Color(0f, 0f, 0f, normalizedAlpha);
            yield return null;
        }

        // Ensure the alpha is exactly 1
        blackScreenImage.color = new Color(0f, 0f, 0f, 1f);

        // Activate all blackscreentext objects
        SetBlackScreenTextsActive(true);

        switch (LetterFunc)
        {
            case "End":
                LetterInstance.PrintEndScreen();
                break;
            case "Success":
                LetterInstance.PrintSuccessScreen();
                break;
            case "Fail":
                LetterInstance.PrintFailureScreen();
                break;
        }
    }

    IEnumerator FadeOut()
    {
        float timer = 0f;

        // Deactivate all blackscreentext objects before fading out
        SetBlackScreenTextsActive(false);

        while (timer < fadeDuration + 5)
        {
            timer += Time.deltaTime;
            float normalizedAlpha = Mathf.InverseLerp(fadeDuration + 5, 0, timer);
            blackScreenImage.color = new Color(0f, 0f, 0f, normalizedAlpha);
            yield return null;
        }

        blackScreenImage.color = new Color(0f, 0f, 0f, 0f);
    }

    // Helper function to activate/deactivate all black screen texts
    private void SetBlackScreenTextsActive(bool state)
    {
        if (blackscreentexts != null)
        {
            foreach (GameObject textObject in blackscreentexts)
            {
                if (textObject != null)
                    textObject.SetActive(state);
            }
        }
    }
}
