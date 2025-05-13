using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CoreLoopManager : MonoBehaviour
{
    public enum LoopStages
    {
        Onboarding,
        DayOneCycle
    }
    public LoopStages Enum_LoopStages;

    public enum OnboardingStages
    {
        Intro,
        PowerCores,
        Generator01,
        ScanEnemy,
        KillEnemy,
        Generator02,
        Complete
    }
    public OnboardingStages Enum_OnboardingStages;

    public enum DayStages
    {
        IntegrityActivate,
        IntegritySuccess,
        DisplayInterfaceSuccess,
        DisplayInterfaceFailure,
        Fail
    }
    public DayStages Enum_DayStages;

    [Header("--- OVERRIDES ---")]
    [SerializeField] bool IsDayLoop = false;

    [Header("--- ONBOARDING ---")]
    [SerializeField] bool hasPickedUpPowerCore = false;

    [Header("--- REFERENCES ---")]
    public GameObject HullIntegrity;
    public GameObject IntegrityUI;
    public GameObject EscapeSignUI;
    [SerializeField] public int FirstTutorialGeneratorID, SecondTutorialGeneratorID;
    [SerializeField] GameObject objectToEnableAfterFirstPowerCore; // Assigned in Inspector
    GameObject elevatorTrigger;

    public static event Action SecondPowerCorePickedUp;
    public static event Action GeneratorCharged;

    bool PlayerPickedUpSecondPowerCore = false;
    bool PlayerInZoneForTutorialBot = false;
    bool Day_ActivateIntegrity = false;

    public bool RundownSuccessful { get; private set; }

    PlayerController pcInstance;
    TutorialSequence TutorialSeqInstance;
    LetterByLetterWithPause LetterFuncInstance;
    BlackScreenFadeOutScript BlackScreenInstance;
    IntegrityManager IntegrityInstance;

    public static CoreLoopManager Instance;

    private void OnEnable()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        // Subscribe to tutorial and power core events
        TutorialTrigger.TutorialTriggered += IntroToPowerCore;
        PowerReserveManager.PowerCorePickedUp += PowerCoresToGenerator01;
        Generators.PowerCorePlaced += Generator01ToScanEnemy;
        PowerReserveManager.PowerCorePickedUp += SecondPowerCoreTrigger;
        TutorialBotStage.PlayerInTrigger += TutorialBotTrigger;
        EnemyBehavior.TutorialBotKilled += TutorialBotKilled;
        Generators.Gen02Charging += Generator02ToComplete;
        IntegrityManager.RundownSuccessAction += RundownSuccess;

        InitializationParameters();
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        TutorialTrigger.TutorialTriggered -= IntroToPowerCore;
        PowerReserveManager.PowerCorePickedUp -= PowerCoresToGenerator01;
        Generators.PowerCorePlaced -= Generator01ToScanEnemy;
        PowerReserveManager.PowerCorePickedUp -= SecondPowerCoreTrigger;
        TutorialBotStage.PlayerInTrigger -= TutorialBotTrigger;
        EnemyBehavior.TutorialBotKilled -= TutorialBotKilled;
        Generators.Gen02Charging -= Generator02ToComplete;
        IntegrityManager.RundownSuccessAction -= RundownSuccess;
    }

    void InitializationParameters()
    {
        PlayerInZoneForTutorialBot = false;
        EscapeSignUI.SetActive(false);

        if (IsDayLoop)
        {
            Enum_LoopStages = LoopStages.DayOneCycle;
            Enum_DayStages = DayStages.IntegrityActivate;
            Day_ActivateIntegrity = false;
        }
        else
        {
            Enum_LoopStages = LoopStages.Onboarding;
            Enum_OnboardingStages = OnboardingStages.Intro;
            Enum_DayStages = DayStages.IntegrityActivate;
        }

        HullIntegrity.SetActive(false);
        IntegrityUI.SetActive(false);

        elevatorTrigger = FindObjectOfType<ElevatorManager>()?.gameObject;
        if (elevatorTrigger != null)
            elevatorTrigger.SetActive(false);

        if (objectToEnableAfterFirstPowerCore != null)
            objectToEnableAfterFirstPowerCore.SetActive(false);
    }

    void Start()
    {
        RundownSuccessful = false;

        pcInstance = PlayerController.instance;
        TutorialSeqInstance = TutorialSequence.Instance;
        LetterFuncInstance = LetterByLetterWithPause.Instance;
        BlackScreenInstance = BlackScreenFadeOutScript.Instance;
        IntegrityInstance = IntegrityManager.instance;
    }

    void RundownSuccess()
    {
        if (IsDayLoop)
            Enum_DayStages = DayStages.IntegritySuccess;
    }

    #region ------ Onboarding Stages ------
    void IntroToPowerCore()
    {
        if (Enum_LoopStages == LoopStages.Onboarding)
        {
            Enum_OnboardingStages = OnboardingStages.PowerCores;
        }
        else if (Enum_DayStages == DayStages.IntegrityActivate)
        {
            Day_ActivateIntegrity = true;
        }
    }

    void PowerCoresToGenerator01()
    {
        MetricManagerScript.instance?.LogString("1st PowerCore Picked Up", Time.time.ToString());
        if (Enum_OnboardingStages == OnboardingStages.PowerCores)
            Enum_OnboardingStages = OnboardingStages.Generator01;
    }

    void Generator01ToScanEnemy()
    {
        MetricManagerScript.instance?.LogString("1st Generator Charged", Time.time.ToString());

        // Unlock player abilities
        pcInstance.AbilitiesConstrained = false;

        // Activate the specified scene object set in the Inspector
        if (objectToEnableAfterFirstPowerCore != null)
            objectToEnableAfterFirstPowerCore.SetActive(true);

        // Switch to the enemy scanning/killing stage
        Enum_OnboardingStages = OnboardingStages.ScanEnemy;
    }

    void SecondPowerCoreTrigger()
    {
        if (Enum_OnboardingStages == OnboardingStages.ScanEnemy)
        {
            SecondPowerCorePickedUp?.Invoke();
            MetricManagerScript.instance?.LogString("2nd Power Core Picked Up", Time.time.ToString());
            PlayerPickedUpSecondPowerCore = true;
        }
    }

    void TutorialBotTrigger()
    {
        MetricManagerScript.instance?.LogString("First encounter triggered", Time.time.ToString());
        PlayerInZoneForTutorialBot = true;
        PlayerPickedUpSecondPowerCore = false;
    }

    void TutorialBotKilled()
    {
        MetricManagerScript.instance?.LogString("Enemy killed", Time.time.ToString());
        Enum_OnboardingStages = OnboardingStages.Generator02;
    }

    void Generator02ToComplete()
    {
        MetricManagerScript.instance?.LogString("2nd Generator Charged", Time.time.ToString());
        Enum_OnboardingStages = OnboardingStages.Complete;
        LetterFuncInstance.isPrintingText = false;
        BlackScreenInstance.TriggerFadeIn("End");
    }
    #endregion

    void Update()
    {
        switch (Enum_LoopStages)
        {
            case LoopStages.Onboarding:
                switch (Enum_OnboardingStages)
                {
                    case OnboardingStages.Intro: Onboarding_Intro(); break;
                    case OnboardingStages.PowerCores: Onboarding_PowerCores(); break;
                    case OnboardingStages.Generator01: Onboarding_Generator01(); break;
                    case OnboardingStages.ScanEnemy: Onboarding_ScanEnemy(); break;
                    case OnboardingStages.KillEnemy: break;
                    case OnboardingStages.Generator02: Onboarding_Generator02(); break;
                    case OnboardingStages.Complete: Onboarding_Complete(); break;
                }
                break;
            case LoopStages.DayOneCycle:
                switch (Enum_DayStages)
                {
                    case DayStages.IntegrityActivate:
                        if (TutorialSeqInstance == null) return;
                        DayRegularBehaviours();
                        break;
                    case DayStages.IntegritySuccess:
                        SuccessBehaviors();
                        break;
                    case DayStages.DisplayInterfaceSuccess:
                        DisplaySuccess();
                        break;
                    case DayStages.Fail:
                        FailedBehaviors();
                        break;
                    case DayStages.DisplayInterfaceFailure:
                        DisplayFailed();
                        break;
                }
                break;
        }
    }

    void Onboarding_Intro() { pcInstance.AbilitiesConstrained = true; }
    void Onboarding_PowerCores() { TutorialSeqInstance.PickUpPowerCore1(); HullIntegrity.SetActive(true); IntegrityUI.SetActive(true); }
    void Onboarding_Generator01() { TutorialSeqInstance.InsertPowerCore1_1(); }
    void Onboarding_ScanEnemy()
    {
        if (PlayerPickedUpSecondPowerCore)
            TutorialSeqInstance.PickUpPowerCore2_2();
        else if (PlayerInZoneForTutorialBot)
        {
            TutorialSeqInstance.FirstEncounter();
            pcInstance.AbilitiesConstrained = false;
        }
        else
            TutorialSeqInstance.PickUpPowerCore2();
    }
    void Onboarding_Generator02() { TutorialSeqInstance.FirstEncounter_2(); }
    void Onboarding_Complete() { TutorialSeqInstance.FinalCockpit(); }

    public void DayStagesDisplayFailure() { Enum_DayStages = DayStages.DisplayInterfaceFailure; }
    void DayRegularBehaviours() { if (Day_ActivateIntegrity) { HullIntegrity.SetActive(true); IntegrityUI.SetActive(true); TutorialSeqInstance.DayLoop(); } }
    void SuccessBehaviors() { RundownSuccessful = true; TutorialSeqInstance.ReturnToElevator(); if (elevatorTrigger != null) elevatorTrigger.SetActive(true);
        if (EscapeSignUI != null) EscapeSignUI.SetActive(true); }
    void DisplaySuccess() { if (!pcInstance.PlayerConstrained) { BlackScreenInstance.TriggerFadeIn("Success"); pcInstance.PlayerConstrained = true; } }
    void FailedBehaviors() { RundownSuccessful = false; }
    bool FailedDisplayed = false;
    void DisplayFailed()
    {
        if (FailedDisplayed) return;
        FailedDisplayed = true;
        BlackScreenInstance.TriggerFadeIn("Fail");
        print("TriggeredFadeIn ---");
    }
}
