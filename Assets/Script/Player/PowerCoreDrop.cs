using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerCoreDrop : MonoBehaviour
{

    [Header("REFERENCES")]
    [SerializeField] GameObject powerCorePrefab;

    PowerCorePooling poolInstance;
    PowerReserveManager PRMInstance;

    public static PowerCoreDrop instance;

    private void OnEnable()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    private void Start()
    {
        poolInstance = PowerCorePooling.instance;
        PRMInstance = PowerReserveManager.instance;
    }
    public void PowerCoreActions()
    {
        if (PRMInstance.isEquiped)
        {
            print("PowerCore Dropped ---");

            Vector3 instPos = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z - 2);
            GameObject powerCoreObj = poolInstance.GetPowerCore();
            powerCoreObj.transform.position = instPos;
            powerCoreObj.transform.localScale = Vector3.one * 1;
            int powerCoreLayerIndex = LayerMask.NameToLayer("PowerCore");
            powerCoreObj.layer = powerCoreLayerIndex;

            PowerCores powerCoreChild = powerCoreObj.GetComponent<PowerCores>();
            powerCoreChild.Dropped();
            PRMInstance.currentPowerCore = null;
        }
    }
}
