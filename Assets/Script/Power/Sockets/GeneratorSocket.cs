using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorSocket : PowerSockets
{
    Generators generatorParent;
    [SerializeField] bool isHeldDown = false;
    float holdThreshold = 0.1f;
    float holdTime = 0f;

    [Header("--- UI ---")]
    [SerializeField] Image BarImage;
    Material BarMaterial;

    protected override void Start()
    {
        base.Start();

        generatorParent = GetComponentInParent<Generators>();
        BarMaterial = BarImage.GetComponent<Image>().material;
    }
    protected override void Update()
    {
        base.Update();
        if(powerCoreChild.isActiveAndEnabled)
        {
            if (powerCoreChild._powerAmount <= 0)
                generatorParent.isCharging = false;
        }

        float GenLerp = Mathf.InverseLerp(0, generatorParent.GeneratorMaxAmount, generatorParent.GeneratorPowerAmount);
        BarMaterial.SetFloat("_SliceCoverage", GenLerp);
    }
    public override void PlayerInZoneActions()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            print("E Pressed");
            isHeldDown = true;
            holdTime = Time.time;
        }
        if (Input.GetKeyUp(KeyCode.E)) // When the key is pressed
        {
            if (isHeldDown)
            {
                print("E Released");
                if (powerCoreChild.isActiveAndEnabled && PRMInstance.currentPowerCore == null)
                {
                    if (powerCoreChild.isCharging)
                    {
                        PRMInstance.currentPowerCore = powerCoreChild;
                        PRMInstance.powerAmount = powerCoreChild._powerAmount;
                        powerCoreChild.Equipped();
                        ads.clip = adcp[0];
                        ads.Play();
                        powerCoreChild.gameObject.SetActive(false);
                        generatorParent.isCharging = false;
                    }
                }
                else if (!powerCoreChild.isActiveAndEnabled && PRMInstance.currentPowerCore != null)
                {
                    if (PRMInstance.currentPowerCore != null && PRMInstance.currentPowerCore.isEquiped)
                    {
                        powerCoreChild.gameObject.SetActive(true);
                        powerCoreChild.Charging();
                        ads.clip = adcp[1];
                        ads.Play();
                        powerCoreChild._powerAmount = PRMInstance.powerAmount;
                        PRMInstance.currentPowerCore = null;
                        generatorParent.isCharging = true;
                    }
                }
            }
            else
            {
                generatorParent.isCharging = false;
            }
        }     
    }

    private void HeldDownActions()
    {
        if (Input.GetKey(KeyCode.E) && Time.time - holdTime >= holdThreshold) // When the key is held down
        {
            print("E Held");
            isHeldDown = false;
            if (PRMInstance.currentPowerCore != null && PRMInstance.currentPowerCore.isEquiped && PRMInstance.powerAmount > 0)
            {
                generatorParent.isCharging = true;
                PRMInstance.powerAmount -= Time.deltaTime * PRMInstance.currentPowerCore.ChargingDecrement;
            }
        }
    }
}
