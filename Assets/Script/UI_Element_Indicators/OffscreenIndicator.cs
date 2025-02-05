using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffscreenIndicator : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform target;
    [SerializeField] private Generators targetGen;
    [SerializeField] Image GenPower;
    [Range(0, 0.5f)] public float screenBorder = 0.1f;

    Camera mainCamera;

    [SerializeField] GameObject indicator;
    RectTransform indicatorRectTransform;

    void Start()
    {
        mainCamera = Camera.main;

        targetGen = target.GetComponent<Generators>();
        GenPower.GetComponent<Image>();

        indicatorRectTransform = GetComponent<RectTransform>();

        indicator.SetActive(false);
    }

    void Update()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(target.position);

        float fillLerp = Mathf.InverseLerp(0, targetGen.GeneratorMaxAmount, targetGen.GeneratorPowerAmount);
        GenPower.fillAmount = fillLerp;

        if (screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1 || screenPoint.z < 0)
        {
            Vector3 indicatorPosition = GetIndicatorPosition(screenPoint);
            indicatorRectTransform.position = indicatorPosition;
            indicator.SetActive(true);
        }
        else
        {
            indicator.SetActive(false);
        }
    }

    Vector3 GetIndicatorPosition(Vector3 screenPoint)
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float offsetX = Mathf.Clamp01(screenPoint.x) - 0.5f;
        float offsetY = Mathf.Clamp01(screenPoint.y) - 0.5f;

        offsetX *= 1 - screenBorder * 2;
        offsetY *= 1 - screenBorder * 2;

        float indicatorX = Mathf.Clamp(offsetX, -0.5f, 0.5f) * screenWidth + screenWidth * 0.5f;
        float indicatorY = Mathf.Clamp(offsetY, -0.5f, 0.5f) * screenHeight + screenHeight * 0.5f;
        Vector3 indicatorPosition = new Vector3(indicatorX, indicatorY, 0);

        return indicatorPosition;
    }
}
 