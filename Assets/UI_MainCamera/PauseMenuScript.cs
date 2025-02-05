using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PauseMenuScript : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public VideoPlayer videoPlayer;

    private void Start()
    {
        pauseMenuUI.SetActive(false);
        videoPlayer.Play();
    }

    public void Resume()
    {
        TogglePauseMenu();
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    private void TogglePauseMenu()
    {
        if (pauseMenuUI.activeSelf)
        {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            AudioListener.pause = false;
            videoPlayer.Play();
        }
        else
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            AudioListener.pause = true;
            videoPlayer.Pause();
        }
    }
}
