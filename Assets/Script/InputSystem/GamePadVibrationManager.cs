using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePadVibrationManager : MonoBehaviour
{

    private Gamepad gamePad;

    public static GamePadVibrationManager instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        instance = this;
    }

    public void Rumble(float duration, float lowFreq, float highFreq)
    {
        gamePad = Gamepad.current;

        if (gamePad == null)
            return;

        if (CO_Rumbling != null)
            StopCoroutine(Rumbling(0,0,0));
        CO_Rumbling = StartCoroutine(Rumbling(duration, lowFreq, highFreq));
    }

    Coroutine CO_Rumbling;
    IEnumerator Rumbling(float duration, float lowFreq, float highFreq)
    {
        float time = 0;
        while(time < duration)
        {
            time += Time.deltaTime;
            gamePad.SetMotorSpeeds(lowFreq, highFreq);

            yield return null;
        }

        gamePad.SetMotorSpeeds(0, 0);
    }
}
