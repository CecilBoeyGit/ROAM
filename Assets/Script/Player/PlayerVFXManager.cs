using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFXManager : MonoBehaviour
{

    [SerializeField] ParticleSystem ElectricityVFX;
    ParticleSystemRenderer psRenderer;
    Material electricityMat;
    [SerializeField, ColorUsage(true, true)] Color defaultElectricityColor, criticalElectricityColor;
    [SerializeField] List<AudioClip> adcp = new List<AudioClip>();
    AudioSource ads;

    PlayerController playerInstance;

    // Start is called before the first frame update
    void Start()
    {
        isPlaying = false;

        ElectricityVFX.GetComponent<ParticleSystem>();
        psRenderer = ElectricityVFX.GetComponent<ParticleSystemRenderer>();
        if(psRenderer != null)
        {
            electricityMat = psRenderer.material;
        }
        ads = GetComponent<AudioSource>();

        playerInstance = PlayerController.instance;

        ElectricityVFX.gameObject.SetActive(false);
    }

    bool isPlaying = false;

    // Update is called once per frame
    void Update()
    {
        if (playerInstance == null)
            return;

        if(playerInstance.healthPoint > 100)
        {
            ElectricityVFX.gameObject.SetActive(false);
        }
        else if(playerInstance.healthPoint == 100)
        {
            var particleEmission = ElectricityVFX.emission;
            particleEmission.rateOverTime = 5;
            electricityMat.SetColor("_EmissionColor", defaultElectricityColor);

            ElectricityVFX.gameObject.SetActive(true);
        }
        else if(playerInstance.healthPoint == 50)
        {
            var particleEmission = ElectricityVFX.emission;
            particleEmission.rateOverTime = 20;
            electricityMat.SetColor("_EmissionColor", criticalElectricityColor);

            if (isPlaying)
                return;

            ads.clip = adcp[0];
            ads.Play();

            isPlaying = true;
        }
    }
}
