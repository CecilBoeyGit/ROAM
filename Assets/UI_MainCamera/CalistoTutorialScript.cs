using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Video;
using System.Reflection;

public class CalistoTutorialScript : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI ObjectTypeText;
    public TextMeshProUGUI TutorialText1;
    public TextMeshProUGUI TutorialText2;
    public TextMeshProUGUI TutorialText3;

    [Header("Tutorial Data")]
    public string[] Category;
    public string[] Tutorial1;
    public string[] Tutorial2;
    public string[] Tutorial3;

    [Header("Video Players & Clips")]
    public List<VideoPlayer> videoPlayers = new List<VideoPlayer>(); // Supports multiple VideoPlayers
    public List<VideoClip> videoClips = new List<VideoClip>(); // Stores tutorial clips

    [SerializeField] private string methodName;
    [SerializeField] private GameObject CalistoUI; // Assign the tutorial UI panel

    private void Start()
    {
        InvokeMethodByName(methodName);
    }

    private void InvokeMethodByName(string methodName)
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

    public void CloseTutorial()
    {
        gameObject.SetActive(false); // Deactivates this object
    }

    private void PlayVideos(int startIndex)
    {
        for (int i = 0; i < videoPlayers.Count; i++)
        {
            if (startIndex + i < videoClips.Count)
            {
                videoPlayers[i].clip = videoClips[startIndex + i];
                videoPlayers[i].Play();
            }
        }
    }

    private void StopAllVideos()
    {
        foreach (var vp in videoPlayers)
        {
            vp.Stop();
        }
    }

    private void SetupTutorial(string category, string[] tutorialText, int videoStartIndex)
    {
        CalistoUI.SetActive(true);
        ObjectTypeText.text = category;
        TutorialText1.text = tutorialText.Length > 0 ? tutorialText[0] : "";
        TutorialText2.text = tutorialText.Length > 1 ? tutorialText[1] : "";
        TutorialText3.text = tutorialText.Length > 2 ? tutorialText[2] : "";

        StopAllVideos(); // Stop any running videos before starting new ones
        PlayVideos(videoStartIndex);
    }

    void Countdown()
    {
        SetupTutorial(Category[0], Tutorial1, 0);
    }

    void SonarTower()
    {
        SetupTutorial(Category[1], Tutorial2, 3);
    }

    void PowerExplained()
    {
        SetupTutorial(Category[2], Tutorial3, 6);
    }
}
