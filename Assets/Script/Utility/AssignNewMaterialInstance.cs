using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssignNewMaterialInstance : MonoBehaviour
{
    private Image img;

    private void Awake()
    {
        img = GetComponent<Image>();

        if (img != null && img.material != null)
            img.material = new Material(img.material);
    }
}
