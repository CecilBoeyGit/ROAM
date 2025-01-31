using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Control : MonoBehaviour
{
    public float delayBetweenJumps = 1f; // Delay between opacity jumps in seconds

    private Image image;

    void Start()
    {
        // Get the Image component attached to this GameObject
        image = GetComponent<Image>();

        // Start the opacity jump coroutine
        StartCoroutine(JumpOpacity());
    }

    IEnumerator JumpOpacity()
    {
        while (true)
        {
            // Set opacity to 10%
            SetOpacity(0.1f);
            yield return new WaitForSeconds(delayBetweenJumps);

            // Set opacity to 20%
            SetOpacity(0.2f);
            yield return new WaitForSeconds(delayBetweenJumps);
        }
    }

    // Set the opacity of the image
    void SetOpacity(float opacity)
    {
        Color color = image.color;
        color.a = opacity;
        image.color = color;
    }
}
