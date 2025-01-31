using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoNotDestroyOnLoad : MonoBehaviour
{
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Menu"
            || SceneManager.GetActiveScene().name == "IntroCutScene")
            DontDestroyOnLoad(gameObject);
        else
            Destroy(this.gameObject);
    }
    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "Menu"
           && SceneManager.GetActiveScene().name != "IntroCutScene")
            Destroy(this.gameObject);
    }
}
