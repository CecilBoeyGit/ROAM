using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSockets : MonoBehaviour
{
    private bool _playerInZone;
    public bool PlayerInZone
    {
        get { 
            return _playerInZone; 
        }
        set {
            _playerInZone = value;
        }
    }

    [SerializeField] float distanceThreshold;
    [SerializeField] float playerInZoneThreshold;

    [Header("REFERENCES")]
    [SerializeField] Transform socketForwardTransform;
    Transform playerForwardTransform;
    [SerializeField] protected PowerCores powerCoreChild;

    [Header("Aduio")]
    protected AudioSource ads;
    [SerializeField] protected AudioClip[] adcp = new AudioClip[2];
    bool isPlayingSFX = false;

    [Header("--- UI ---")]
    [SerializeField] GameObject UI_Group;
    [SerializeField] float offsetY; [SerializeField] float offsetX; [SerializeField] float offsetZ;
    [SerializeField] GameObject UI_Empty; [SerializeField] GameObject UI_Charging;

    protected PowerReserveManager PRMInstance;
    protected PlayerController playerInstance;
    protected InputSubscriptions _InputSub;

    enum SocketType
    {
        Engine,
        Generator
    }

    SocketType SType;

    protected virtual void Start()
    {
        if (socketForwardTransform == null)
            Debug.LogError("--- NO CHILD WITH ZONE LOCATED ---");

        CheckPowerCoreChild();

        ads = GetComponent<AudioSource>();

        //UI_RectTransform = UI_Group.GetComponent<RectTransform>();
        UI_Group.SetActive(false);
        if (transform.gameObject.name.Contains("Power"))
        {
            UI_Empty.SetActive(false);
            UI_Charging.SetActive(false);
        }

        PRMInstance = PowerReserveManager.instance;
        playerInstance = PlayerController.instance;
        _InputSub = InputSubscriptions.instance;
    }
    protected virtual void Update()
    {
        playerForwardTransform = playerInstance.transform;
        PlayerInZone = CheckPlayerInZone();
        if (_playerInZone)
            PlayerInZoneActions();
        UIGroupManager();
        if (transform.gameObject.name.Contains("Power"))
            SocketTextIndicator();
    }
    protected void SocketTextIndicator()
    {
        if (powerCoreChild.isActiveAndEnabled)
        {
            UI_Empty.SetActive(false);
            UI_Charging.SetActive(true);
        }
        else
        {
            UI_Empty.SetActive(true);
            UI_Charging.SetActive(false);
        }
    }
    void UIGroupManager()
    {
        if(_playerInZone)
        {
            UI_Group.SetActive(true);
            UI_Group.transform.position = transform.position + new Vector3(offsetX, offsetY, offsetZ);
        }
        else
            UI_Group.SetActive(false);
    }
    void CheckPowerCoreChild()
    {
        powerCoreChild = transform.Find("PowerCore")?.GetComponent<PowerCores>();
    }
    public virtual void PlayerInZoneActions()
    {
        if (_InputSub.InteractInput)
        {
            if (powerCoreChild.isActiveAndEnabled && PRMInstance.currentPowerCore == null)
            {
                if (!powerCoreChild.isEquiped && !powerCoreChild.isDropped && !powerCoreChild.isCharging)
                {
                    PRMInstance.currentPowerCore = powerCoreChild;
                    PRMInstance.powerAmount = powerCoreChild._powerAmount;
                    powerCoreChild.Equipped();
                    ads.clip = adcp[0];
                    ads.Play();
                    powerCoreChild.gameObject.SetActive(false);
                }
            }
            else if (!powerCoreChild.isActiveAndEnabled && PRMInstance.currentPowerCore != null)
            {
                if (PRMInstance.currentPowerCore != null && PRMInstance.currentPowerCore.isEquiped && !PRMInstance.currentPowerCore.isDropped)
                {
                    powerCoreChild.gameObject.SetActive(true);
                    powerCoreChild.BackToSocket();
                    ads.clip = adcp[1];
                    ads.Play();
                    powerCoreChild._powerAmount = PRMInstance.powerAmount;
                    PRMInstance.currentPowerCore = null;
                }
            }
        }   
    }
    bool CheckPlayerInZone()
    {
        float zoneThreshold = Vector3.Dot(playerForwardTransform.forward, -socketForwardTransform.forward);

        float dist = Vector3.Distance(playerForwardTransform.position, socketForwardTransform.position);

        if (dist < distanceThreshold && zoneThreshold < -playerInZoneThreshold || dist < distanceThreshold && zoneThreshold > playerInZoneThreshold)
        {
            //print("Threshold: " + zoneThreshold + " Distance: " + dist + " InZone: " + _playerInZone);
            return true;
        }
        else
            return false;
    }
    private void OnDrawGizmos()
    {
        Debug.DrawLine(socketForwardTransform.position, socketForwardTransform.position + -socketForwardTransform.forward * distanceThreshold, Color.yellow);
    }
}
