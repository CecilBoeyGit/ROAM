using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarCollider : MonoBehaviour
{

    [Header("--- REFERENCES ---")]
    [SerializeField] FieldOfView fovController;

    private void Start()
    {
        fovController.GetComponent<FieldOfView>();
    }
    private void OnTriggerExit(Collider other)
    {
        if(fovController.visibleTargets.Count != 0)
        {
            foreach(Transform child in fovController.visibleTargets)
            {
                if (child.gameObject.Equals(other.gameObject))
                    child.gameObject.GetComponent<EnemyBehavior>().SonarBeamExitBehaviors();
            }
        }
    }
}
