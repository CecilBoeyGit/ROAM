using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadForkLiftBehaviors : MonoBehaviour
{

    [SerializeField] private int visualToDisplay = 0;
    [SerializeField] private int lineToPlay = 0;
    [SerializeField] private int initialIndex;
    [SerializeField] private int segmentIndex;

    [SerializeField] GameObject Visuals;
    [SerializeField] List<GameObject> visualsList = new List<GameObject>();

    [SerializeField] SequentialLineTrigger sequentialLineComponent;

    private void Awake()
    {
        sequentialLineComponent.GetComponent<SequentialLineTrigger>();

        if (visualsList.Count != 0)
            visualsList.Clear();

        foreach (Transform child in Visuals.transform)
        {
            visualsList.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.2f);
    }

    public void SetActiveState(int visualToDisplay, int lineToPlay, int initialIndex, int segmentIndex)
    {
        if (visualToDisplay >= 0 && visualToDisplay < visualsList.Count)
        {
            //print(visualToDisplay + " " + lineToPlay);
            this.visualToDisplay = visualToDisplay;
            this.lineToPlay = lineToPlay;
            this.segmentIndex = segmentIndex;
            this.initialIndex = initialIndex;

            visualsList[visualToDisplay].SetActive(true);
            if (sequentialLineComponent != null)
            {
                sequentialLineComponent.ShuffleBySegment(initialIndex, segmentIndex);
                sequentialLineComponent.currentClipIndex = lineToPlay;
            }
        }
    }
}
