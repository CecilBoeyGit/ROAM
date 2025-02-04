using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;
using System.Collections.Generic;

public class TransitionTrigger : MonoBehaviour
{
    public MusicTransitionScript audioTransition;
    private bool isTransitionToSource2 = false;
    public string objectTag;

    [SerializeField] List<GameObject> EnemyStayed = new List<GameObject>();
    bool InCombat = false;

    AnalogGlitchVolume _analogVolume;

    private void Start()
    {
        var volumeStack = VolumeManager.instance.stack;
        _analogVolume = volumeStack.GetComponent<AnalogGlitchVolume>();
    }

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 15);

        EnemyStayed.Clear();

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag(objectTag))
            {
                GameObject enemyObject = collider.gameObject;
                EnemyStayed.Add(enemyObject);
            }
        }  
    }

    private void LateUpdate()
    {
        if (EnemyStayed.Count != 0 && !InCombat)
        {
            InCombat = true;
            isTransitionToSource2 = true;
            audioTransition.StartTransition(true);

            print("Combat BGM");
        }
        else if (EnemyStayed.Count == 0 && InCombat)
        {
            InCombat = false;
            isTransitionToSource2 = false;
            audioTransition.StartTransition(false);

            print("Unsettling BGM");
        }
    }
}
