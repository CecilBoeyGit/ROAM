using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ProximityTrigger : MonoBehaviour
{

    [SerializeField] ParticleSystem ps;
    [SerializeField] BoxCollider boxCol;
    [SerializeField] AudioSource ads;
    [SerializeField] float triggerSize = 5;
    [SerializeField] float coolDownTime = 8;
    bool coolDownCompleted = true;
    [SerializeField] float probabilityPercentile = 2;

    Coroutine CO_EnterCoolDown;

    private void OnEnable()
    {
        boxCol = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        ps.GetComponent<ParticleSystem>();
        boxCol = GetComponent<BoxCollider>();
        ads.GetComponent<AudioSource>();

        if (boxCol != null)
            boxCol.size = new Vector3(triggerSize, 5, triggerSize);

        coolDownCompleted = true;
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            if (boxCol != null)
                boxCol.size = new Vector3(triggerSize, 5, triggerSize);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!coolDownCompleted)
            return;

        if (other.gameObject.tag.Equals("Player") || other.gameObject.tag.Equals("Enemy"))
        {
            float prob = Mathf.FloorToInt(Random.Range(0, probabilityPercentile));
            if (prob == 1)
                PlayerParticle();

            print(prob);
        }
    }

    void PlayerParticle()
    {
        coolDownCompleted = false;

        if (CO_EnterCoolDown != null)
            StopCoroutine(CO_EnterCoolDown);

        CO_EnterCoolDown = StartCoroutine(StartCoolDown(coolDownTime));
    }

    IEnumerator StartCoolDown(float time)
    {
        float timer = 0;

        if (ps != null)
            ps.Play();
        if (ads != null)
            ads.Play();

        while(timer < time)
        {
            timer += Time.deltaTime;

            yield return null;
        }

        coolDownCompleted = true;
    }
}
