using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PowerReserveManager : MonoBehaviour
{
    #region     --- VARIABLES ---
    public PowerCores currentPowerCore;
    GameObject PowerCoreGraphics;
    PlayerController playerInstance;

    [Header("MAIN AMOUNTS")]
    public float weaponAmount;
    public float weaponAmountMax;
    [SerializeField] float weaponAmountIncrement;
    [SerializeField] float weaponAmountStationaryMultiplier;
    [SerializeField] float weaponAmountPowerMultiplier;
    private float weaponMultiplierContainer;
    private float weaponStationaryMultiContainer;

    public bool isEquiped;

    [Header("DEPLETION VARIABLES")]
    public float scannerConsumption;
    public float pulseConsumption;
    public float runningConsumption;
    public float shootingConsumption;
    public float overdrawConsumption;

    bool weaponIncrementing = false;
    Coroutine CO_WeaponAmountIncrement;

    public bool isHeldDown = false;
    float timer = 0;

    [Header("DEPLETION VARIABLES")]
    public Material WeaponRing;
    public Material BulletRing;
    public Material ScanRing;
    public Material HPRing;
    public Material PowerRing;

    public bool isConsumingWeaponAmount = false;

    [SerializeField] GameObject IntegManager;

    bool secondCoreTriggered = false;

    public static event Action PowerCorePickedUp;

    CoreLoopManager CLMInstance;

    int AbilitiesEmptyTriggerMetric = 0;

    public static PowerReserveManager instance;
    #endregion

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        playerInstance = PlayerController.instance;
        currentPowerCore = null;
        PowerCoreGraphics = transform.Find("PowerCoreGraphics").gameObject;
        if (PowerCoreGraphics == null)
            Debug.LogError("No PowerCore Attached on Player!!!");

        PowerCoreGraphics.SetActive(false);

        CLMInstance = CoreLoopManager.Instance;
        
        AbilitiesEmptyTriggerMetric = 0;
    }
    void DEBUG_INVINCIBLE() 
    {
        if (Input.GetKey(KeyCode.P))
        {
            weaponAmount = 100;
        }
    }
    private void Update()
    {
        //Power Amount
        //PowerAmountOverdraw();

        PowerCoreStatus();

        //Weapon Amount
        WeaponMultiplierAdjustment();
        WeaponStationaryMulti();
        WeaponAmountIncrement();

        PowerCoreGraphics.SetActive(currentPowerCore == null? false : true);
        DEBUG_INVINCIBLE();

        CoreLoopStages();

        //Visualizations
        UIRingControls();
    }
    float WeaponMultiplierAdjustment()
    {
        return weaponMultiplierContainer = 1; //PLACEHOLDER VALUE
    }
    float WeaponStationaryMulti()
    {
        return weaponStationaryMultiContainer = playerInstance.velocity <= 0 ? weaponAmountStationaryMultiplier : 1;
    }
    void PowerCoreStatus()
    {
        isEquiped = currentPowerCore == null ? false : true;
    }
    void PowerAmountOverdraw() //Timer is implemented to prevent other functions with MouseButtons to trigger
    {
        if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
        {
            timer += Time.deltaTime;
            if (timer >= 0.1f)
                isHeldDown = true;
            else
                isHeldDown = false;
        }
        else
        {
            if(timer > 0)
            {
                if (timer >= 0.1f)
                {
                    timer = 0.1f;
                }

                timer -= Time.deltaTime;
            }
            else if (timer <= 0)
            {
                timer = 0;
                isHeldDown = false;
            }
        }
    }
    void WeaponAmountIncrement()
    {
        if (weaponAmountRecharging)
            return;

        if (weaponAmount >= weaponAmountMax)
            weaponAmount = weaponAmountMax;
        else if (weaponAmount < weaponAmountMax && !isConsumingWeaponAmount)
            weaponAmount += Time.deltaTime * weaponAmountIncrement * weaponStationaryMultiContainer * weaponMultiplierContainer;

        if (weaponAmount < 5.0f)
        {
            weaponAmount = 0;

            if (CO_WeaponAmountRecharging != null)
                StopCoroutine(CO_WeaponAmountRecharging);
            StartCoroutine(WeaponAmountRecharge(3.0f));

            if (IntegManager.activeInHierarchy)
            {
                if (CLMInstance.Enum_LoopStages == CoreLoopManager.LoopStages.DayOneCycle)
                {
                    AbilitiesEmptyTriggerMetric++;
                    string playerTransformString = ("World Position Trigger Point: " + transform.position + "Weapon Amount Null: " + AbilitiesEmptyTriggerMetric);
                    if (MetricManagerScript.instance != null)
                    {
                        MetricManagerScript.instance.LogString("Weapon Amount Empty", playerTransformString.ToString());
                    }
                }
            }
        }
    }

    bool weaponAmountRecharging = false;
    Coroutine CO_WeaponAmountRecharging;

    IEnumerator WeaponAmountRecharge(float duration)
    {
        weaponAmountRecharging = true;
        playerInstance.AbilitiesConstrained = true;

        float time = 0;
        while(time < duration)
        {
            time += Time.deltaTime;
            float lerpVal = Mathf.InverseLerp(0, duration, time);
            weaponAmount = lerpVal * weaponAmountMax;

            yield return null;
        }

        playerInstance.AbilitiesConstrained = false;
        weaponAmountRecharging = false;
    }

    void CoreLoopStages()
    {
        switch (CLMInstance.Enum_OnboardingStages)
        {
            case CoreLoopManager.OnboardingStages.PowerCores:
                if (currentPowerCore != null)
                {
                    TutorialElementPlayScript.isPlayingAudio = false;
                    PowerCorePickedUp?.Invoke();
                }
                break;
            case CoreLoopManager.OnboardingStages.ScanEnemy:
                if (currentPowerCore != null)
                {
                    if (!secondCoreTriggered && currentPowerCore.transform.parent.gameObject.name.Contains("PowerSocket"))
                    {
                        secondCoreTriggered = true;
                        TutorialElementPlayScript.isPlayingAudio = false;
                        PowerCorePickedUp?.Invoke();
                    }
                }
                break;
        }
    }
    void UIRingControls()
    {
        float WeaponAmountLerpVal = Mathf.InverseLerp(0, weaponAmountMax, weaponAmount);
        float BulletPercentile = Mathf.Floor(weaponAmountMax / shootingConsumption);
        float BulletLerpVal = Mathf.InverseLerp(0, BulletPercentile, Mathf.Floor(weaponAmount / shootingConsumption));
        float ScanPercentile = Mathf.Floor(weaponAmountMax / scannerConsumption);
        float ScanLerpVal = Mathf.InverseLerp(0, ScanPercentile, Mathf.Floor(weaponAmount / scannerConsumption));
        float HPPercentile = Mathf.InverseLerp(0, 100, playerInstance.healthPoint);

        WeaponRing.SetFloat("_SliceCoverage", WeaponAmountLerpVal);

        BulletRing.SetFloat("_SegmentCount", BulletPercentile);
        BulletRing.SetFloat("_SliceCoverage", BulletLerpVal);

        ScanRing.SetFloat("_SegmentCount", ScanPercentile);
        ScanRing.SetFloat("_SliceCoverage", ScanLerpVal);

        HPRing.SetFloat("_SliceCoverage", HPPercentile);
    }
}
