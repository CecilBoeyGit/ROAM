using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableCursor : MonoBehaviour
{
    private void OnEnable()
    {   
        Cursor.visible = false;
    }
    void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.C))
            Cursor.visible = true;
        else
            Cursor.visible = false;
    }
}
