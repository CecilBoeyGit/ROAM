using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[ExecuteAlways]
public class TutorialFrameManager : MonoBehaviour
{

    [HideInInspector]
    public GameObject cameraTarget;
    public GameObject frame;
    [SerializeField] GameObject continueGraphics;
    [SerializeField] float cameraThreshold = 1.0f;
    GameObject activeVolumeObject;

    [SerializeField] string tutorialContentParentName = "TutorialContent";
    [HideInInspector]
    public GameObject tutorialContentParent;
    GameObject tutorialContentToShow;

    bool isDisplayingTutorial = false;

    PlayerController playerInstance;
    InputSubscriptions _InputSub;

    public static TutorialFrameManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Init()
    {
        if (continueGraphics != null)
            continueGraphics.SetActive(false);
        if (frame != null)
            frame.SetActive(false);

        isDisplayingTutorial = false;

        tutorialContentParent = GetTutorialContentParent();
        if (tutorialContentParent != null)
        {
            foreach (Transform child in tutorialContentParent.transform)
            {
                child.gameObject.SetActive(false); //To make sure that all child gameObjects are hidden when the game starts
            }
        }
    }

    public GameObject GetTutorialContentParent()
    {
        foreach (Transform child in this.transform)
        {
            if (child.name.Equals(tutorialContentParentName))
            {
                return child.gameObject;
            }
        }

        return null;
    }

    private void OnEnable()
    {
        Init();
    }
    // Start is called before the first frame update
    void Start()
    {
        playerInstance = PlayerController.instance;
        _InputSub = InputSubscriptions.instance;
    }

    // Update is called once per frame
    void Update()
    {
        DistanceToCameraTarget();
        OnTutorialFrameDeactive();
    }

    public void OnTutorialFrameActive(GameObject activeContent, GameObject virtualCamera, Vector3 objectToFrame)
    {
        tutorialContentToShow = activeContent;
        cameraTarget = virtualCamera;
        if (frame != null)
        {
            frame.transform.position = objectToFrame;
            frame.SetActive(true);
        }
        playerInstance.PlayerConstrained = true;
    }
    void OnTutorialFrameDeactive()
    {
        if (isDisplayingTutorial)
        {
            if (_InputSub.InteractInput)
            {
                continueGraphics.SetActive(false);
                frame.SetActive(false);
                tutorialContentToShow.SetActive(false);
                cameraTarget.SetActive(false);
                playerInstance.PlayerConstrained = false;
                isDisplayingTutorial = false;
            }
        }
    }

    void DistanceToCameraTarget()
    {
        if (!playerInstance.PlayerConstrained)
            return;

        if (isDisplayingTutorial)
            return;

        //Check if the Main Camera has interpolated pass the threshold point to the target Virtual Camera
        if (Vector3.Distance(Camera.main.transform.position, cameraTarget.transform.position) < cameraThreshold)
        {
            tutorialContentToShow.SetActive(true); //If yes, turn on the tutorial sequence
            continueGraphics.SetActive(true);
            isDisplayingTutorial = true;
        }
    }
}
