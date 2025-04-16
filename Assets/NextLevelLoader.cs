using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelLoader : MonoBehaviour
{
    public float holdDuration = 2f; // Time required to hold the button
    private float holdTimer = 0f;

    void Update()
    {
        // Check if B (joystick button 1) is being held
        if (Input.GetKey(KeyCode.JoystickButton1))
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= holdDuration)
            {
                LoadNextLevel();
                holdTimer = 0f; // prevent multiple loads
            }
        }
        else
        {
            holdTimer = 0f; // Reset if released early
        }
    }

    void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more scenes to load!");
        }
    }
}
