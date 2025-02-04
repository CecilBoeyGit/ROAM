using UnityEngine;
using UnityEngine.UI;

public class GeneratorPointerScript : MonoBehaviour
{
    public Transform targetObject; // Reference to the object in the scene that the UI element should point to
    public Transform player; // Reference to the player object
    public RectTransform uiElement; // Reference to the RectTransform component of the UI element
    public RectTransform panel; // Reference to the RectTransform component of the panel
    public float maxScale = 3f; // Maximum scale of the UI element
    public float minScale = 0.5f; // Minimum scale of the UI element
    public float minDistance = 0.1f; // Minimum distance to prevent division by zero

    private void Start()
    {
        //gameObject.SetActive(false);
    }
    void Update()
    {
        // Check if the target object, player, UI element, and the panel are all assigned
        if (targetObject != null && player != null && uiElement != null && panel != null)
        {
            // Convert the target object's position from world space to screen space
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetObject.position);

            // Calculate the panel boundaries in screen space
            Vector3[] panelCorners = new Vector3[4];
            panel.GetWorldCorners(panelCorners);
            Vector2 panelMin = panelCorners[0];
            Vector2 panelMax = panelCorners[2];

            // Calculate the clamped screen position within the panel boundaries with offset
            float clampedX = Mathf.Clamp(screenPosition.x, panelMin.x, panelMax.x);
            float clampedY = Mathf.Clamp(screenPosition.y, panelMin.y, panelMax.y);
            Vector3 clampedScreenPosition = new Vector3(clampedX, clampedY, screenPosition.z);

            // Set the position of the UI element to the clamped screen position
            uiElement.position = clampedScreenPosition;

            // Calculate the distance between the player and the target object with a minimum distance offset
            float distance = Mathf.Max(Vector3.Distance(targetObject.position, player.position), minDistance);


            // Map the distance to the scale range (minScale to maxScale)
            float scaleFactor = Mathf.Lerp(minScale, maxScale, 1f / distance);

            // Set the scale of the UI element
            uiElement.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
        }
    }
}
