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
    [SerializeField] GameObject HullIntegrity, IntegrityUI; //GenIndicatorGroup;
    [SerializeField] GameObject Callisto01, Callisto02, Callisto03;
    //[SerializeField] GameObject GenIndicator01, GenIndicator02, GenIndicator03;
    [SerializeField] public int FirstTutorialGeneratorID, SecondTutorialGeneratorID;
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

            //GenIndicator01.SetActive(false); GenIndicator02.SetActive(false); GenIndicator03.SetActive(false);
        }

        HullIntegrity.SetActive(false);
        IntegrityUI.SetActive(false);
        //GenIndicatorGroup.SetActive(false);

        if (Callisto01 != null)
        {
            Callisto01.SetActive(false);
            Callisto02.SetActive(false);
            Callisto03.SetActive(false);
        }

        elevatorTrigger = FindObjectOfType<ElevatorManager>()?.gameObject;
        if (elevatorTrigger != null)
            elevatorTrigger.SetActive(false);
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
        {
            Enum_DayStages = DayStages.IntegritySuccess;
        }
    }

    #region ------ OnBoarding Stages ------
    void IntroToPowerCore()
    {
        if (Enum_LoopStages == LoopStages.Onboarding)
        {
            Enum_OnboardingStages = OnboardingStages.PowerCores;
            if (Callisto01 != null)
                Callisto01.SetActive(true);
        }
        else if (Enum_DayStages == DayStages.IntegrityActivate)
        {
            Day_ActivateIntegrity = true;
        }
    }
    void PowerCoresToGenerator01()
    {
        if (MetricManagerScript.instance != null)
        {
            MetricManagerScript.instance.LogString("1st PowerCore Picked Up", Time.time.ToString());
        }
        if (Enum_OnboardingStages == OnboardingStages.PowerCores)
            Enum_OnboardingStages = OnboardingStages.Generator01;
    }
    void Generator01ToScanEnemy()
    {
        if (MetricManagerScript.instance != null)
        {
            MetricManagerScript.instance.LogString("1st Generator Charged", Time.time.ToString());
        }

        pcInstance.AbilitiesConstrained = false;

        Enum_OnboardingStages = OnboardingStages.Complete;
        Callisto02.SetActive(true);
    }
    void SecondPowerCoreTrigger()
    {
        if (Enum_OnboardingStages == OnboardingStages.ScanEnemy)
        {
            SecondPowerCorePickedUp?.Invoke();

            if (MetricManagerScript.instance != null)
            {
                MetricManagerScript.instance.LogString("2nd Power Core Picked Up", Time.time.ToString());
            }
            PlayerPickedUpSecondPowerCore = true;
        }
    }
    void TutorialBotTrigger()
    {
        if (MetricManagerScript.instance != null)
        {
            MetricManagerScript.instance.LogString("1st Encounter Triggered", Time.time.ToString());
        }
        PlayerInZoneForTutorialBot = true;
        PlayerPickedUpSecondPowerCore = false;
        Callisto03.SetActive(true);
    }
    void TutorialBotKilled()
    {
        if (MetricManagerScript.instance != null)
        {
            MetricManagerScript.instance.LogString("Encounter Killed", Time.time.ToString());
        }
        Enum_OnboardingStages = OnboardingStages.Generator02;
    }
    void Generator02ToComplete()
    {
        if (MetricManagerScript.instance != null)
        {
            MetricManagerScript.instance.LogString("2nd Generator Charged", Time.time.ToString());
        }
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
                    case OnboardingStages.Intro:
                        Onboarding_Intro();
                        break;
                    case OnboardingStages.PowerCores:
                        Onboarding_PowerCores();
                        break;
                    case OnboardingStages.Generator01:
                        Onboarding_Generator01();
                        break;
                    case OnboardingStages.ScanEnemy:
                        Onboarding_ScanEnemy();
                        break;
                    case OnboardingStages.KillEnemy:
                        //Skipped for lesser instructions. ADD LATER!!
                        break;
                    case OnboardingStages.Generator02:
                        Onboarding_Generator02();
                        break;
                    case OnboardingStages.Complete:
                        Onboarding_Complete();
                        break;
                }
                break;
            case LoopStages.DayOneCycle:
                switch(Enum_DayStages)
                {
                    case DayStages.IntegrityActivate:
                        if (TutorialSeqInstance == null)
                            return;

                        DayRegularBehaviours();
                        break;
                    case DayStages.IntegritySuccess:
                        SuccessBehaviors();
                        break;
                    case DayStages.DisplayInterfaceSuccess:
                        DisplaySuccess();
                        break;
                    case DayStages.Fail:
                        Failedbehaviors();
                        break;
                    case DayStages.DisplayInterfaceFailure:
                        DisplayFailed();
                        break;
                }
                break;
        }
    }
    public void DayStagesDisplayFailure()
    {
        Enum_DayStages = DayStages.DisplayInterfaceFailure;
    }
    void DayRegularBehaviours()
    {
        if(Day_ActivateIntegrity)
        {
            HullIntegrity.SetActive(true);
            IntegrityUI.SetActive(true);
            //GenIndicatorGroup.SetActive(true); GenIndicator02.SetActive(false);
            TutorialSeqInstance.DayLoop();
        }
    }
    void SuccessBehaviors()
    {
        RundownSuccessful = true;
        TutorialSeqInstance.ReturnToElevator(); //Objective text set to Return To Elevator
        if (elevatorTrigger != null)
            elevatorTrigger.SetActive(true);
    }
    void DisplaySuccess()
    {
        if (pcInstance.PlayerConstrained) //Ensures this method only runs once after called
            return;

        BlackScreenInstance.TriggerFadeIn("Success"); //Display the rundown success interface
        pcInstance.PlayerConstrained = true;
    }
    void Failedbehaviors()
    {
        RundownSuccessful = false;
    }
    void DisplayFailed()
    {
        if (pcInstance.PlayerConstrained) //Ensures this method only runs once after called
            return;

        BlackScreenInstance.TriggerFadeIn("Fail"); //Display the rundown failure interface
        print("TriggeredFadeIn ---");
        pcInstance.PlayerConstrained = true;
    }

    // --- ONBOARDING ------------------------

    void Onboarding_Intro()
    {
        pcInstance.AbilitiesConstrained = true;
    }
    void Onboarding_PowerCores()
    {
        TutorialSeqInstance.PickUpPowerCore1();
        HullIntegrity.SetActive(true);
        IntegrityUI.SetActive(true);
        //GenIndicatorGroup.SetActive(true);
    }
    void Onboarding_Generator01()
    {
        TutorialSeqInstance.InsertPowerCore1_1();
        //GenIndicator01.SetActive(true);
    }
    void Onboarding_ScanEnemy()
    {
        if (PlayerPickedUpSecondPowerCore)
        {
            TutorialSeqInstance.PickUpPowerCore2_2();
            //GenIndicator03.SetActive(true);
        }
        else
        {
            if (PlayerInZoneForTutorialBot)
            {
                TutorialSeqInstance.FirstEncounter();
                pcInstance.AbilitiesConstrained = false;
            }
            else
            {
                TutorialSeqInstance.PickUpPowerCore2();
            }
        }
    }
    void Onboarding_Generator02()
    {
        TutorialSeqInstance.FirstEncounter_2();
    }
    void Onboarding_Complete()
    {
        TutorialSeqInstance.FinalCockpit();
    }
}
