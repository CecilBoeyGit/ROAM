using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleReset : MonoBehaviour
{
    private void OnEnable()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
    }
}
