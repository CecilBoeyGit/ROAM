using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewSonar : FieldOfView
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    protected override void LateUpdate()
    {
        base.LateUpdate();
    }
    public override void TriggerTarget()
    {
        foreach (Transform child in visibleTargets)
        {
            child.GetComponent<EnemyBehavior>().ScannedBehaviors();
            child.GetComponent<EnemyBehavior>().SonarBeamScannedBehaviors();
        }
    }
}
