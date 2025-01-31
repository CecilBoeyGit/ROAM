using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetricsVisualizer : MonoBehaviour
{

    [SerializeField] MetricsHolder SO_MetricsHolder;
    [SerializeField] GameObject MetricBallRed, MetricBallYellow;
    private List<Vector3> SonarScans = new List<Vector3>();
    private List<Vector3> SonarPulses = new List<Vector3>();

    void OnEnable()
    {
        SonarScans = SO_MetricsHolder.SonarScanClickedPos;
        SonarPulses = SO_MetricsHolder.SonarPulseTriggeredPos;
    }
    private void Start()
    {
        foreach (Vector3 dataPoint in SonarScans)
        {
            Instantiate(MetricBallRed, dataPoint, Quaternion.identity);
        }
        foreach (Vector3 dataPoint in SonarPulses)
        {
            Instantiate(MetricBallYellow, dataPoint, Quaternion.identity);
        }
    }
    void OnDrawGizmos()
    {
        foreach (Vector3 dataPoint in SonarScans)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(dataPoint, 2f);
        }
        foreach (Vector3 dataPoint in SonarPulses)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(dataPoint, 2f);
        }
    }
}
