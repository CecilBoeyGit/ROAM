using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[RequireComponent(typeof(PowerCores))]
public class PowerCores : MonoBehaviour, IPlayerInterface
{

    [Header("VARIABLES")]
    //public float ChargingIncrement = 1;
    //public float ChargingDecrement;
    public float pickRadius = 5;

    [Header("--- REFEREBCES ---")]
    [SerializeField] GameObject Liquid;
    Material LiquidMat;

    PlayerController playerInstance;
    PowerCorePooling poolInstance;
    PowerReserveManager PRMInstance;

    public bool isEquiped;
    public bool isDropped;

    public enum PowerCoreState
    {
        Equipped,
        BackToSocket,
        Charging,
        Dropped
    }

    public PowerCoreState PCStates;

    private void Awake()
    {
        isEquiped = false;
        isDropped = false;
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
        HandlePowerCoreState(PCStates);
        LiquidVisuals();
    }
    void LiquidVisuals()
    {
        float LiquidFillVal = 1; //PLACEHOLDER VALUE
        LiquidMat.SetFloat("_PowerFill", LiquidFillVal);
    }
    bool CheckPlayerDistance()
    {
        float dist = Vector3.Distance(transform.position, playerInstance.transform.position);
        return dist <= pickRadius;
    }
    public void Interact()
    {
        if (!isDropped)
            return;

        print("PowerCore picked up ---");

        Equipped();
        PRMInstance.currentPowerCore = this;
        poolInstance.ReturnPowerCore(gameObject);
    }
    public void HandlePowerCoreState(PowerCoreState state)
    {
        switch (state)
        {
            case PowerCoreState.Equipped:

                break;
            case PowerCoreState.BackToSocket:

                break;
            case PowerCoreState.Dropped:

                break;
            case PowerCoreState.Charging:

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
        PCStates = PowerCoreState.Equipped;
    }
    public void BackToSocket()
    {
        isEquiped = false;
        isDropped = false;
        PCStates = PowerCoreState.BackToSocket;
    }
    public void Dropped()
    {
        isEquiped = false;
        isDropped = true;
        PCStates = PowerCoreState.Dropped;
    }
    public void Charging()
    {
        isEquiped = false;
        isDropped = false;
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
