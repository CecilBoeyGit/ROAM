using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadForkLiftBehaviors : MonoBehaviour
{

    public int visualToDisplay = 0;
    public int lineToPlay = 0;

    [SerializeField] GameObject Visuals;
    List<GameObject> visualsList = new List<GameObject>();

    [SerializeField] SequentialLineTrigger sequentialLineComponent;



    private void OnEnable()
    {
        sequentialLineComponent.GetComponent<SequentialLineTrigger>();

        foreach (Transform child in Visuals.transform)
        {
            visualsList.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        visualsList[visualToDisplay].SetActive(true);
        if (sequentialLineComponent != null)
            sequentialLineComponent.currentClipIndex = lineToPlay;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
