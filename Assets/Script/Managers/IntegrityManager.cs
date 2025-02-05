using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class IntegrityManager : MonoBehaviour
{
    [Header("--- REFERENCES ---")]
    [SerializeField] Image UI_IntegrityMeter;
    [SerializeField] TextMeshProUGUI UI_CountDown;

    [Header("--- VARIABLES ---")]
    [SerializeField] float MeterAmount;
    [SerializeField] float MeterMax;
    [SerializeField] float MeterDecrementAnyMulti;
    float MeterDecreAnyHolder;
    [SerializeField] float MeterDecrementAllMulti;
    float MeterDecreAllHolder;
    [SerializeField] float MeterIncrementMulti;
    float MeterIncrementHolder;

    [Header("--- VARIABLES COUNTDOWN ---")]
    [SerializeField] float TimerMax;
    [SerializeField] float TimerDecrementMulti = 1.0f;

    [Header("--- VISUALS ---")]
    [SerializeField] Color MaxColor;
    [SerializeField] Color MinColor;

    [Header("--- DEBUG ---")]
    [SerializeField] bool usingDEBUG = false;

    public static event Action MeterNull;

    CoreLoopManager CLMInstance;
    ReloadAllScenes ReloadInstance;

    private void OnEnable()
    {
        GeneratorManager.GenAnyEmpty += MeterEmptyStage01;
        GeneratorManager.GenAllEmpty += MeterEmptyStage02;
    }
    private void OnDisable()
    {
        GeneratorManager.GenAnyEmpty -= MeterEmptyStage01;
        GeneratorManager.GenAllEmpty -= MeterEmptyStage02;
    }
    private void Start()
    {
        UI_IntegrityMeter.GetComponent<Image>();

        CLMInstance = CoreLoopManager.Instance;
        ReloadInstance = ReloadAllScenes.instance;

        if (CLMInstance.Enum_LoopStages == CoreLoopManager.LoopStages.Onboarding)
            TimerMax = 5940f;
        else
            TimerMax = 300f;
    }
    private void Update()
    {
        MeterDecrement();
        MeterAmountRemap();
        MeterNullActions();

        CountDownManager();
        CountDownNull();
    }
    void ConvertSeconds(float seconds, out int minutes, out int secondsRemain, out int microseconds)
    {
        minutes = (int)(seconds / 60);
        secondsRemain = (int)(seconds % 60);
        microseconds = (int)((seconds - Mathf.Floor(seconds)) * 100);
    }
    void CountDownManager()
    {
        if (TimerMax > 0)
            TimerMax -= Time.deltaTime * TimerDecrementMulti;
        else
            TimerMax = 0;

        int minutes, secondsRemain, microseconds;
        ConvertSeconds(TimerMax, out minutes, out secondsRemain, out microseconds);
        UI_CountDown.text = string.Format("{0:00}:{1:00}.{2:000}", minutes, secondsRemain, microseconds);
    }
    void CountDownNull()
    {
        if(TimerMax <= 0)
        {
            MeterNullActions();
        }
    }
    void MeterAmountRemap()
    {
        float meterRemap = Mathf.InverseLerp(0, MeterMax, MeterAmount);
        UI_IntegrityMeter.fillAmount = meterRemap;
        UI_IntegrityMeter.color = Color.Lerp(MinColor, MaxColor, meterRemap);

        if (MeterAmount >= MeterMax)
            MeterAmount = MeterMax;
        else if (MeterAmount <= 0)
            MeterAmount = 0;
    }
    void MeterDecrement()
    {
        MeterAmount += Time.deltaTime * (-(MeterDecreAnyHolder + MeterDecreAllHolder) + MeterIncrementHolder);
    }
    void MeterEmptyStage01(bool condi)
    {
        if (CLMInstance != null)
        {
            if (CLMInstance.Enum_LoopStages == CoreLoopManager.LoopStages.Onboarding)
                MeterDecrementAnyMulti = 0.5f;
        }

        MeterDecreAnyHolder = condi ? MeterDecrementAnyMulti : 0;
        MeterIncrementHolder = condi ? 0 : MeterIncrementMulti;
    }
    void MeterEmptyStage02(bool condi)
    {
        if (CLMInstance.Enum_LoopStages == CoreLoopManager.LoopStages.Onboarding)
            MeterDecrementAllMulti = 0f;

        MeterDecreAllHolder = condi ? MeterDecrementAllMulti : 0;
    }
    void MeterNullActions()
    {
        if (MeterAmount <= 0)
        {
            if (!usingDEBUG)
                MeterNull?.Invoke();
            else
                ReloadInstance.ReloadInvoke();
        }
    }
}
