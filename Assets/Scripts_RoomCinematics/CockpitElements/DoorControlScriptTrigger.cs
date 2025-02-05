using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DoorControlScriptTrigger : MonoBehaviour
{
    public DoorControlScript doorcontrolscript;
    public AudioSource audioSource;

    [Header("--- SEQUENCING ---")]
    [SerializeField] bool OnBoardingDoor_PowerCore = false;
    [SerializeField] bool OnBoardingDoor_2F = false;
    [SerializeField] bool OnBoardingDoor_Enemy = false;
    public bool Day01Door = false;
    int trueIndex = -1;

    [SerializeField] private List<bool> DoorConditions = new List<bool>();

    [SerializeField] bool Door_PowerCore = false;
    [SerializeField] bool Door_2F = false;
    [SerializeField] bool Door_Enemy = false;

    void DoorTrueIndex()
    {
        for (int i = 0; i < DoorConditions.Count; i++)
        {
            if (DoorConditions[i])
            {
                trueIndex = i;
                break;
            }
        }
    }
    private void OnEnable()
    {
        PowerReserveManager.PowerCorePickedUp += PowerCoreDoor;
        CoreLoopManager.SecondPowerCorePickedUp += TwoFDoor;
        EnemyBehavior.TutorialBotKilled += EnemyDoor;
    }
    private void OnDisable()
    {
        PowerReserveManager.PowerCorePickedUp -= PowerCoreDoor;
        CoreLoopManager.SecondPowerCorePickedUp -= TwoFDoor;
        EnemyBehavior.TutorialBotKilled -= EnemyDoor;
    }
    private void Start()
    {
        DoorConditions.Add(OnBoardingDoor_PowerCore);
        DoorConditions.Add(OnBoardingDoor_2F);
        DoorConditions.Add(OnBoardingDoor_Enemy);
        DoorConditions.Add(Day01Door);
    }

    private void Update()
    {
        DoorTrueIndex();
        RegularDoor();
        OnQueueOpen();
    }
    void OnQueueOpen()
    {
        switch (trueIndex)
        {
            case 0:
                if (Door_PowerCore)
                {
                    doorcontrolscript.canOpen = true;

                    if (audioSource != null)
                    {
                        audioSource.Play();
                    }
                }
                break;
            case 1:
                if (Door_2F)
                {
                    doorcontrolscript.canOpen = true;

                    if (audioSource != null)
                    {
                        audioSource.Play();
                    }
                }
                break;
            case 2:
                if (Door_Enemy)
                {
                    doorcontrolscript.canOpen = true;

                    if (audioSource != null)
                    {
                        audioSource.Play();
                    }
                }
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        switch (trueIndex)
        {
            case 3:
                if (other.CompareTag("Player"))
                {
                    doorcontrolscript.canOpen = true;

                    if (audioSource != null)
                    {
                        audioSource.Play();
                    }
                }
                break;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (Day01Door)
        {
            if (other.CompareTag("Player"))
            {
                //doorcontrolscript.moveBack();
            }
        }
    }

    void PowerCoreDoor()
    {
        Door_PowerCore = true;
        Door_2F = false;
        Door_Enemy = false;
        Day01Door = false;
    }
    void TwoFDoor()
    {
        Door_PowerCore = false;
        Door_2F = true;
        Door_Enemy = false;
        Day01Door = false;
    }
    void EnemyDoor()
    {
        Door_PowerCore = false;
        Door_2F = false;
        Door_Enemy = true;
        Day01Door = false;
    }
    void RegularDoor()
    {
        if (Day01Door)
        {
            Door_PowerCore = false;
            Door_2F = false;
            Door_Enemy = false;
        }
    }
}
