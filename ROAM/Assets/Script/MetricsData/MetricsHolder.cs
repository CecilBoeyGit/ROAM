using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MetricsDataHolder", menuName = "Custom/AbilitiesClickedWorldSpace")]
public class MetricsHolder : ScriptableObject
{

    public List<Vector3> SonarScanClickedPos = new List<Vector3>();
    public List<Vector3> SonarPulseTriggeredPos = new List<Vector3>();

}
