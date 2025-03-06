using System.Collections;
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
    public string[] RundownSuccessText;
    public string[] RundownFailedText;

    public float letterDelay = 0.1f; // Delay between each letter
    public float linePauseDuration = 1f; // Pause after text prints
    public float textClearDelay = 2f; // Delay before clearing text

    [SerializeField] bool PlayOnStart = false;
    public bool isPrintingText = false;

    [SerializeField] bool OpeningCutScene = false;
    [SerializeField] bool OpeningButton = false;
    [SerializeField] bool PrintEnd = false;
    [SerializeField] bool PrintOnboarding = false;
    [SerializeField] bool PrintDay01 = false;
    public bool PrintSuccess = false;
    public bool PrintFailure = false;

    Animator InterfaceVolumeAnim;

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

        InterfaceVolumeAnim = GameObject.Find("InterfaceVolume")?.GetComponent<Animator>();

        isPrintingText = false;

        if (PlayOnStart)
        {
            StartTextSequence();
        }
    }

    public void StartTextSequence()
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
        else if (PrintSuccess)
            StartCoroutine(PrintText(RundownSuccessText, true));
        else if (PrintFailure)
            StartCoroutine(PrintText(RundownFailedText, true));
    }

    public void PrintEndScreen()
    {
        if (!isPrintingText)
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

    public void PrintSuccessScreen()
    {
        if (!isPrintingText)
            StartCoroutine(PrintText(RundownSuccessText, false));
    }
    public void PrintFailureScreen()
    {
        if (!isPrintingText)
            StartCoroutine(PrintText(RundownFailedText, false));
    }

    IEnumerator PrintText(string[] screentext, bool FadeBool)
    {
        isPrintingText = true;
        if (PlayerInstance != null)
            PlayerInstance.PlayerConstrained = true;

        if (InterfaceVolumeAnim != null)
            InterfaceVolumeAnim.SetTrigger("DisplayVolume");

        textMeshPro.text = ""; // Ensure text is cleared before printing
        string fullText = "";

        foreach (string line in screentext)
        {
            fullText += line + "\n"; // Concatenate lines
        }

        foreach (char c in fullText)
        {
            textMeshPro.text += c;
            yield return new WaitForSeconds(letterDelay);
        }

        yield return new WaitForSeconds(linePauseDuration); // Pause after full text prints
        yield return new WaitForSeconds(textClearDelay); // Extra delay before transitioning

        textMeshPro.text = ""; // Clear text

        if (FadeBool)
            BlackScreenInstance.TriggerFadeOut();

        if (screentext == EndScreen)
        {
            if (SceneManager.GetActiveScene().name.Equals("S_LevelBlockout"))
                SceneManager.LoadScene("S_DayLoop");
        }

        if(screentext == RundownSuccessText)
        {
            if (SceneManager.GetActiveScene().name.Equals("S_DayLoop"))
                SceneManager.LoadScene("Menu");
        }
        if (screentext == RundownFailedText)
        {
            if (SceneManager.GetActiveScene().name.Equals("S_DayLoop"))
                SceneManager.LoadScene("S_DayLoop");
        }


        if (InterfaceVolumeAnim != null)
            InterfaceVolumeAnim.SetTrigger("HideVolume");

        PlayerInstance.PlayerConstrained = false;
        isPrintingText = false; // Mark as complete so transition can happen
    }

    public bool IsTextFinished()
    {
        return !isPrintingText;
    }
}
