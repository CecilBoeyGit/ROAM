using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadSceneSimple : MonoBehaviour
{
    [Tooltip("Pressing the combination of these keys will reload all currently open scenes.")]
    [SerializeField]
    private List<KeyCode> KeyCodes = new()
    {
        KeyCode.R
    };

    private void Update()
    {
        bool bAreKeyCodesPressed = true;
        foreach (KeyCode key in KeyCodes)
        {
            bAreKeyCodesPressed &= Input.GetKey(key);
        }

        if (bAreKeyCodesPressed)
        {
            ReloadCurrentScene();
        }
    }

    void ReloadCurrentScene()
    {
        // Get the name of the current scene
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Reload the current scene by its name
        SceneManager.LoadScene(currentSceneName);
    }
}
