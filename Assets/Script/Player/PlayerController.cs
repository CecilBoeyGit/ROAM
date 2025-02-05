using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    private Camera mainCamera;
    public CinemachineVirtualCamera vCam;
    public CinemachineBasicMultiChannelPerlin vCamNoise;

    private CharacterController controller;
    private Vector3 playerVelocity;

    [Header("--- OVERRIDES ---")]
    public bool PlayerConstrained = false;
    public bool AbilitiesConstrained = false;

    [Header("--- Movement ---")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] private bool groundedPlayer;
    [SerializeField] private float walkSpeed = 2.0f;
    [SerializeField] private float runSpeed = 4.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float torsoSensitivity = 2.0f;
    public float movementMasterControl = 1.0f;

    Vector3 current_pos, last_pos;
    float speed1, speed2;
    public float velocity;

    [Header("--- Camera ---")]
    [SerializeField] float FOVSpeed = 5f;
    [SerializeField] float FOVMin = 60f; [SerializeField] float FOVMax = 80f;
    float radiusLerpTimer = 0, radiusIncrement = 0;

    [Header("--- References ---")]
    public float powerAmount;
    [SerializeField] private float powerAmountMax;
    [SerializeField] private float scannerConsumption;
    [SerializeField] private float runningConsumption;
    [SerializeField] GameObject torsoFollowTarget;
    [SerializeField] FieldOfView fovController;
    [SerializeField] float scannerRadiusMax;
    [SerializeField] Material Mat_ScanLine;
    [SerializeField] GameObject WeaponCursor;
    public bool isScanning = false;
    public bool buttonHeldDown = false;
    Coroutine CO_SonarScanning;

    public float healthPoint;

    [Header("--- INTERFACE ---")]
    [SerializeField] private float pickUpRadius = 5.0f;
    public bool facingSockets = false;

    [Header("--- Animation ---")]
    [SerializeField] Animator anim;

    [Header("--- Timeline ---")]
    [SerializeField] GameObject DeathTimeline;

    [Header("--- Aduio ---")]
    AudioSource ads;
    [SerializeField] private AudioClip[] adcp = new AudioClip[2];
    bool isPlayingSFX = false;

    [Header("--- UI ---")]
    [SerializeField] Slider UI_healthPoint;
    [SerializeField] Slider UI_powerAmount;

    [Header("--- DEBUG ---")]
    [SerializeField] private bool usingPlaneCast = false;
    [SerializeField] private bool usingForwardMovement = false;
    [SerializeField] private bool charMoving = false;
    [SerializeField] private float charSpeed;
    [SerializeField] private bool DebugScanner = false;
    int SonarScannerClicked = 0;

    PowerReserveManager PRMInstance;
    ReloadAllScenes ReloadInstance;
    PowerCoreDrop PCDropInstance;
    public static PlayerController instance;
    private void OnEnable()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);


        fovController = GetComponent<FieldOfView>();
    }

    void ScanLineConfigDefault(float defaultVal)
    {
        Mat_ScanLine.SetFloat("_OverallEffect", defaultVal);
        Mat_ScanLine.SetFloat("_MaskRadius", defaultVal);
        Mat_ScanLine.SetFloat("_ScanLineTiling", defaultVal);
    }

    void Start()
    {
        mainCamera = Camera.main;
        vCam.GetComponent<CinemachineVirtualCamera>();

        controller = GetComponent<CharacterController>();
        vCamNoise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        vCamNoise.m_FrequencyGain = 0;

        current_pos = transform.position; last_pos = transform.position;
        speed1 = charSpeed; speed2 = charSpeed;

        fovController.viewRadius = 0;
        ScanLineConfigDefault(0);

        //anim.GetComponent<Animator>();
        ads = GetComponent<AudioSource>();

        ReloadInstance = ReloadAllScenes.instance;
        PRMInstance = PowerReserveManager.instance;
        PCDropInstance = PowerCoreDrop.instance;

        SonarScannerClicked = 0;

        DeathTimeline.SetActive(false);
    }
    void PowerVariableHolder()
    {
        scannerConsumption = PRMInstance.scannerConsumption;
        runningConsumption = PRMInstance.runningConsumption;
    }

    void Update()
    {
        if (!PlayerConstrained)
        {
            PowerVariableHolder();
            OnPlayerMove();
            SearchInteractables();
            if(!AbilitiesConstrained)
                ScannerControls();

            RunningCameraBehaviors();

            RunningSFX();
            RunningConsumption();
            if(anim != null)
                AnimationStates();

            HealthNullAction();
            //DebugSection();
        }
    }
    void SearchInteractables()
    {
        int PowerCoreLayer = LayerMask.GetMask("PowerCore");
        int PowerSocketLayer = LayerMask.GetMask("PowerSocket");

        if (Input.GetKeyDown(KeyCode.E))
        {       
            Collider[] colliders = Physics.OverlapSphere(transform.position, pickUpRadius, PowerCoreLayer);//Only search for PowerCores that are instantiated
            Collider[] collidersSockets = Physics.OverlapSphere(transform.position, pickUpRadius, PowerSocketLayer);//Only search for PowerSockets
            var NewSocketsList = collidersSockets.Select(col => col.GetComponent<PowerSockets>()).Where(socket => socket != null && socket.PlayerInZone).ToList();
            var NewGenSocketsList = collidersSockets.Select(col => col.GetComponent<GeneratorSocket>()).Where(socket => socket != null && socket.PlayerInZone).ToList();
            print(NewGenSocketsList.Count);
            if (NewSocketsList.Count == 0 && NewGenSocketsList.Count == 0)
            {
                if (PRMInstance.currentPowerCore != null)
                {
                    PCDropInstance.PowerCoreActions();
                    return;
                }
                if (colliders.Length > 0)
                {
                    Collider closestCollider = FindClosestCollider(colliders);
                    IPlayerInterface interactable = closestCollider.GetComponent<IPlayerInterface>();
                    if (interactable != null)
                    {
                        //print("Interactable Interacted");
                        interactable.Interact();
                    }
                }
            }
            NewSocketsList.Clear();
        }
    }
    Collider FindClosestCollider(Collider[] colliders)
    {
        Collider closestCollider = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);

            if (distance < closestDistance)
            {
                closestCollider = collider;
                closestDistance = distance;
            }
        }

        return closestCollider;
    }
    void ScannerControls()
    {
        if(Input.GetMouseButtonUp(1) && PRMInstance.weaponAmount >= scannerConsumption && !buttonHeldDown && !PRMInstance.isHeldDown)
        {
            print(isScanning + "isScanning Condition ---");
            if(!isScanning)
            {
                if (CO_SonarScanning != null)
                    StopCoroutine(CO_SonarScanning);

                if (!DebugScanner)
                    CO_SonarScanning = StartCoroutine(SonarScanning(1.0f));
                else
                    fovController.viewRadius = scannerRadiusMax;

                SonarScannerClicked++;
                string playerTransformString = ("World Position Trigger Point: " + transform.position + "Sonar Scanner: " + SonarScannerClicked);
                if (MetricManagerScript.instance != null)
                {
                    MetricManagerScript.instance.LogString("Sonar Scanner Clicked", playerTransformString.ToString());
                }
            }
        }
    }
    IEnumerator SonarScanning(float scanTime)
    {
        isScanning = true;

        PRMInstance.weaponAmount -= scannerConsumption;

        ads.clip = adcp[2];
        ads.Play();

        float timer = 0;
        while(timer < scanTime)
        {
            timer += Time.deltaTime;
            float lerpVal = Mathf.InverseLerp(0, scanTime, timer);
            fovController.viewRadius = Mathf.Lerp(0, scannerRadiusMax, lerpVal);

            Mat_ScanLine.SetFloat("_OverallEffect", lerpVal);
            Mat_ScanLine.SetFloat("_MaskRadius", lerpVal * (scannerRadiusMax - 3));
            Mat_ScanLine.SetFloat("_ScanLineTiling", lerpVal * (scannerRadiusMax - 5));

            yield return null;
        }

        fovController.viewRadius = 0;
        isScanning = false;

        ScanLineConfigDefault(0);
    }
    void RunningConsumption()
    {
        if (InRunningState() && SufficientRunningPower())
            PRMInstance.weaponAmount -= runningConsumption;
    }
    void RunningCameraBehaviors()
    {
        CameraFOVInterpolator(FOVSpeed);
        vCam.m_Lens.FieldOfView = radiusIncrement;
    }
    void CameraFOVInterpolator(float lerpSpeed)
    {
        if (isScanning)
        {
            if (radiusLerpTimer < 1)
                radiusLerpTimer += Time.deltaTime * lerpSpeed;
        }
        else
        {
            if (radiusLerpTimer > 0)
                radiusLerpTimer -= Time.deltaTime * lerpSpeed;
        }

        radiusIncrement = Mathf.Lerp(FOVMin, FOVMax, radiusLerpTimer);
    }
    bool SufficientRunningPower()
    {
        return PRMInstance.weaponAmount >= runningConsumption;
    }
    bool InRunningState()
    {
        return velocity != 0 && charSpeed == runSpeed;
    }
    void RunningSFX()
    {
        speed1 = charSpeed;
        if (velocity != 0)
        {
            if (!isPlayingSFX)
            {
                isPlayingSFX = true;

                if (charSpeed == walkSpeed)
                {
                    ads.clip = adcp[0];
                    ads.Play();
                }
                else
                {
                    ads.clip = adcp[1];
                    ads.Play();
                }
            }

            if (!ads.isPlaying)
                ads.Play();
            isPlayingSFX = speed1 == speed2;
            speed2 = speed1;
        }
        else
        {
            isPlayingSFX = false;
            ads.Stop();
        }
    }
    void AnimationStates()
    {
        bool isMoving = velocity != 0 ? true : false;
        anim.SetBool("IsWalking", isMoving);

        if (isMoving)
        {
            if (charSpeed == runSpeed)
                anim.SetFloat("RunningSpeed", 2.0f);
            else
                anim.SetFloat("RunningSpeed", 1.0f);
        }
    }
    void OnPlayerMove()
    {
        Aim();

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 move1 = (transform.forward * moveZ) + (transform.right * moveX);
        Vector3 move2 = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 move = usingForwardMovement ? move1 : move2;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            charSpeed = SufficientRunningPower() ? runSpeed : walkSpeed;
            vCamNoise.m_FrequencyGain = 0.1f;
        }
        else
        {
            charSpeed = walkSpeed;
            if(!buttonHeldDown)
                vCamNoise.m_FrequencyGain = 0;
        }
        controller.Move(move * Time.deltaTime * charSpeed * movementMasterControl);

        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        current_pos = transform.position;
        velocity = (current_pos - last_pos).magnitude / Time.deltaTime;
        last_pos = current_pos;
    }
    private void Aim()
    {
        var (success, position) = GetMousePosition();
        if (success)
        {
            var direction = position - transform.position;
            direction.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * mouseSensitivity * movementMasterControl);
            Vector3 mousePos = new Vector3(position.x, WeaponCursor.transform.position.y, position.z);
            //WeaponCursor.transform.position = Vector3.Lerp(WeaponCursor.transform.position, mousePos, Time.deltaTime * mouseSensitivity * movementMasterControl);
            //--- Deprecated because forklift no longer needs separate rotations ---
            //TorsoRotation(direction);
        }

        Debug.DrawLine(transform.position, position, Color.red);
    }
    void TorsoRotation(Vector3 direction)
    {
        //Converting the torso rotation into local space to distinguish the rotation from its parent 
        Quaternion lookDirection = Quaternion.LookRotation(direction, Vector3.up);
        Quaternion lookRotWorldSpace = Quaternion.Inverse(torsoFollowTarget.transform.parent.rotation) * lookDirection;
        torsoFollowTarget.transform.localRotation = Quaternion.Lerp(torsoFollowTarget.transform.localRotation, lookRotWorldSpace, Time.deltaTime * torsoSensitivity * movementMasterControl);
    }
    private (bool success, Vector3 position) GetMousePosition()
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (!usingPlaneCast)
        {
            //Method 1
            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))  
                return (success: true, position: hitInfo.point);
            else
                return (success: false, position: Vector3.zero);
        }
        else
        {
            //Method 2
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out float hitDistance))
            {
                Vector3 hitPoint = ray.GetPoint(hitDistance);
                return (success: true, position: hitPoint);
            }
            else
                return (success: false, position: Vector3.zero);
        }
    }
    void HealthNullAction()
    {
        if(healthPoint <= 0)
            DeathTimeline.SetActive(true);
    }
    public void ReloadSceneFunction()
    {
        ReloadInstance.ReloadInvoke();
    }
    void DebugSection()
    {
        UI_healthPoint.GetComponent<Slider>();
        float hpLerpVal = Mathf.InverseLerp(0, 100, healthPoint);
        UI_healthPoint.value = hpLerpVal;

        UI_powerAmount.GetComponent<Slider>();
        float powerLerpVal = Mathf.InverseLerp(0, powerAmountMax, powerAmount);
        UI_powerAmount.value = powerLerpVal;
    }
}
