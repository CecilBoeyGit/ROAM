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

    [Header("--- Audio ---")]
    AudioSource ads;
    [SerializeField] AudioSource alarm_ads;
    [SerializeField] AudioSource voiceLine_ads;
    [SerializeField] private List<AudioClip> adcp = new List<AudioClip>();
    [SerializeField] private AudioClip RundownCompleteVoiceLine;

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
        alarmPlaying = false;
        EnemiesPool = GameObject.Find("EnemiesPool")?.GetComponent<ObjectsPoolingDefault>();

        ads = GetComponent<AudioSource>();
        alarm_ads.GetComponent<AudioSource>();
        voiceLine_ads.GetComponent<AudioSource>();

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
        if (RundownSuccess || meterNullTriggered)
            return;

        MeterDecrement();
        MeterAmountRemap();
        MeterNullActions();

        CountDownManager();
        CountDownNull();

        MeterAlarmWarningThreshold();
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

    public void WipeAllEnemies()
    {
        var allRemainingEnemies = FindObjectsByType<EnemyBehavior>(FindObjectsSortMode.None)
        .Where(g => g.isActiveAndEnabled)
        .ToList();

        foreach (EnemyBehavior enemy in allRemainingEnemies)
        {
            enemy.enemyStateControl = EnemyBehavior.enemyStates.DeathState;
        }
    }

    Coroutine CO_RundownSuccessBuffer;

    IEnumerator RundownSuccessBuffer(float duration)
    {
        RundownSuccessAction?.Invoke();

        ads.clip = adcp[0];
        ads.Play();

        alarm_ads.clip = adcp[2]; //Temporarily using alarm AudioSource to play a second SFX when rundown is successful
        alarm_ads.Play();

        voiceLine_ads.clip = RundownCompleteVoiceLine;
        voiceLine_ads.Play();

        WipeAllEnemies();

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

    [SerializeField] float alarmThreshold = 0.15f;
    bool alarmPlaying = false;

    void MeterAlarmWarningThreshold()
    {
        float thresholdCalc = TimerInitial * alarmThreshold;
        if(MeterAmount <= thresholdCalc)
        {
            if (alarmPlaying)
                return;

            if (MeterDecreAnyHolder != 0 || MeterDecreAllHolder != 0)
            {
                alarm_ads.clip = adcp[2];
                alarm_ads.Play();

                alarmPlaying = true;
            }
        }
        else
        {
            if (!alarmPlaying)
                return;

            alarm_ads.Stop();
            alarmPlaying = false;
        }
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

    bool meterNullTriggered = false;

    void MeterNullActions()
    {
        //To ensure this function only gets called ONCE
        if (meterNullTriggered)
            return;

        if (MeterAmount <= 0 && TimerMax > 0)
        {
            if (!usingDEBUG)
            {
                MeterNull?.Invoke();
                CLMInstance.IntegrityUI.SetActive(false);
                meterNullTriggered = true;

                alarm_ads.Stop();

                ads.clip = adcp[1];
                ads.Play();
            }
            else
                PlayerController.instance.HealthNullAction(true);
        }
    }
}
