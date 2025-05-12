using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Generators : MonoBehaviour
{

    public bool isCharging = false;
    public float PowerIncrementMultiplier = 100;
    public float GeneratorPowerAmount;
    public float GeneratorMaxAmount;
    [SerializeField] float PowerDecrement;

    [Header("--- VIARIABLES ---")]
    public int GeneratorID;
    [Range(0.0f, 1.0f)]
    [SerializeField] float SonarBeamThreshold;
    bool rundownSuccess = false;
    bool rundownFailed = false;
    bool canStartPowerDecre = false;

    [Header("--- UI ---")]
    [SerializeField] Slider UI_GeneratorPower;
    [SerializeField] Slider UI_generatorAmountScreen;

    [Header("--- DEBUG ---")]
    [SerializeField] bool DEBUG_UI = false;

    public static event Action<int, bool> PowerAmountThreshold;
    bool actionThresholdTriggered = false;

    public static event Action PowerCorePlaced;
    public static event Action Gen02Charging;

    CoreLoopManager CLMInstance;
    GeneratorsSFX genSFXInstance;

    private void OnEnable()
    {
        TutorialTrigger.TutorialTriggered += CanStartPowerDecre;

        IntegrityManager.RundownSuccessAction += RundownSuccess;
        IntegrityManager.MeterNull += RundownFailed;
    }
    private void OnDisable()
    {
        TutorialTrigger.TutorialTriggered -= CanStartPowerDecre;

        IntegrityManager.RundownSuccessAction -= RundownSuccess;
        IntegrityManager.MeterNull -= RundownFailed;
    }

    void CanStartPowerDecre()
    {
        canStartPowerDecre = true;
    }

    void Start()
    {
        rundownSuccess = false;
        rundownFailed = false;
        canStartPowerDecre = false;
        PowerZeroCapped = false;

        CLMInstance = CoreLoopManager.Instance;
        genSFXInstance = GeneratorsSFX.instance;
    }

    bool PowerZeroCapped = false;

    void Update()
    {
        if (rundownSuccess || rundownFailed)
            return;

        if (!canStartPowerDecre)
            return;

        if (GeneratorPowerAmount >= GeneratorMaxAmount)
            GeneratorPowerAmount = GeneratorMaxAmount;
        else if (GeneratorPowerAmount <= 0)
        {
            if (PowerZeroCapped)
                return;

            GeneratorPowerAmount = 0;
            if (genSFXInstance != null)
                genSFXInstance.PlayOffAudio();

            PowerZeroCapped = true;
        }
        else
        {
            PowerZeroCapped = false;
        }

        if (!isCharging)
            GeneratorPowerAmount += Time.deltaTime * -PowerDecrement;

        GeneratorThreshold();
        CoreLoopStages();



        if (DEBUG_UI)
            UI_Debug();
    }
    void RundownSuccess()
    {
        rundownSuccess = true;
        GeneratorPowerAmount = GeneratorMaxAmount;
    }
    void RundownFailed()
    {
        rundownFailed = true;
        GeneratorPowerAmount = 0;
    }
    void GeneratorThreshold() //Triggering the Defensive Sonar Towers
    {
        float percentile = Mathf.Lerp(0, GeneratorMaxAmount, SonarBeamThreshold);
        if (GeneratorPowerAmount >= percentile)
        {
            if (!actionThresholdTriggered)
            {
                PowerAmountThreshold?.Invoke(GeneratorID, true);
                actionThresholdTriggered = !actionThresholdTriggered;
            }
        }
        else
        {
            if (actionThresholdTriggered)
            {
                PowerAmountThreshold?.Invoke(GeneratorID, false);
                actionThresholdTriggered = !actionThresholdTriggered;
            }
        }

    }
    void CoreLoopStages()
    {
        switch(CLMInstance.Enum_OnboardingStages)
        {
            case CoreLoopManager.OnboardingStages.Generator01:
                if(GeneratorID == CLMInstance.FirstTutorialGeneratorID)
                {
                    if(isCharging)
                    {
                        TutorialElementPlayScript.isPlayingAudio = false;
                        PowerCorePlaced?.Invoke();
                    }
                }
                break;
            case CoreLoopManager.OnboardingStages.Generator02:
                if (GeneratorID == CLMInstance.SecondTutorialGeneratorID)
                {
                    if (isCharging)
                    {
                        TutorialElementPlayScript.isPlayingAudio = false;
                        Gen02Charging?.Invoke();
                    }
                }
                break;
        }
    }
    void UI_Debug()
    {
        UI_GeneratorPower.GetComponent<Slider>();
        float hpLerpVal = Mathf.InverseLerp(0, GeneratorMaxAmount, GeneratorPowerAmount);
        UI_GeneratorPower.value = hpLerpVal;
        UI_generatorAmountScreen.GetComponent<Slider>();
        UI_generatorAmountScreen.value = hpLerpVal;
    }
}
