using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Reflection;

public class CalistoTutorialScript : MonoBehaviour
{

    public TextMeshProUGUI ObjectTypeText;
    public TextMeshProUGUI TutorialText1;
    public TextMeshProUGUI TutorialText2;
    public TextMeshProUGUI TutorialText3;
   

    public string[] Category;
    public string[] Tutorial1;
    public string[] Tutorial2;
    public string[] Tutorial3;

    public List<VideoClip> videoClips = new List<VideoClip>(); 
    //public RenderTexture[] renderTextures; 
    [SerializeField] VideoPlayer videoPlayer1;
    [SerializeField] VideoPlayer videoPlayer2;
    [SerializeField] VideoPlayer videoPlayer3;

    [SerializeField] string methodName;
   

   [SerializeField] GameObject CalistoUI;

    public VideoPlayer videoPlayer;

    // Update is called once per frame
    void Start()
    {
        videoPlayer1 = GetComponent<VideoPlayer>();
        videoPlayer2 = GetComponent<VideoPlayer>();
        videoPlayer3 = GetComponent<VideoPlayer>();
        InvokeMethodByName(methodName);
    }

    void InvokeMethodByName(string methodName)
    {
        MethodInfo method = GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        if (method != null)
        {
            method.Invoke(this, null);
        }
        else
        {
            Debug.LogError($"Method {methodName} not found in {GetType()}");
        }
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            Time.timeScale = 0f;
            videoPlayer.Pause();
        }
    }
    

    public void Resume()
    {
        CalistoUI.SetActive(false);
        Time.timeScale = 1f;
    }
    void Countdown()
    {
        CalistoUI.SetActive(true);
        ObjectTypeText.text = Category[0];

        TutorialText1.text = Tutorial1[0];
        TutorialText2.text = Tutorial1[1];
        TutorialText3.text = Tutorial1[2];

/*        videoPlayer1.clip = videoClips[0];
        videoPlayer2.clip = videoClips[1];
        videoPlayer3.clip = videoClips[2];*/

/*        videoPlayer1.Play();
        videoPlayer2.Play();
        videoPlayer3.Play();*/
    }
    void SonarTower()
    {
        CalistoUI.SetActive(true);
        ObjectTypeText.text = Category[1];
        TutorialText1.text = Tutorial2[0];
        TutorialText2.text = Tutorial2[1];
        TutorialText3.text = Tutorial2[2];

/*        videoPlayer1.clip = videoClips[3];
        videoPlayer2.clip = videoClips[4];
        videoPlayer3.clip = videoClips[5];*/
/*
        videoPlayer1.Play();
        videoPlayer2.Play();
        videoPlayer3.Play();*/
    }
    void PowerExplained()
    {
        CalistoUI.SetActive(true);
        ObjectTypeText.text = Category[2];
        TutorialText1.text = Tutorial3[0];
        TutorialText2.text = Tutorial3[1];
        TutorialText3.text = Tutorial3[2];

/*
        videoPlayer1.clip = videoClips[6];
        videoPlayer2.clip = videoClips[7];
        videoPlayer3.clip = videoClips[8];*/
/*
        videoPlayer1.Play();
        videoPlayer2.Play();
        videoPlayer3.Play();*/
    }
}
