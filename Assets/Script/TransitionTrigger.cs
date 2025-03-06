using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class TransitionTrigger : MonoBehaviour
{
    public MusicTransitionScript audioTransition;
    public string objectTag;

    [SerializeField] List<GameObject> EnemyStayed = new List<GameObject>();

    Coroutine CO_CheckForEnemies;

    private void OnDisable()
    {
        if (CO_CheckForEnemies != null)
            StopCoroutine(CO_CheckForEnemies);
    }

    private void Start()
    {
        if (CO_CheckForEnemies != null)
            StopCoroutine(CO_CheckForEnemies);
        else
            CO_CheckForEnemies = StartCoroutine(CheckForEnemies(0.1f));
    }

    IEnumerator CheckForEnemies(float cooldown)
    {
        while(true)
        {
            EnemyStayed = FindObjectsOfType<EnemyBehavior>()
    .Where(enemy => enemy.enemyStateControl == EnemyBehavior.enemyStates.ChaseState
    || enemy.enemyStateControl == EnemyBehavior.enemyStates.AttackState)
    .Select(enemy => enemy.gameObject)
    .ToList();

            yield return new WaitForSeconds(cooldown);
        }
    }

    private void LateUpdate()
    {
        if (EnemyStayed.Count != 0)
        {
            audioTransition.StartTransition(true);

            //print("Combat BGM");
        }
        else
        {
            audioTransition.StartTransition(false);

            //print("Unsettling BGM");
        }
    }
}
