using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

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
    public bool RundownSuccess = false;

    [Header("--- VARIABLES COUNTDOWN ---")]
    [SerializeField] float CountdownTimer = 100;
    public float TimerInitial;
    public float TimerMax;
    [SerializeField] float TimerDecrementMulti = 1.0f;

    [Header("--- VISUALS ---")]
    [SerializeField] Color MaxColor;
    [SerializeField] Color MinColor;

    [Header("--- DEBUG ---")]
    [SerializeField] bool usingDEBUG = false;

    ObjectsPoolingDefault EnemiesPool;

    public static event Action MeterNull;
    public static event Action RundownSuccessAction;

    CoreLoopManager CLMInstance;
    ReloadAllScenes ReloadInstance;

    public static IntegrityManager instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance);
        else
            instance = this;
    }
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
        RundownSuccess = false;
        EnemiesPool = GameObject.Find("EnemiesPool")?.GetComponent<ObjectsPoolingDefault>();

        UI_IntegrityMeter.GetComponent<Image>();

        CLMInstance = CoreLoopManager.Instance;
        ReloadInstance = ReloadAllScenes.instance;

        if (CLMInstance.Enum_LoopStages == CoreLoopManager.LoopStages.Onboarding)
            TimerMax = 5940f;
        else
            TimerMax = CountdownTimer;

        TimerInitial = TimerMax;
    }
    private void Update()
    {
        if (RundownSuccess)
            return;

        MeterDecrement();
        MeterAmountRemap();
        MeterNullActions();

        CountDownManager();
        CountDownNull();
    }
    void ConvertSeconds(float seconds, out int minutes, out int secondsRemain)
    {
        minutes = (int)(seconds / 60);
        secondsRemain = (int)(seconds % 60);
        //microseconds = (int)((seconds - Mathf.Floor(seconds)) * 100);
    }
    void CountDownManager()
    {
        if (TimerMax > 0)
            TimerMax -= Time.deltaTime * TimerDecrementMulti;
        else
            TimerMax = 0;

        int minutes, secondsRemain;
        ConvertSeconds(TimerMax, out minutes, out secondsRemain);
        UI_CountDown.text = string.Format("{0:00}:{1:00}", minutes, secondsRemain);
    }
    void CountDownNull()
    {
        if(TimerMax <= 0 && MeterAmount > 0)
        {
            if (RundownSuccess)
                return;

            RundownSuccess = true;

            if (CO_RundownSuccessBuffer != null)
                StopCoroutine(CO_RundownSuccessBuffer);
            CO_RundownSuccessBuffer = StartCoroutine(RundownSuccessBuffer(1f));
        }
    }

    Coroutine CO_RundownSuccessBuffer;

    IEnumerator RundownSuccessBuffer(float duration)
    {
        RundownSuccessAction?.Invoke();

        var allRemainingEnemies = FindObjectsByType<EnemyBehavior>(FindObjectsSortMode.None)
            .Where(g => g.isActiveAndEnabled)
            .ToList();
        foreach(EnemyBehavior enemy in allRemainingEnemies)
        {
            enemy.enemyStateControl = EnemyBehavior.enemyStates.DeathState;
        }

        float time = 0;
        while(time < duration)
        {
            time += Time.deltaTime;
            float meterLerpVal = Mathf.Lerp(MeterAmount, MeterMax, time / duration);
            MeterAmount = meterLerpVal;
            float meterRemap = Mathf.InverseLerp(0, MeterMax, MeterAmount);
            UI_IntegrityMeter.fillAmount = meterRemap;
            UI_IntegrityMeter.color = MaxColor;
            yield return null;
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
        if (MeterAmount <= 0 && TimerMax > 0)
        {
            if (!usingDEBUG)
                MeterNull?.Invoke();
            else
                PlayerController.instance.HealthNullAction(true);
        }
    }
}
