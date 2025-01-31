using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimeManager : MonoBehaviour
{

    PlayerController playerInstance;

    void Start()
    {
        playerInstance = PlayerController.instance;

        Time.timeScale = 1.0f;
    }

    public void SlowMotion()
    {
        playerInstance.PlayerConstrained = true;
        Time.timeScale = 0.3f;
    }
}
