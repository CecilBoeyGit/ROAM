using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    [SerializeField] public string fullText = "Hello, this is a typewriter effect!";
    public float startDelay = 2f;
    public float typingSpeed = 0.05f;
    public bool IsTextFinished { get; private set; } = false; // Flag to track completion

    private void Start()
    {
        if (textMeshPro != null)
        {
            textMeshPro.text = ""; // Clear text initially
            StartCoroutine(TypeText());
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component is not assigned!");
        }
    }

    private IEnumerator TypeText()
    {
        yield return new WaitForSeconds(startDelay); // Initial delay

        for (int i = 0; i <= fullText.Length; i++)
        {
            textMeshPro.text = fullText.Substring(0, i);
            yield return new WaitForSeconds(typingSpeed);
        }

        IsTextFinished = true; // Mark text as finished
    }
}
