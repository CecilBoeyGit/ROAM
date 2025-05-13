using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GlobalLightingManager : MonoBehaviour
{

    [Header("--- RUNTIME REFERENCES ---")]
    [SerializeField] GameObject Generator01Lights, Generator02Lights;
    [SerializeField] Generators Generator01, Generator02;
    float Gen01Power, Gen02Power;

    bool ForceLights = false;

    public static GlobalLightingManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    private void OnEnable()
    {
        IntegrityManager.RundownSuccessAction += ForceAllLightsOn;
        IntegrityManager.MeterNull += ForceAllLightsOff;
    }
    private void OnDisable()
    {
        IntegrityManager.RundownSuccessAction -= ForceAllLightsOn;
        IntegrityManager.MeterNull -= ForceAllLightsOff;
    }
    void InitSettings()
    {
        ForceLights = false;

        if (Generator01Lights == null || Generator02Lights == null)
        {
            Debug.Log("NO GENERATOR LIGHTINGS!!!");
            return;
        }

        Generator01Lights.SetActive(true);
        Generator01Lights.SetActive(true);

        var generator01 = FindObjectsOfType<Generators>().FirstOrDefault(g => g.GeneratorID == 1);
        if(generator01 != null)
            Generator01 = generator01.GetComponent<Generators>();
        var generator02 = FindObjectsOfType<Generators>().FirstOrDefault(g => g.GeneratorID == 2);
        if (generator02 != null)
            Generator02 = generator02.GetComponent<Generators>();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitSettings();
    }

    // Update is called once per frame
    void Update()
    {
        if (ForceLights)
            return;

        if (Generator01 != null)
        {
            Gen01Power = Generator01.GeneratorPowerAmount;
            if (Gen01Power <= 0)
            {
                Generator01Lights.SetActive(false);
            }
            else
            {
                Generator01Lights.SetActive(true);
            }
        }
        if (Generator02 != null)
        {
            Gen02Power = Generator02.GeneratorPowerAmount;
            if (Gen02Power <= 0)
            {
                Generator02Lights.SetActive(false);
            }
            else
            {
                Generator02Lights.SetActive(true);
            }
        }
    }

    public void ForceAllLightsOn()
    {
        ForceLights = true;
        if (Generator01 != null)
            Generator01Lights.SetActive(true);
        if (Generator02 != null)
            Generator02Lights.SetActive(true);
    }
    public void ForceAllLightsOff()
    {
        ForceLights = true;
        if (Generator01 != null)
            Generator01Lights.SetActive(false);
        if (Generator02 != null)
            Generator02Lights.SetActive(false);
    }
}
