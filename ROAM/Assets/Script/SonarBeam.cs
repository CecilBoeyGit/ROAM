using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SonarBeam : MonoBehaviour
{

    [Header("--- VARIABLES ---")]
    [SerializeField] int sonarBeamID;
    [SerializeField] float scannerRadiusMax = 10f;

    [Header("--- REFERENCES ---")]
    [SerializeField] FieldOfView fovController;
    [SerializeField] Generators generatorParent;
    [SerializeField] SphereCollider colliderZone;

    [Header("--- DEBUG ---")]
    [SerializeField] bool UseDebug = false;
    [SerializeField] bool DEBUG_SonarActivate;

    float timer = 0f;
    bool sonarActivate = false;
    private void OnEnable()
    {
        Generators.PowerAmountThreshold += TriggerSonarBeam;
    }
    void Start()
    {
        fovController = GetComponent<FieldOfView>();    
        fovController.viewRadius = 0;

        generatorParent = transform.parent.parent.gameObject.GetComponent<Generators>();
        sonarBeamID = generatorParent.GeneratorID;
        colliderZone.GetComponent<SphereCollider>();
    }
    private void Update()
    { 
        if(UseDebug)
            TriggerSonarBeam(sonarBeamID, DEBUG_SonarActivate);

        FOVBehaviors(2.0f);
        ColliderZoneBehaviors();
    }

    public void TriggerSonarBeam(int GenID, bool TriggerBoolean)
    {
        if(sonarBeamID == GenID)
            sonarActivate = TriggerBoolean;
    }
    void FOVBehaviors(float activateTime)
    {
        if(sonarActivate)
        {
            if(timer < activateTime)
                timer += Time.deltaTime;
        }
        else
        {
            if(timer > 0)
                timer -= Time.deltaTime;
        }

        float lerpVal = Mathf.InverseLerp(0, activateTime, timer);
        fovController.viewRadius = Mathf.Lerp(0, scannerRadiusMax, lerpVal);
    }
    void ColliderZoneBehaviors()
    {
        colliderZone.radius = fovController.viewRadius;
    }
}
