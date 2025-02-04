using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Generators : MonoBehaviour
{

    public bool isCharging = false;
    float powerDecre;
    [SerializeField] float PowerIncrementMultiplier;
    [SerializeField] public float GeneratorPowerAmount;
    [SerializeField] public float GeneratorMaxAmount;
    [SerializeField] float PowerDecrement;

    [Header("--- VIARIABLES ---")]
    [SerializeField] public int GeneratorID;
    [Range(0.0f, 1.0f)]
    [SerializeField] float SonarBeamThreshold;

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

    void Start()
    {
        CLMInstance = CoreLoopManager.Instance;
    }

    void Update()
    {
        if (GeneratorPowerAmount >= GeneratorMaxAmount)
            GeneratorPowerAmount = GeneratorMaxAmount;
        else if (GeneratorPowerAmount <= 0)
        {
            GeneratorPowerAmount = 0;
        }

        powerDecre = isCharging ? PowerIncrementMultiplier : 0;
        GeneratorPowerAmount += Time.deltaTime * (-PowerDecrement + powerDecre);

        GeneratorThreshold();
        CoreLoopStages();



        if (DEBUG_UI)
            UI_Debug();
    }
    void GeneratorThreshold()
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
