using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorManager : MonoBehaviour
{

    CoreLoopManager CLInstance;

    private void OnEnable()
    {
        //IntegrityManager.RundownSuccessAction += RundownSuccess;
    }
    private void OnDisable()
    {
        //IntegrityManager.RundownSuccessAction -= RundownSuccess;
    }

    private void Start()
    {
        CLInstance = CoreLoopManager.Instance;

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            ClosingInterface();
        }
    }
    void ClosingInterface()
    {
        CLInstance.Enum_DayStages = CoreLoopManager.DayStages.DisplayInterfaceSuccess;
    }
}
