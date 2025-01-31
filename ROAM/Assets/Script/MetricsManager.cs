using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetricsManager : MonoBehaviour
{
    PlayerController playerController;

    void Start()
    {
        MetricsEvents.OnDataCollect += this.CollectData;
        playerController = FindObjectOfType<PlayerController>();
    }
    void CollectData()
    {
        if (playerController != null)
        {
            Vector3 playerTransform = playerController.transform.position;
            string playerTransformString = ("World Position: " + playerTransform);

            if (MetricManagerScript.instance != null)
            {
                MetricManagerScript.instance.LogString("Player Position", playerTransformString);
            }
        }
    }
}
