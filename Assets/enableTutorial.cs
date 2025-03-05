using UnityEngine;
using System.Collections.Generic;

public class EnableTutorial : MonoBehaviour
{
    private Dictionary<string, bool> tutorialParameters = new Dictionary<string, bool>();
    [SerializeField] private GameObject objectToEnable; // The object to show

    // Method to set tutorial parameter
    public void SetTutorialParameter(string parameterName, bool value)
    {
        tutorialParameters[parameterName] = value;
        CheckTutorialState();
    }

    // Check if the tutorial should be enabled
    private void CheckTutorialState()
    {
        if (tutorialParameters.ContainsValue(true))
        {
            objectToEnable.SetActive(true);
        }
        else
        {
            objectToEnable.SetActive(false);
        }
    }
}
