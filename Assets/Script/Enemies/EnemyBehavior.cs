using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemyBehavior : MonoBehaviour, IWeaponSoundInterface
{

    PlayerController pcInstance;
    NavMeshAgent nmAgent;
    MeshRenderer mrd;
    Transform attackRangeTrigger;

    [Header("--- References ---")]
    [SerializeField] Material mat;
    [SerializeField] GameObject ExploParticles;
    ParticleSystem DeathExplosionParticles;

    [Header("--- EnemyProperties ---")]
    public float healthPoint = 2.0f;

    [Header("--- BehaviorProperties ---")]
    [SerializeField] Transform patrolPointsGroup;
    private List<Transform> patrolPoints = new List<Transform>();
    private int currentPatrolIndex = -1;
    [SerializeField] float detectionTime = 0.5f;
    [SerializeField] float VisibilityDelayTime = 1.0f;

    [Header("--- AttackParameters ---")]
    [SerializeField] float attackColliderRadius = 5.0f;
    [SerializeField] float attackDistanceThreshold = 5.0f;
    [SerializeField] float rangeThreshold = 10.0f;
    bool idleRangeSpooling = false;
    bool attackRange = false;
    bool isAttacking = false;
    bool isChasing = true;
    bool isStunned = false;
    bool masterVisibilityToggle = false;

    [Header("--- ParticleSystems ---")]
    [SerializeField] ParticleSystem detectionParticles;
    [SerializeField] float particleEmissionTime = 1.2f;
    Coroutine CO_DetectionParticles, CO_StunnedPhase;

    Coroutine CO_AttackSpool, CO_Attacking, CO_IdleRange, CO_RevealSpool;

    [Header("--- SPECIAL CASES ---")]
    [SerializeField] bool TutorialBot = false;
    [SerializeField] float TutorialDistanceThreshold = 10.0f;
    public static event Action TutorialBotKilled;

    enum enemyStates
    {
        IdleState,
        RegularState,
        AttackState,
        StunnedState
    }

    [SerializeField] enemyStates enemyStateControl;

    // Start is called before the first frame update
    void Start()
    {
        pcInstance = PlayerController.instance;
        attackRangeTrigger = transform.Find("AttackRange");
        attackRangeTrigger.localScale = Vector3.zero;

        enemyStateControl = enemyStates.IdleState;

        FindNearestPatrolPoints();
        foreach (Transform child in patrolPointsGroup)
        {
            patrolPoints.Add(child);
        }

        nmAgent = GetComponent<NavMeshAgent>();
        mrd = GetComponent<MeshRenderer>();
        mat = mrd.material;
        detectionParticles.GetComponent<ParticleSystem>();
    }
    void FindNearestPatrolPoints()
    {
        patrolPointsGroup = GameObject.Find("PatrolPoints").transform;
        print(patrolPointsGroup);
    }

    void DeathCondition()
    {
        if (healthPoint <= 0)
        {
            if (TutorialBot)
            {
                TutorialElementPlayScript.isPlayingAudio = false;
                TutorialBotKilled?.Invoke();
            }
            nmAgent = null;
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        DeathCondition();
        DistanceToPlayer();
        EnemyStatesMachine();
    }
    public void WeaponSoundTriggered()
    {
        enemyStateControl = enemyStates.RegularState; //Immediately change to chasing behaviors when triggered by weapon firing sound
    }
    void EnemyStatesMachine()
    {
        switch(enemyStateControl)
        {
            case enemyStates.IdleState:
                IdleBehaviors();
                break;
            case enemyStates.RegularState:
                RegularBehaviors();
                break;
            case enemyStates.AttackState:
                AttackBehaviors();
                break;
            case enemyStates.StunnedState:
                StunnedBehaviors();
                break;
        }
    }
    void IdleBehaviors()
    {
        if (nmAgent.remainingDistance <= nmAgent.stoppingDistance && !nmAgent.pathPending)
        {
            if(!TutorialBot)
                SetRandomPatrolPoint();
        }
        if (!masterVisibilityToggle)
            Dissolve(true, 0.5f);
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
    void RegularBehaviors()
    {
        //print("Enemy Behaviors REGULAR");

        if (isChasing && !isAttacking)
        {
            if (!masterVisibilityToggle)
                Dissolve(true, 0.5f);
            if (nmAgent == null)
                return;
            if (!TutorialBot)
                nmAgent.destination = pcInstance.transform.position;
        }
        else
        {
            if (!masterVisibilityToggle)
                Dissolve(false, 0.5f);
            if (nmAgent == null)
                return;
            nmAgent.destination = transform.position;
            transform.LookAt(new Vector3(pcInstance.transform.position.x, transform.position.y, pcInstance.transform.position.z), Vector3.up);

            if (!isAttacking)
            {
                if (CO_AttackSpool != null)
                    StopCoroutine(CO_AttackSpool);

                CO_AttackSpool = StartCoroutine(AttackSpool(0.8f));
            }
        }
    }
    void AttackBehaviors()
    {

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

    IEnumerator AttackSpool(float spoolTime)
    {
        isAttacking = true;

        float timer = 0;
        while(timer < spoolTime)
        {
            AttackEffects(timer, spoolTime);
            timer += Time.deltaTime;
            yield return null;
        }

        if (CO_Attacking != null)
            StopCoroutine(CO_Attacking);

        CO_Attacking = StartCoroutine(Attacking(0.5f));
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
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;

        MaterialDefault();

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
            Instantiate(ExploParticles, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
    IEnumerator RevealDelay(float duration)
    {
        masterVisibilityToggle = true; Dissolve(false, 0.5f);
        yield return new WaitForSeconds(duration);
        masterVisibilityToggle = false;
    }
    void DistanceToPlayer()
    {
        float dist = Vector3.Distance(transform.position, pcInstance.transform.position);
        //print("Distance: " + dist);
        if(dist <= rangeThreshold)
        {
            if (dist <= attackDistanceThreshold && !attackRange)
                enemyStateControl = enemyStates.RegularState; //Instantly changing to chasing when the player is too close to the enemy
            else
            {
                if (!attackRange && !idleRangeSpooling)
                {
                    if (CO_IdleRange != null)
                    {
                        StopCoroutine(CO_IdleRange);
                        idleRangeSpooling = false;
                    }
                    CO_IdleRange = StartCoroutine(RangeBoolean(detectionTime, true, enemyStates.RegularState));
                }
            }
        }
        #region
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
        float time = 0;
        while(time < spoolDuration)
        {
            time += Time.deltaTime;
            yield return null;
        }

        attackRange = condition;
        enemyStateControl = state;
        idleRangeSpooling = false;
    }

    public void ScannedBehaviors()
    {
        if(!detectionParticles.isEmitting && isChasing)
        {
            if (CO_DetectionParticles != null)
                detectionParticles.Stop();
            
            CO_DetectionParticles = StartCoroutine(DetectionParticlesEmission(2.0f));
        }
    }
    public void PulseScannedBehaviors()
    {
        if (CO_StunnedPhase != null)
            StopCoroutine(CO_StunnedPhase);
        
        CO_StunnedPhase = StartCoroutine(StunnedPhase(1.5f));
    }
    public void SonarBeamScannedBehaviors()
    {
        masterVisibilityToggle = true; Dissolve(false, 0.5f);
    }
    public void SonarBeamExitBehaviors()
    {
        print("--- SonarBeamExitBehaviors Triggered ---");

        if (CO_RevealSpool != null)
            StopCoroutine(CO_RevealSpool);

        CO_RevealSpool = StartCoroutine(RevealDelay(VisibilityDelayTime));
    }

    IEnumerator DetectionParticlesEmission(float time)
    {
        detectionParticles.Play(); //print(gameObject.name + "VFX Playing");
        float timer = 0;

        while(timer < time)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        detectionParticles.Stop();
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
        enemyStateControl = enemyStates.RegularState;
    }

    // --- MATERIAL FUNCTIONS ---

    void MaterialDefault()
    {
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
}
