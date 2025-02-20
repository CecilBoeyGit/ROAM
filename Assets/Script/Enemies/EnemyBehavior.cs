using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System;

[ExecuteInEditMode]
public class EnemyBehavior : MonoBehaviour, IWeaponSoundInterface
{

    PlayerController pcInstance;
    NavMeshAgent nmAgent;
    MeshRenderer mrd;
    Transform attackRangeTrigger;

    [Header("--- References ---")]
    [SerializeField] Material mat;
    [SerializeField] GameObject ExploParticles;

    [Header("--- EnemyProperties ---")]
    public float healthPoint = 2.0f;

    [Header("--- BehaviorProperties ---")]
    [SerializeField] Transform patrolPointsGroup;
    private List<Transform> patrolPoints = new List<Transform>();
    private int currentPatrolIndex = -1;
    [SerializeField] float detectionTime = 0.1f;
    [SerializeField] float VisibilityDelayTime = 1.0f;

    [Header("--- AttackParameters ---")]
    [SerializeField] float attackColliderRadius = 5.0f;
    [SerializeField] float attackDistanceThreshold = 5.0f;
    [SerializeField] float chaseDistanceThreshold = 10.0f;
    bool idleRangeSpooling = false;
    bool attackRange = false;
    bool isAttacking = false;
    bool isChasing = true;
    bool isStunned = false;
    [HideInInspector] public bool forcedVisibilityToggle = false;

    [Header("--- ParticleSystems ---")]
    [SerializeField] ParticleSystem detectionParticles;
    [SerializeField] float particleEmissionTime = 1.2f;
    Coroutine CO_DetectionParticles, CO_StunnedPhase;

    Coroutine CO_AttackSpool, CO_Attacking, CO_IdleRange, CO_RevealSpool;

    [Header("--- UI ---")]
    public Text StatusText;

    GamePadVibrationManager _GamePadVibInstance;

    [Header("--- SPECIAL CASES ---")]
    [SerializeField] bool DEBUGBot = false;
    [SerializeField] bool DEBUGSTATUS = false;
    [SerializeField] bool TutorialBot = false;
    [SerializeField] float TutorialDistanceThreshold = 10.0f;
    public static event Action TutorialBotKilled;
    public static Action OnDeathTriggered;

    public enum enemyStates
    {
        IdleState,
        ChaseState,
        AttackState,
        StunnedState,
        DeathState
    }

    public enemyStates enemyStateControl;

    // Start is called before the first frame update
    void Start()
    {
        pcInstance = PlayerController.instance;
        _GamePadVibInstance = GamePadVibrationManager.instance;
        attackRangeTrigger = transform.Find("AttackRange");
        attackRangeTrigger.localScale = Vector3.zero;

        nmAgent = GetComponent<NavMeshAgent>();        
        mrd = GetComponent<MeshRenderer>();
        detectionParticles.GetComponent<ParticleSystem>();

        ResetDefaultState();
    }
    public void ResetDefaultState()
    {
        enemyStateControl = enemyStates.IdleState;

        FindNearestPatrolPoints();
        foreach (Transform child in patrolPointsGroup)
        {
            patrolPoints.Add(child);
        }

        if (nmAgent != null)
            nmAgent.SetDestination(transform.position);

        if (!Application.isPlaying)
            mat = mrd.sharedMaterial;
        else
            mat = mrd.material;
        mat.SetFloat("_Dissolve", 1);
    }
    void FindNearestPatrolPoints()
    {
        patrolPointsGroup = GameObject.Find("SpawnPoints").transform;
        print(patrolPointsGroup);
    }

    void DeathCondition()
    {
        if (healthPoint <= 0)
        {
            enemyStateControl = enemyStates.DeathState;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.isPlaying)
        {
            DrawCircle(transform.position, chaseDistanceThreshold, 20, Color.yellow);
            DrawCircle(transform.position, attackColliderRadius, 20, Color.red);
            return;
        }
        else
        {
            if (DEBUGBot)
            {
                DrawCircle(transform.position, chaseDistanceThreshold, 20, Color.yellow);
                DrawCircle(transform.position, attackColliderRadius, 20, Color.red);
            }

            if (DEBUGSTATUS)
            {
                if (StatusText == null)
                    return;

                StatusText.gameObject.SetActive(true);
                StatusText.text = enemyStateControl.ToString();

                DrawCircle(transform.position, chaseDistanceThreshold, 20, Color.yellow);
                DrawCircle(transform.position, attackColliderRadius, 20, Color.red);
            }
            else
            {
                if (StatusText == null)
                    return;

                StatusText.text = null;
                StatusText.gameObject.SetActive(false);
            }
        }

        print(DissolveTime);
        DeathCondition();
        DistanceToPlayer();
        EnemyStatesMachine();
    }
    public void WeaponSoundTriggered()
    {
        if (enemyStateControl == enemyStates.ChaseState || 
            enemyStateControl == enemyStates.AttackState ||
            enemyStateControl == enemyStates.DeathState) //If enemy is ALREADY chasing or attacking or dead while WeaponSound is triggered, skip this function entirely
            return;

        print("Weapong Sound Triggered");
        if (_GamePadVibInstance != null)
            _GamePadVibInstance.Rumble(1.2f, 0.1f, 0.5f);
        forcedVisibilityToggle = true; //If enemy is triggerd by WeaponSound, visibility is turned ON during the ChaseState
        enemyStateControl = enemyStates.ChaseState; //Immediately change to chasing behaviors when triggered by weapon firing sound
    }
    void EnemyStatesMachine()
    {
        switch(enemyStateControl)
        {
            case enemyStates.IdleState:
                IdleBehaviors();
                break;
            case enemyStates.ChaseState:
                ChaseBehaviors();
                break;
            case enemyStates.AttackState:
                AttackBehaviors();
                break;
            case enemyStates.StunnedState:
                StunnedBehaviors();
                break;
            case enemyStates.DeathState:
                DeathBehaviors();
                break;
        }
    }
    void IdleBehaviors()
    {
        //print("Idling");

        if (DEBUGBot)
        {
            forcedVisibilityToggle = true;
            Dissolve(false, 0.1f);
            return;
        }

        if(!forcedVisibilityToggle)
            Dissolve(true, 0.3f);

        if (nmAgent.remainingDistance <= nmAgent.stoppingDistance && !nmAgent.pathPending)
        {
            if (!TutorialBot)
            {
                if (switchingPatrolPoint)
                    return;

                if (CO_RandomPatrolPoints != null)
                    StopCoroutine(RandomPatrolWithCooldown(0));
                CO_RandomPatrolPoints = StartCoroutine(RandomPatrolWithCooldown(PatrolWaitDuration));
            }
        }
    }
    public float PatrolWaitDuration = 8.0f;
    bool switchingPatrolPoint = false;
    Coroutine CO_RandomPatrolPoints;
    IEnumerator RandomPatrolWithCooldown(float waitDuration)
    {
        switchingPatrolPoint = true;

        yield return new WaitForSeconds(waitDuration);

        SetRandomPatrolPoint();
        switchingPatrolPoint = false;
    }
    void SetRandomPatrolPoint()
    {
        if (patrolPoints.Count > 0)
        {
            int previousIndex = currentPatrolIndex;
            while (currentPatrolIndex == previousIndex)
            {
                currentPatrolIndex = UnityEngine.Random.Range(0, patrolPoints.Count);
            }
            nmAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
        else
        {
            Debug.LogWarning("No patrol points assigned to the enemy.");
        }
    }
    void ChaseBehaviors()
    {
        //print("Chasing");

        if (DEBUGBot)
        {
            forcedVisibilityToggle = true;
            Dissolve(false, 0.1f);
        }

        if(forcedVisibilityToggle)
            Dissolve(false, 0.3f);

        if (isChasing && !isAttacking)
        {
            if (!forcedVisibilityToggle)
                Dissolve(true, 0.3f);
            if (nmAgent == null)
                return;
            if (!TutorialBot)
                nmAgent.destination = pcInstance.transform.position;
        }
        else
        {
            if (!forcedVisibilityToggle)
                Dissolve(false, 0.3f);
            if (nmAgent == null)
                return;
            nmAgent.destination = transform.position;
            transform.LookAt(new Vector3(pcInstance.transform.position.x, transform.position.y, pcInstance.transform.position.z), Vector3.up);

            if (!isAttacking)
            {
                enemyStateControl = enemyStates.AttackState;
            }
        }
    }

    public float AttackSpoolTime = 0.3f;

    void AttackBehaviors()
    {
        //print("Attacking");

        if (isAttacking)
            return;

        if (CO_AttackSpool != null)
            StopCoroutine(CO_AttackSpool);

        CO_AttackSpool = StartCoroutine(AttackSpool(AttackSpoolTime));
    }

    void StunnedBehaviors()
    {
        print("Enemy Behaviors STUNNED");

        Dissolve(false, 0.5f);

        if (nmAgent == null)
            return;
        nmAgent.destination = transform.position;
        if (CO_AttackSpool != null)
            StopCoroutine(CO_AttackSpool);
        if (CO_Attacking != null) //If the enemy is currently attacking, stop the process immediately when stunned
            StopCoroutine(CO_Attacking);

        attackRangeTrigger.localScale = Vector3.zero;
        isAttacking = false;
    }
    void DeathBehaviors()
    {
        //print("Death");

        MaterialDefault();
        Instantiate(ExploParticles, transform.position, Quaternion.identity);
        if (TutorialBot)
        {
            TutorialElementPlayScript.isPlayingAudio = false;
            TutorialBotKilled?.Invoke();
        }
        nmAgent = null;
        OnDeathTriggered?.Invoke();

        if (_GamePadVibInstance != null)
            _GamePadVibInstance.Rumble(0.1f, 0.1f, 0.3f);

        Destroy(gameObject);
    }
    IEnumerator AttackSpool(float spoolTime)
    {
        isAttacking = true;
        forcedVisibilityToggle = false;

        float timer = 0;
        while(timer < spoolTime)
        {
            Dissolve(false, 0.3f);
            AttackEffects(timer, spoolTime);
            timer += Time.deltaTime;
            yield return null;
        }

        if (CO_Attacking != null)
            StopCoroutine(CO_Attacking);

        CO_Attacking = StartCoroutine(Attacking(0.2f));
    }  
    IEnumerator Attacking(float attackTime)
    {
        float timer = 0;
        while (timer < attackTime)
        {         
            timer += Time.deltaTime;

            AttackingEffects(timer, attackTime);
            yield return null;
        }

        //Attack after expansion warning, and wait for n second to allow the sphere trigger to register
        attackRangeTrigger.localScale = Vector3.one * attackColliderRadius;
        yield return new WaitForSeconds(0.1f);
        isAttacking = false;

        if (TutorialBot)
        {
            attackRangeTrigger.localScale = Vector3.zero;

            if (CO_RevealSpool != null)
            {
                StopCoroutine(CO_RevealSpool);
            }
            CO_RevealSpool = StartCoroutine(RevealDelay(VisibilityDelayTime));
        }
        else
        {
            enemyStateControl = enemyStates.DeathState; //Switch to DeathState
        }
    }
    IEnumerator RevealDelay(float duration)
    {
        forcedVisibilityToggle = true; Dissolve(false, 0.5f);
        yield return new WaitForSeconds(duration);
        forcedVisibilityToggle = false;
    }
    void DistanceToPlayer()
    {
        float dist = Vector3.Distance(transform.position, pcInstance.transform.position);
        //print("Distance: " + dist);
        if(dist <= chaseDistanceThreshold)
        {
            if (dist <= attackDistanceThreshold && !attackRange)
                enemyStateControl = enemyStates.ChaseState; //Instantly changing to chasing when the player is too close to the enemy
            else
            {
                if (!attackRange && !idleRangeSpooling)
                {
                    if (CO_IdleRange != null)
                    {
                        StopCoroutine(CO_IdleRange);
                        idleRangeSpooling = false;
                    }
                    CO_IdleRange = StartCoroutine(RangeBoolean(detectionTime, true, enemyStates.ChaseState));
                }
            }
        }
        #region Out of range behaviors
        /*        else
                {
                    if (playerInRange && !idleRangeSpooling && !isAttacking && !isStunned)
                    {
                        if (CO_IdleRange != null)
                        {
                            StopCoroutine(CO_IdleRange);
                            idleRangeSpooling = false;
                        }
                        CO_IdleRange = StartCoroutine(RangeBoolean(detectionTime, false, enemyStates.IdleState));
                    }
                }*/
        #endregion

        if (attackRange)
        {
            float attaclDistanceHolder = TutorialBot ? TutorialDistanceThreshold : attackDistanceThreshold;
            isChasing = dist <= attaclDistanceHolder ? false : true;
        }
        else
            isChasing = true;
    }
    IEnumerator RangeBoolean(float spoolDuration, bool condition, enemyStates state)
    {
        idleRangeSpooling = true;

        yield return new WaitForSeconds(spoolDuration);

        attackRange = condition;
        enemyStateControl = state;
        idleRangeSpooling = false;
    }

    public void ScannedBehaviors()
    {
        if (forcedVisibilityToggle || DEBUGBot)
            return;

        if (CO_DetectionParticles != null)
            detectionParticles.Stop();

        CO_DetectionParticles = StartCoroutine(ScannedEffects(2.0f));
    }

    private bool beingScanned = false;
    IEnumerator ScannedEffects(float time)
    {
        beingScanned = true;

        //detectionParticles.Play(); //print(gameObject.name + "VFX Playing");
        mat.SetFloat("_Dissolve", 0.6f);
        forcedVisibilityToggle = true;
        float timer = 0;
        while (timer < time)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        forcedVisibilityToggle = false;
        //detectionParticles.Stop();

        beingScanned = false;
    }

    public void PulseScannedBehaviors()
    {
        if (CO_StunnedPhase != null)
            StopCoroutine(CO_StunnedPhase);
        
        CO_StunnedPhase = StartCoroutine(StunnedPhase(1.5f));
    }
    public void SonarBeamScannedBehaviors()
    {
        forcedVisibilityToggle = true; Dissolve(false, 0.5f);
    }
    public void SonarBeamExitBehaviors()
    {
        print("--- SonarBeamExitBehaviors Triggered ---");

        if (CO_RevealSpool != null)
            StopCoroutine(CO_RevealSpool);

        CO_RevealSpool = StartCoroutine(RevealDelay(VisibilityDelayTime));
    }
    IEnumerator StunnedPhase(float time)
    {
        isStunned = true;
        enemyStateControl = enemyStates.StunnedState;
        float timer = 0;

        while (timer < time)
        {
            timer += Time.deltaTime;
            mat.SetFloat("_NoiseStrength", 0);
            yield return null;
        }

        mat.SetFloat("_NoiseStrength", 0.4f);

        if (CO_RevealSpool != null)
        {
            StopCoroutine(CO_RevealSpool);
        }
        CO_RevealSpool = StartCoroutine(RevealDelay(VisibilityDelayTime));
        isStunned = false;
        enemyStateControl = enemyStates.ChaseState;
    }

    // --- MATERIAL FUNCTIONS ---

    void MaterialDefault()
    {
        Dissolve(true, 1.0f);
        mat.SetFloat("_NoiseStrength", 0.4f);
        mat.SetFloat("_EmissiveIntensity", 2);
        mat.SetFloat("_NoiseSize", 2);
    }
    [SerializeField] private float DissolveTime = 0f;
    void Dissolve(bool isDissolving, float duration)
    {
        if (isDissolving)
        {
            if (DissolveTime < duration)
                DissolveTime += Time.deltaTime;
            else
                DissolveTime = duration;
        }
        else
        {
            if (DissolveTime > 0)
                DissolveTime -= Time.deltaTime;
            else
                DissolveTime = 0;
        }

        float lerpVal = Mathf.InverseLerp(0, duration, DissolveTime);
        mat.SetFloat("_Dissolve", lerpVal);
    }
    void AttackEffects(float timer, float duration)
    {
        float lerpVal = Mathf.InverseLerp(0, duration, timer);
        float EffectsLerp = Mathf.Lerp(2, 4, lerpVal);
        mat.SetFloat("_EmissiveIntensity", EffectsLerp);
        mat.SetFloat("_NoiseSize", EffectsLerp);
    }
    void AttackingEffects(float timer, float duration)
    {
        float lerpVal = Mathf.InverseLerp(0, duration, timer);
        float EffectsLerp = Mathf.Lerp(0.4f, 3, lerpVal);
        mat.SetFloat("_NoiseStrength", EffectsLerp);
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
