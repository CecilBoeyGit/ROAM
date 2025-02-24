using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSockets : MonoBehaviour
{
    private bool _playerInZone;
    private bool socketEmpty = false;
    private bool socketSpawningPowerCore = false;
    [SerializeField] float socketSpawntime = 3.5f;
    float socketSpawntimer = 0;
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
    [SerializeField] public GameObject UI_Group;
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
        PowerCoreStatus();

        UIGroupManager();
        if (transform.gameObject.name.Contains("Power"))
            SocketTextIndicator();
    }
    void PowerCoreStatus()
    {
        if (socketSpawntimer >= socketSpawntime)
        {
            if (CO_SpawningPowerCore != null)
                StopCoroutine(CO_SpawningPowerCore);
            CO_SpawningPowerCore = StartCoroutine(PowerCoreSpawning(2.0f));
        }

        if (socketEmpty)
            socketSpawntimer += Time.deltaTime;
        else
            socketSpawntimer = 0;

    }
    Coroutine CO_SpawningPowerCore;
    IEnumerator PowerCoreSpawning(float duration)
    {
        socketSpawningPowerCore = true;
        socketEmpty = false;

        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;

            //Placeholder for animations and effects while the PowerCore is spawning

            yield return null;
        }

        powerCoreChild.gameObject.SetActive(true);
        powerCoreChild.BackToSocket();
        ads.clip = adcp[1];
        ads.Play();
        socketSpawningPowerCore = false;
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
    public virtual void UIGroupManager()
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
        if (socketSpawningPowerCore)
            return;

        if (_InputSub.InteractInput)
        {
            if (powerCoreChild.isActiveAndEnabled && PRMInstance.currentPowerCore == null)
            {
                if (!powerCoreChild.isEquiped && !powerCoreChild.isDropped)
                {
                    powerCoreChild.Equipped();
                    PRMInstance.currentPowerCore = powerCoreChild;
                    ads.clip = adcp[0];
                    ads.Play();
                    powerCoreChild.gameObject.SetActive(false);
                    socketEmpty = true;
                }
            }
            else if (!powerCoreChild.isActiveAndEnabled && PRMInstance.currentPowerCore != null)
            {
                if (PRMInstance.currentPowerCore.isEquiped)
                {
                    socketEmpty = false;
                    powerCoreChild.gameObject.SetActive(true);
                    powerCoreChild.BackToSocket();
                    ads.clip = adcp[1];
                    ads.Play();
                    PRMInstance.currentPowerCore = null;
                }
            }
        }   
    }

    bool CheckPlayerInZone()
    {
        //Conditions to check if the player is facing the socket or not
        float zoneThreshold = Vector3.Dot(playerForwardTransform.forward, -socketForwardTransform.forward);
        //Conditions to check if the player is within the distance of the socket or not
        float dist = Vector3.Distance(playerForwardTransform.position, socketForwardTransform.position);

        if (dist < distanceThreshold)
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
