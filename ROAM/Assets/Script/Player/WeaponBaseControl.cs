using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class WeaponBaseControl : MonoBehaviour
{

    [Header("REFERENCES")]
    [SerializeField] private float shootingConsumption = 2.0f;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePos;
    [SerializeField] GameObject fireDir;
    [SerializeField] float bulletSpeed = 10f;
    [SerializeField] float fireRate = 0.2f;
    LineRenderer laserPointer;

    float nextFireTime = 0f;

    [Header("Aduio")]
    AudioSource ads;
    [SerializeField] private AudioClip[] adcp = new AudioClip[2];
    bool isPlayingSFX = false;

    [Header("UI")]
    [SerializeField] Slider UI_WeaponAmount;

    Vector3 LaserHitPoint;

    PowerReserveManager PRMInstance;
    PlayerController pcInstance;

    public static Action<float> PowerDecrement; 
    public static Action GunFired;

    private void Start()
    {
        PRMInstance = PowerReserveManager.instance;
        pcInstance = PlayerController.instance;

        ads = GetComponent<AudioSource>();
        laserPointer = fireDir.GetComponent<LineRenderer>();
    }

    private void Update()
    {
        PowerVariableHolder();
        if (!pcInstance.PlayerConstrained)
        {
            if (!pcInstance.AbilitiesConstrained)
            {
                if (PRMInstance.weaponAmount >= shootingConsumption)
                {
                    if (Input.GetMouseButton(0) && Time.time >= nextFireTime && !PRMInstance.isHeldDown)
                    {
                        WeaponFireShots();
                        PRMInstance.weaponAmount -= shootingConsumption;
                        nextFireTime = Time.time + 1.0f / fireRate;
                    }
                }
            }
        }
        //DebugSection();
    }
    private void FixedUpdate()
    {
        LaserPointerPositions();
    }
    void PowerVariableHolder()
    {
        shootingConsumption = PRMInstance.shootingConsumption;
    }
    void WeaponFireShots()
    {
        GunFired?.Invoke();

        ads.Play();

        GameObject bullets = Instantiate(bulletPrefab, firePos.position, firePos.rotation);

        Rigidbody bulletRB = bullets.GetComponent<Rigidbody>();

        if (bulletRB != null)
            bulletRB.AddForce(bullets.transform.forward * bulletSpeed, ForceMode.VelocityChange);
    }
    void LaserPointerPositions()
    {
        //firePos.LookAt(fireDir.transform.position);

        RayCastLaser();

        laserPointer.SetPosition(0, firePos.position);
        laserPointer.SetPosition(1, LaserHitPoint);
    }
    void RayCastLaser()
    {
        RaycastHit hit;
        int enemyLayerMask = LayerMask.NameToLayer("Enemies");
        LayerMask allOtherMasks = ~(1 << enemyLayerMask);
        if (Physics.Raycast(firePos.position, firePos.forward, out hit, Mathf.Infinity, allOtherMasks, QueryTriggerInteraction.Ignore))
            LaserHitPoint = hit.point;
        else
            LaserHitPoint = fireDir.transform.position;

    }
    void DebugSection()
    {
        UI_WeaponAmount.GetComponent<Slider>();
        float hpLerpVal = Mathf.InverseLerp(0, PRMInstance.weaponAmountMax, PRMInstance.weaponAmount);
        UI_WeaponAmount.value = hpLerpVal;
    }
}
