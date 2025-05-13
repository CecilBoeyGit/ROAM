using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InjuredVolumeManager : MonoBehaviour
{

    [Header("--- OVERRIDES ---")]
    [SerializeField] bool InjuredVol = false;
    [SerializeField] bool WeaponVol = false;

    Animator Anim;

    AudioSource ads;

    private void OnEnable()
    {
        MeleeAttackTrigger.Attacked += AttackTriggered;
        //WeaponBaseControl.GunFired += WeaponTriggered;
        //EnemyBehavior.OnDeathTriggered += WeaponTriggered;
        Bullets.OnHitTriggered += WeaponTriggered;
    }
    private void OnDisable()
    {
        MeleeAttackTrigger.Attacked -= AttackTriggered;
        //WeaponBaseControl.GunFired -= WeaponTriggered;
        //EnemyBehavior.OnDeathTriggered -= WeaponTriggered;
        Bullets.OnHitTriggered -= WeaponTriggered;
    }


    private void Start()
    {
        Anim = GetComponent<Animator>();
        ads = GetComponent<AudioSource>();
    }

    void AttackTriggered()
    {
        if (InjuredVol)
        {
            Anim.SetTrigger("VolumeTriggered");
            ads.Play();
        }
    }
    void WeaponTriggered()
    {
        if (WeaponVol)
            Anim.SetTrigger("VolumeTriggered");
    }
}
