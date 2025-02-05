using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPrintScript : MonoBehaviour
{
    public TMP_Text textMeshPro;
    public float letterDelay = 0.1f; // Delay between each letter

    private string originalText;
    private string displayedText = "";
    private float timer = 0f;
    private int currentLetterIndex = 0;
    private bool isFinished = false;

    void Start()
    {
        originalText = textMeshPro.text;
        textMeshPro.text = ""; // Clear text to start with
    }

    void Update()
    {
        if (!isFinished)
        {
            timer += Time.deltaTime;
            if (timer >= letterDelay && currentLetterIndex < originalText.Length)
            {
                displayedText += originalText[currentLetterIndex];
                textMeshPro.text = displayedText;
                currentLetterIndex++;
                timer = 0f;
            }
            else if (currentLetterIndex >= originalText.Length)
            {
                isFinished = true;
                // Optionally, add any additional behavior when the text finishes printing
            }
        }
        else
        {
            // Reset variables to start over
            timer = 0f;
            currentLetterIndex = 0;
            displayedText = "";
            isFinished = false;
        }
    }
}
