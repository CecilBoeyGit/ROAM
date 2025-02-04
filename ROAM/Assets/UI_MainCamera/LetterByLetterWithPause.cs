using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LetterByLetterWithPause : MonoBehaviour
{
    public TMP_Text textMeshPro; // Reference to the TextMeshPro object
    public string[] OpeningCut;
    public string[] OpeningButtonText;
    public string[] EndScreen;
    public string[] OnboardingDay;
    public string[] Day01;

    public float letterDelay = 0.1f; // Delay between each letter
    public float linePauseDuration = 1f; // Pause duration after each line has been fully printed

    [SerializeField] bool PlayOnStart = false;
    public bool isPrintingText = false;

    [SerializeField] bool OpeningCutScene = false;
    [SerializeField] bool OpeningButton = false;
    [SerializeField] bool PrintEnd = false;
    [SerializeField] bool PrintOnboarding = false;
    [SerializeField] bool PrintDay01 = false;

    BlackScreenFadeOutScript BlackScreenInstance;
    PlayerController PlayerInstance;

    public static LetterByLetterWithPause Instance;

    private void OnEnable()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    private void Start()
    {
        BlackScreenInstance = BlackScreenFadeOutScript.Instance;
        PlayerInstance = PlayerController.instance;

        isPrintingText = false;

        if (PlayOnStart)
        {
            if (OpeningCutScene)
                StartCoroutine(PrintText(OpeningCut, false));
            else if (OpeningButton)
                StartCoroutine(PrintText(OpeningButtonText, false));
            else if (PrintEnd)
                StartCoroutine(PrintText(EndScreen, false));
            else if (PrintOnboarding)
                StartCoroutine(PrintText(OnboardingDay, true));
            else if (PrintDay01)
                StartCoroutine(PrintText(Day01, true));
        }
    }
    public void PrintEndScreen()
    {
        if(!isPrintingText)
            StartCoroutine(PrintText(EndScreen, false));
    }
    public void PrintOnBoarding()
    {
        if (!isPrintingText)
            StartCoroutine(PrintText(OnboardingDay, true));
    }
    public void PrintDayOne()
    {
        if (!isPrintingText)
            StartCoroutine(PrintText(Day01, true));
    }
    IEnumerator PrintText(string[] screentext, bool FadeBool)
    {
        isPrintingText = true;
        if(PlayerInstance != null)
            PlayerInstance.PlayerConstrained = true;

        string fullText = "";
        foreach (string line in screentext)
        {
            fullText += line + "\n"; 
        }

        foreach (char c in fullText)
        {
            textMeshPro.text += c; 
            yield return new WaitForSeconds(letterDelay);
        }

        textMeshPro.text = "";
        yield return new WaitForSeconds(linePauseDuration);
        if (FadeBool)
            BlackScreenInstance.TriggerFadeOut();

        if (screentext == EndScreen)
        {
            if (SceneManager.GetActiveScene().name.Equals("S_LevelBlockout"))
                SceneManager.LoadScene("S_DayLoop");
        }

        PlayerInstance.PlayerConstrained = false;
    }
}
