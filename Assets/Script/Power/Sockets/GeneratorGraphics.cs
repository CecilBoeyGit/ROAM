using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorGraphics : MonoBehaviour
{
    [SerializeField] Generators generatorMain;

    [Header("--- GRAPHICS ---")]
    [SerializeField] GameObject warningStrips;
    [SerializeField] Color Color_Online, Color_Offline;
    Material M_WarningStrips;

    void DefaultState()
    {
        M_WarningStrips.SetColor("_EmissiveColor", Color_Online);
        M_WarningStrips.SetFloat("_AtlasIndex", 0);
        M_WarningStrips.SetFloat("_Flicker", 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        if(warningStrips != null)
            M_WarningStrips = warningStrips.GetComponent<MeshRenderer>().material;

        generatorMain.GetComponent<Generators>();

        DefaultState();
    }

    // Update is called once per frame
    void Update()
    {
        if(generatorMain.GeneratorPowerAmount <= 0)
        {
            M_WarningStrips.SetColor("_EmissiveColor", Color_Offline);
            M_WarningStrips.SetFloat("_AtlasIndex", 1);
            M_WarningStrips.SetFloat("_Flicker", 1);
        }
        else
        {
            M_WarningStrips.SetColor("_EmissiveColor", Color_Online);
            M_WarningStrips.SetFloat("_AtlasIndex", 0);
            M_WarningStrips.SetFloat("_Flicker", 0);
        }
    }
}
