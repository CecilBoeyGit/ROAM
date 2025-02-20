using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

[ExecuteInEditMode]
public class WeaponBaseControl : MonoBehaviour
{

    [Header("REFERENCES")]
    [SerializeField] private float shootingConsumption = 2.0f;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePos;
    [SerializeField] GameObject fireDir;
    [SerializeField] float fireSoundRange = 10.0f;
    [SerializeField] float bulletSpeed = 10f;
    [SerializeField] float fireRate = 0.2f;
    LineRenderer laserPointer;
    ObjectsPoolingDefault BulletsPool;

    float nextFireTime = 0f;

    [Header("Audio")]
    AudioSource ads;
    [SerializeField] private AudioClip[] adcp = new AudioClip[2];
    bool isPlayingSFX = false;

    [Header("UI")]
    [SerializeField] Slider UI_WeaponAmount;

    Vector3 LaserHitPoint;

    PowerReserveManager PRMInstance;
    PlayerController pcInstance;
    InputSubscriptions _InputSub;

    public static Action<float> PowerDecrement; 
    public static Action GunFired;

    GamePadVibrationManager _GamePadVibInstance;

    public static WeaponBaseControl instance;


    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }
    private void Start()
    {
        PRMInstance = PowerReserveManager.instance;
        pcInstance = PlayerController.instance;
        _InputSub = InputSubscriptions.instance;
        _GamePadVibInstance = GamePadVibrationManager.instance;

        ads = GetComponent<AudioSource>();
        laserPointer = fireDir.GetComponent<LineRenderer>();

        BulletsPool = GameObject.Find("BulletsPool")?.GetComponent<ObjectsPoolingDefault>();
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            DrawCircle(transform.parent.position, fireSoundRange, 20, Color.red);
            return;
        }


        PowerVariableHolder();
        if (pcInstance.PlayerConstrained || pcInstance.AbilitiesConstrained)
            return;

        if (PRMInstance.weaponAmount >= shootingConsumption)
        {
            if (_InputSub.FireInput && Time.time >= nextFireTime)
            {
                WeaponFireShots();
                WeaponSound();
                PRMInstance.isConsumingWeaponAmount = true;
                PRMInstance.weaponAmount -= shootingConsumption;
                nextFireTime = Time.time + 1.0f / fireRate;
            }
        }

        //DebugSection();
    }

    public void FireInputCanceled()
    {
        PRMInstance.isConsumingWeaponAmount = false;
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

        _GamePadVibInstance.Rumble(0.1f, 0.1f, 1.0f);

        ads.Play();

        //GameObject bullets = Instantiate(bulletPrefab, firePos.position, firePos.rotation);
        if (BulletsPool == null)
            return;

        GameObject bullets = BulletsPool.GetPooledObject(firePos.position, firePos.rotation);
        Rigidbody bulletRB = bullets.GetComponent<Rigidbody>();

        if (bulletRB != null)
            bulletRB.AddForce(bullets.transform.forward * bulletSpeed, ForceMode.VelocityChange);
    }
    void WeaponSound()
    {
        int PowerCoreLayer = LayerMask.GetMask("Enemies");
        Collider[] colliders = Physics.OverlapSphere(transform.position, fireSoundRange, PowerCoreLayer);//Search for enemies within range
        var NewEnemyList = colliders.Select(col => col.GetComponent<EnemyBehavior>()).Where(socket => socket != null).ToList();
        foreach(EnemyBehavior enemy in NewEnemyList)
        {
            IWeaponSoundInterface _weaponSoundInterface = enemy.GetComponent<IWeaponSoundInterface>();
            if (_weaponSoundInterface == null)
                return;
            _weaponSoundInterface.WeaponSoundTriggered();
        }
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
    void DrawCircle(Vector3 center, float radius, int segments, Color color)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

            Debug.DrawLine(prevPoint, newPoint, color);
            prevPoint = newPoint;
        }
    }
}
