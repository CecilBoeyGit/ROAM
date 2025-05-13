using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public VideoPlayer videoPlayer;

    public Volume interfaceVolume;

    InputSubscriptions _InputSub;
    PlayerController _PlayerInstance;
    LetterByLetterWithPause _LetterPrintingInstance;

    private void Start()
    {
        _InputSub = InputSubscriptions.instance;
        _PlayerInstance = PlayerController.instance;
        _LetterPrintingInstance = LetterByLetterWithPause.Instance;

        pauseMenuUI.SetActive(false);

        if(videoPlayer != null)
            videoPlayer.Play();

        interfaceVolume.weight = 0;
    }

    public void Resume()
    {
        TogglePauseMenu();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        if (_InputSub.MenuInput)
            TogglePauseMenu();
    }

    private void TogglePauseMenu()
    {
        if (_LetterPrintingInstance.isPrintingText)
            return;

        if (pauseMenuUI.activeSelf)
        {
            _PlayerInstance.PlayerConstrained = false;
            interfaceVolume.weight = 0;
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            AudioListener.pause = false;

            if (videoPlayer != null)
                videoPlayer.Play();
        }
        else
        {
            _PlayerInstance.PlayerConstrained = true;
            interfaceVolume.weight = 1;
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            AudioListener.pause = true;

            if (videoPlayer != null)
                videoPlayer.Pause();
        }
    }
}
