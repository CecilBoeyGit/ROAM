using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeEmissionHue : MonoBehaviour
{
    private Material material; // Reference to the material
    public Elevatorscript elevatorfencescript;
    public float targetHue = 0.5f; // Target hue value (0 to 1)

    void Start()
    {
        // Ensure there's a Renderer component attached
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("No Renderer component found on this GameObject!");
            return;
        }

        // Get the material attached to the renderer
        material = renderer.material;

        // Ensure the material has emission enabled
        material.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        if (elevatorfencescript.shouldShiftHue)
        {
            // Get the current emission color
            Color currentColor = material.GetColor("_EmissionColor");

            // Convert the current color to HSV
            Color.RGBToHSV(currentColor, out float currentHue, out float currentSaturation, out float currentValue);

            // Set the new hue value
            currentHue = targetHue;

            // Convert the new HSV color back to RGB
            Color newColor = Color.HSVToRGB(currentHue, currentSaturation, currentValue);

            // Set the new emission color
            material.SetColor("_EmissionColor", newColor);
        }
    }
}
