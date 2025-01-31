using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[RequireComponent(typeof(PowerCores))]
public class PowerCores : MonoBehaviour, IPlayerInterface
{
    [Header("MAIN AMOUNTS")]
    public float _powerAmount;

    [Header("VARIABLES")]
    public float ChargingIncrement = 1;
    public float ChargingDecrement;
    public float pickRadius = 5;

    [Header("--- REFEREBCES ---")]
    [SerializeField] GameObject Liquid;
    Material LiquidMat;

    PlayerController playerInstance;
    PowerCorePooling poolInstance;
    PowerReserveManager PRMInstance;

    public float PowerAmount
    {
        get
        {
            return _powerAmount;
        }
        set
        {
            _powerAmount = value;
        }
    }

    public bool isEquiped;
    public bool isDropped;
    public bool isCharging;

    public enum PowerCoreState
    {
        Equipped,
        BackToSocket,
        Charging,
        Dropped
    }

    PowerCoreState PCStates;

    private void Awake()
    {
        isEquiped = false;
        isDropped = false;
        isCharging = false;
    }
    private void Start()
    {
        LiquidMat = Liquid.GetComponent<MeshRenderer>().material;

        playerInstance = PlayerController.instance;
        poolInstance = PowerCorePooling.instance;
        PRMInstance = PowerReserveManager.instance;
    }
    private void Update()
    {
        PowerAmountSetLimit();
        HandlePowerCoreState(PCStates);
        LiquidVisuals();
    }
    void LiquidVisuals()
    {
        float LiquidFillVal = Mathf.InverseLerp(0, PRMInstance.powerAmountMax, _powerAmount);
        LiquidMat.SetFloat("_PowerFill", LiquidFillVal);
    }
    void PowerAmountSetLimit()
    {
        PowerAmount = _powerAmount >= PRMInstance.powerAmountMax ? PRMInstance.powerAmountMax : _powerAmount;
    }
    bool CheckPlayerDistance()
    {
        float dist = Vector3.Distance(transform.position, playerInstance.transform.position);
        return dist <= pickRadius;
    }
    public void Interact()
    {
        if (PRMInstance.currentPowerCore == null)
        {
            if (CheckPlayerDistance())
            {
                PRMInstance.currentPowerCore = this;
                PRMInstance.powerAmount = _powerAmount;
                Equipped();
                poolInstance.ReturnPowerCore(gameObject);
            }
        }
    }
    public void HandlePowerCoreState(PowerCoreState state)
    {
        switch (state)
        {
            case PowerCoreState.Equipped:

                break;
            case PowerCoreState.BackToSocket:
                PowerAmount += Time.deltaTime * ChargingIncrement;
                break;
            case PowerCoreState.Dropped:
                //Interact();
                break;
            case PowerCoreState.Charging:
                if (_powerAmount <= 0)
                    PowerAmount = 0;
                else
                    PowerAmount -= Time.deltaTime * ChargingDecrement;
                break;
            default:
                Debug.LogWarning("Invalid power core state: " + state);
                break;
        }
    }
    public void Equipped()
    {
        isEquiped = true;
        isDropped = false;
        isCharging = false;
        PCStates = PowerCoreState.Equipped;
    }
    public void BackToSocket()
    {
        isEquiped = false;
        isDropped = false;
        isCharging = false;
        PCStates = PowerCoreState.BackToSocket;
    }
    public void Dropped()
    {
        isEquiped = false;
        isDropped = true;
        isCharging = false;
        PCStates = PowerCoreState.Dropped;
    }
    public void Charging()
    {
        isEquiped = false;
        isDropped = false;
        isCharging = true;
        PCStates = PowerCoreState.Charging;
    }
}

/*[CustomEditor(typeof(PowerCores))]
public class PowerCoresEditor : Editor
{
    private void OnSceneGUI()
    {
        PowerCores pCore = (PowerCores)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(pCore.transform.position, Vector3.up, Vector3.forward, 360, pCore.pickRadius);
    }
}*/
