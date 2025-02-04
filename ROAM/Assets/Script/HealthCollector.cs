using UnityEngine;

public class HealthCollector : MonoBehaviour
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
            float playerHealth = playerController.healthPoint;

            if (MetricManagerScript.instance != null)
            {
                MetricManagerScript.instance.LogString("PlayerHealth", playerHealth.ToString());
            }
        }
    }
}
