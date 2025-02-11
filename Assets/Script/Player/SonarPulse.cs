using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SonarPulse : MonoBehaviour
{
    [Header("References")]
    [SerializeField] FieldOfViewCone fovConeController;
    [SerializeField] float pulseConsumption;
    [SerializeField] float pulseLengthMax;
    [SerializeField] float pulseSpoolTime;
    [SerializeField] float pulseTriggerTime;
    [SerializeField] Image spoolBar;

    [SerializeField] bool spooled = false;
    [SerializeField] bool isSpooling = false;
    [SerializeField] bool isScanning = false;

    [Header("Aduio")]
    AudioSource ads;
    [SerializeField] private AudioClip[] adcp = new AudioClip[2];
    bool isPlayingSFX = false;

    Coroutine CO_Spooling;
    Coroutine CO_Pulse;

    int SonarPulseTriggeredMetric = 0;

    PlayerController playerInstance;
    PowerReserveManager PRMInstance;

    private void Start()
    {
        fovConeController = GetComponent<FieldOfViewCone>();

        fovConeController.viewRadius = 0;
        if (spoolBar != null)
        {
            spoolBar.GetComponent<Image>();
            spoolBar.fillAmount = 0;
        }

        ads = GetComponent<AudioSource>();

        playerInstance = PlayerController.instance;
        PRMInstance = PowerReserveManager.instance;
        
        SonarPulseTriggeredMetric = 0;
    }
    private void Update()
    {
/*        pulseConsumption = PRMInstance.pulseConsumption;
        if (!playerInstance.PlayerConstrained)
        {
            if(!playerInstance.AbilitiesConstrained)
                SonarPulseControls();
        }*/
    }

    void SonarPulseControlsDEPRE()
    {
        if (Input.GetMouseButton(1) && PRMInstance.weaponAmount >= pulseConsumption && !PRMInstance.isHeldDown)
        {
            if (!isSpooling && !isScanning)
            {
                if (CO_Spooling != null)
                    StopCoroutine(CO_Spooling);

                CO_Spooling = StartCoroutine(PulseSpooling(pulseSpoolTime));
            }
        }
        else if (Input.GetMouseButtonUp(1) || PRMInstance.isHeldDown)
        {
            isSpooling = false;

            if (spooled)
            {
                if (CO_Pulse != null)
                    StopCoroutine(CO_Pulse);

                //CO_Pulse = StartCoroutine(PulseTrigger(pulseTriggerTime));
            }
            else
            {
                if (CO_Spooling != null)
                    StopCoroutine(CO_Spooling);
                spoolBar.fillAmount = 0;
            }
        }
    }
    void SonarPulseControls()
    {
        if (Input.GetMouseButton(1) && PRMInstance.weaponAmount >= pulseConsumption && !PRMInstance.isHeldDown)
        {
            if(!spooled)
            {
                if (CO_Pulse != null)
                    StopCoroutine(CO_Pulse);

                CO_Pulse = StartCoroutine(PulseTrigger(pulseTriggerTime, PulseTrigger_ExitCondition));

                SonarPulseTriggeredMetric++;
                string playerTransformString = ("World Position Trigger Point: " + transform.position + "Sonar Pulse: " + SonarPulseTriggeredMetric);
                if (MetricManagerScript.instance != null)
                {
                    MetricManagerScript.instance.LogString("Sonar Pulse Triggered", playerTransformString.ToString());
                }
            }
        }
        else if(Input.GetMouseButtonUp(1) || PRMInstance.weaponAmount < pulseConsumption || PRMInstance.isHeldDown)
        {
            spooled = false;
        }
    }
    IEnumerator PulseSpooling(float time)
    {
        isSpooling = true;
        float timer = 0;
        while(timer < time)
        {
            if (timer >= 0.1f)
                playerInstance.buttonHeldDown = true;
            else
                playerInstance.buttonHeldDown = false;
            timer += Time.deltaTime;
            float lerpVal = Mathf.InverseLerp(0, time, timer);
            spoolBar.fillAmount = lerpVal;
            yield return null;
        }
        ads.clip = adcp[0];
        ads.Play();
        spooled = true;
    }
    IEnumerator PulseTrigger(float time, Func<bool> ExitCond)
    {
        spooled = true;
        if(spoolBar != null)
            spoolBar.fillAmount = 0;
        playerInstance.movementMasterControl = 0.3f; //Limits the sensitivity of movements for the player when firing the sonar pulse

        ads.clip = adcp[1];
        ads.Play();

        float timer = 0;
        while (ExitCond.Invoke())
        {
            if(timer < time)
                timer += Time.deltaTime;
            if (timer >= 0.15f)
            {
                playerInstance.buttonHeldDown = true;
                playerInstance.vCamNoise.m_FrequencyGain = 2;
                //print(playerInstance.buttonHeldDown);
                PRMInstance.weaponAmount -= pulseConsumption * Time.deltaTime;
                float lerpVal = Mathf.InverseLerp(0, time, timer);
                fovConeController.viewRadius = Mathf.Lerp(0, pulseLengthMax, lerpVal);
            }
            else
            {
                playerInstance.buttonHeldDown = false;
            }

            yield return null;

        }
        playerInstance.buttonHeldDown = false;
        playerInstance.movementMasterControl = 1.0f;
        playerInstance.vCamNoise.m_FrequencyGain = 0;

        fovConeController.viewRadius = 0;
    }
    bool PulseTrigger_ExitCondition()
    {
        return spooled;
    }
}
