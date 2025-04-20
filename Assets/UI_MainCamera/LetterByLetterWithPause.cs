using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LetterByLetterWithPause : MonoBehaviour
{
    [Header("Text References")]
    public TMP_Text textMeshPro;
    public string[] OpeningCut;
    public string[] OpeningButtonText;
    public string[] EndScreen;
    public string[] OnboardingDay;
    public string[] Day01;
    public string[] RundownSuccessText;
    public string[] RundownFailedText;

    [Header("Timing")]
    public float letterDelay = 0.1f;
    public float linePauseDuration = 1f;
    public float textClearDelay = 2f;

    [Header("Play Settings")]
    [SerializeField] bool PlayOnStart = false;
    public bool isPrintingText = false;

    [Header("Which To Print")]
    [SerializeField] bool OpeningCutScene = false;
    [SerializeField] bool OpeningButton = false;
    [SerializeField] bool PrintEnd = false;
    [SerializeField] bool PrintOnboarding = false;
    [SerializeField] bool PrintDay01 = false;
    public bool PrintSuccess = false;
    public bool PrintFailure = false;

    [Header("Success Activation")]
    [Tooltip("Object to activate while success text is printing")]
    public GameObject midSuccessActivationObject;
    [Tooltip("Object to activate after success text disappears")]
    public GameObject successActivationObject;

    [Header("Failure Activation")]
    [Tooltip("Object to activate while failure text is printing")]
    public GameObject midFailActivationObject;
    [Tooltip("Object to activate after failure text disappears")]
    public GameObject failActivationObject;

#if UNITY_EDITOR
    [Header("Editor Testing")]
    [Tooltip("Tick in Play Mode to trigger success text & activation")]
    public bool testSuccess;
    [Tooltip("Tick in Play Mode to trigger failure text")]
    public bool testFailure;
#endif

    Animator InterfaceVolumeAnim;
    BlackScreenFadeOutScript BlackScreenInstance;
    PlayerController PlayerInstance;

    public static LetterByLetterWithPause Instance;

    private void OnEnable()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        // Cache references
        BlackScreenInstance = BlackScreenFadeOutScript.Instance;
        PlayerInstance = PlayerController.instance;
        InterfaceVolumeAnim = GameObject.Find("InterfaceVolume")?.GetComponent<Animator>();
        isPrintingText = false;

        if (PlayOnStart) StartTextSequence();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (testSuccess)
        {
            testSuccess = false;
            TriggerSuccessSequence();
        }
        if (testFailure)
        {
            testFailure = false;
            TriggerFailureSequence();
        }
#endif
    }

    public void StartTextSequence()
    {
        if (OpeningCutScene) StartCoroutine(PrintText(OpeningCut, false));
        else if (OpeningButton) StartCoroutine(PrintText(OpeningButtonText, false));
        else if (PrintEnd) StartCoroutine(PrintText(EndScreen, false));
        else if (PrintOnboarding) StartCoroutine(PrintText(OnboardingDay, true));
        else if (PrintDay01) StartCoroutine(PrintText(Day01, true));
        else if (PrintSuccess) TriggerSuccessSequence();
        else if (PrintFailure) TriggerFailureSequence();
    }

    private void TriggerSuccessSequence()
    {
        if (!isPrintingText && BlackScreenInstance != null)
            BlackScreenInstance.TriggerFadeIn("Success");
    }

    private void TriggerFailureSequence()
    {
        if (!isPrintingText && BlackScreenInstance != null)
            BlackScreenInstance.TriggerFadeIn("Fail");
    }

    // These are called by the fader after fade‐in
    public void PrintEndScreen()
    {
        if (!isPrintingText) StartCoroutine(PrintText(EndScreen, false));
    }

    public void PrintOnBoarding()
    {
        if (!isPrintingText) StartCoroutine(PrintText(OnboardingDay, true));
    }

    public void PrintDayOne()
    {
        if (!isPrintingText) StartCoroutine(PrintText(Day01, true));
    }

    public void PrintSuccessScreen()
    {
        if (!isPrintingText) StartCoroutine(PrintText(RundownSuccessText, false));
    }

    public void PrintFailureScreen()
    {
        if (!isPrintingText) StartCoroutine(PrintText(RundownFailedText, false));
    }

    IEnumerator PrintText(string[] screentext, bool FadeBool)
    {
        isPrintingText = true;
        if (PlayerInstance != null)
            PlayerInstance.PlayerConstrained = true;

        if (InterfaceVolumeAnim != null)
            InterfaceVolumeAnim.SetTrigger("DisplayVolume");

        // --- mid‐sequence activations ---
        if (screentext == RundownSuccessText && midSuccessActivationObject != null)
            midSuccessActivationObject.SetActive(true);
        if (screentext == RundownFailedText && midFailActivationObject != null)
            midFailActivationObject.SetActive(true);

        // Build & print letter by letter
        textMeshPro.text = "";
        string fullText = string.Join("\n", screentext) + "\n";
        foreach (char c in fullText)
        {
            textMeshPro.text += c;
            yield return new WaitForSeconds(letterDelay);
        }

        // Pause then clear
        yield return new WaitForSeconds(linePauseDuration);
        yield return new WaitForSeconds(textClearDelay);
        textMeshPro.text = "";

        // Deactivate mid‐sequence indicators
        if (screentext == RundownSuccessText && midSuccessActivationObject != null)
            midSuccessActivationObject.SetActive(false);
        if (screentext == RundownFailedText && midFailActivationObject != null)
            midFailActivationObject.SetActive(false);

        // --- final activations ---
        if (screentext == RundownSuccessText && successActivationObject != null)
            successActivationObject.SetActive(true);
        if (screentext == RundownFailedText && failActivationObject != null)
            failActivationObject.SetActive(true);

        // Fade out if requested
        if (FadeBool && BlackScreenInstance != null)
            BlackScreenInstance.TriggerFadeOut();

        // Scene transitions
        if (screentext == EndScreen &&
            SceneManager.GetActiveScene().name.Equals("S_LevelBlockout"))
        {
            SceneManager.LoadScene("S_DayLoop");
        }

       

        if (InterfaceVolumeAnim != null)
            InterfaceVolumeAnim.SetTrigger("HideVolume");

        if (PlayerInstance != null)
            PlayerInstance.PlayerConstrained = false;

        isPrintingText = false;
    }

    public bool IsTextFinished()
    {
        return !isPrintingText;
    }
}
