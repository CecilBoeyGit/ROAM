using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScrollText : MonoBehaviour
{
    public float scrollSpeed = 10f; // Speed of scrolling

    private TMP_InputField inputField; // Reference to the TMP_InputField component

    // Start is called before the first frame update
    void Start()
    {
        // Get the reference to the TMP_InputField component
        inputField = GetComponent<TMP_InputField>();

        // Start scrolling the text
        StartScrolling();
    }

    // Method to start scrolling the text
    private void StartScrolling()
    {
        // Call ScrollTextDown() method repeatedly after a delay
        InvokeRepeating(nameof(ScrollTextDown), 0f, 0.1f);
    }

    // Method to scroll the text downwards
    private void ScrollTextDown()
    {
        // Move the text upwards
        inputField.textComponent.rectTransform.anchoredPosition += new Vector2(0f, scrollSpeed * Time.deltaTime);

        // If the text has scrolled beyond the top, reset it to the bottom
        if (inputField.textComponent.rectTransform.anchoredPosition.y > inputField.textComponent.rectTransform.rect.height)
        {
            // Reset the position of the text
            inputField.textComponent.rectTransform.anchoredPosition = new Vector2(0f, -inputField.textComponent.rectTransform.rect.height);
        }
    }
}

