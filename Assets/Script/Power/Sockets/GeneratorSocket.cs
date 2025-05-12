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
    [SerializeField] Image AmountIndicatorUI;
    [SerializeField] Image GeneratorGraphicUI;
    //[SerializeField] Image BarImage;
    //Material BarMaterial;
    Material GeneratorIconMaterial;

    [Header("--- GRAPHICS ---")]
    [SerializeField] GameObject SocketCanvas;

    GeneratorsSFX genSFXInstance;

    protected override void Start()
    {
        base.Start();

        genSFXInstance = GeneratorsSFX.instance;

        generatorParent = GetComponentInParent<Generators>();
        AmountIndicatorUI.GetComponent<Image>();
        GeneratorGraphicUI.GetComponentInParent<Image>();
        GeneratorIconMaterial = GeneratorGraphicUI.material;
        
        GeneratorGraphicUI.color = Color.white;
        if (GeneratorIconMaterial != null)
            GeneratorIconMaterial.SetFloat("_UseFlicker", 0);

        //BarMaterial = BarImage.GetComponent<Image>().material;
    }
    protected override void Update()
    {
        base.Update();

        float GenLerp = Mathf.InverseLerp(0, generatorParent.GeneratorMaxAmount, generatorParent.GeneratorPowerAmount);
        //BarMaterial.SetFloat("_SliceCoverage", GenLerp);
        if(AmountIndicatorUI != null)
            AmountIndicatorUI.fillAmount = GenLerp;

        if (GeneratorIconMaterial == null)
            return;
        if (GenLerp <= 0f)
        {
            GeneratorGraphicUI.color = Color.red;
            if (GeneratorIconMaterial != null)
            {
                GeneratorIconMaterial.SetFloat("_UseFlicker", 1);
                GeneratorIconMaterial.SetFloat("_FlickerFrequency", 8);
            }
        }
        else
        {
            GeneratorGraphicUI.color = Color.white;
            if (GeneratorIconMaterial != null)
                GeneratorIconMaterial.SetFloat("_UseFlicker", 0);
        }
    }
    public override void PlayerInZoneActions()
    {
        if (_InputSub.InteractInput)
        {
            if (!powerCoreChild.isActiveAndEnabled) //If the generator has no PowerCore charging
            {
                if (PRMInstance.currentPowerCore != null && PRMInstance.isEquiped) //If the player is equiped with a PowerCore
                {
                    powerCoreChild.gameObject.SetActive(true);
                    ads.clip = adcp[1];
                    ads.Play();
                    PRMInstance.currentPowerCore = null;
                    generatorParent.isCharging = true;

                    SocketCanvas.SetActive(false);

                    if(genSFXInstance != null)
                        genSFXInstance.PlayOnAudio();

                    if (CO_GeneratorCharing != null)
                        StopCoroutine(CO_GeneratorCharing);
                    CO_GeneratorCharing = StartCoroutine(PowerCoreCharging(2.0f));
                }
            }
        }     
    }

    Coroutine CO_GeneratorCharing;
    IEnumerator PowerCoreCharging(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            //float normalizedLerp = Mathf.InverseLerp(0, duration, time);
            generatorParent.GeneratorPowerAmount += Time.deltaTime * generatorParent.PowerIncrementMultiplier;

            //Placeholder for animations and effects while the generator is charging

            yield return null;
        }

        generatorParent.isCharging = false;
        ads.clip = adcp[0];
        ads.Play();
        powerCoreChild.gameObject.SetActive(false);
        SocketCanvas.SetActive(true);
    }

    public override void UIGroupManager()
    {
        if (generatorParent.isCharging)
        {
            UI_Group.SetActive(false);
            return;
        }
        else
        {
            base.UIGroupManager();
        }
    }

    /*private void HeldDownActions()
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
    }*/
}
