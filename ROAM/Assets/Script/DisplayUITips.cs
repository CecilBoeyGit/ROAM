using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayUITips : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateText(string newText)
    {
        if (textMeshPro != null)
        {
            textMeshPro.text = newText;
        }
        else
        {
            Debug.LogError("TextMeshPro component not found!");
        }
    }
    public void ClearText()
    {
        textMeshPro.text = "";
    }
}
