using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MeleeAttackTrigger : MonoBehaviour
{

    [SerializeField] float AttackStrength  = 20;

    public static Action Attacked;

    PlayerController pcInstance;

    private void Start()
    {
        pcInstance = PlayerController.instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            pcInstance.healthPoint -= AttackStrength;
            Attacked?.Invoke();
        }
    }
}
