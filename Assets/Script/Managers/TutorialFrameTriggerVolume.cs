using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

public class TutorialFrameTriggerVolume : MonoBehaviour
{
    [Header("Trigger Settings")]
    public string targetTag = "Player";

    [Header("Object to be framed during Tutorial")]
    public GameObject ObjectToFrame;
    Vector3 objectFramePositionHolder;

    GameObject tutorialVolumeSelf;
    [SerializeField] GameObject contentObject;

    public GameObject virtualCamera;

    [HideInInspector]
    public int selectedChildIndex = 0;

    [System.Serializable]
    public class GameObjectEvent : UnityEvent<GameObject> { }

    [Header("On Trigger Enter Event")]
    public GameObjectEvent onTriggerEnter;

    private List<GameObject> frameChildren = new List<GameObject>();

    TutorialFrameManager tutorialFrameInstance;

    private void Awake()
    {
        virtualCamera.SetActive(false);
        tutorialVolumeSelf = transform.gameObject;
    }

    private void Start()
    {
        tutorialFrameInstance = TutorialFrameManager.instance;
        objectFramePositionHolder = ObjectToFrame == null ? Vector3.zero : ObjectToFrame.transform.position;
        contentObject = tutorialFrameInstance.tutorialContentParent;
        UpdateChildList();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag) && frameChildren.Count > selectedChildIndex)
        {
            GameObject selectedChild = frameChildren[selectedChildIndex];
            if (selectedChild != null)
            {
                tutorialFrameInstance.OnTutorialFrameActive(selectedChild, virtualCamera, objectFramePositionHolder);
                virtualCamera.SetActive(true);
                onTriggerEnter?.Invoke(selectedChild);

                tutorialVolumeSelf.SetActive(false);
            }
        }
    }

    public void UpdateChildList()
    {
        frameChildren.Clear();
        if (contentObject != null)
        {
            foreach (Transform child in contentObject.transform)
            {
                frameChildren.Add(child.gameObject);
            }
        }
    }

    public List<string> GetChildNames()
    {
        UpdateChildList();
        List<string> names = new List<string>();
        foreach (var child in frameChildren)
        {
            names.Add(child.name);
        }
        return names;
    }
}
