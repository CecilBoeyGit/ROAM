using UnityEngine;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    public GameObject defaultButton;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(defaultButton);
    }

    public void OnReactive()
    {
        EventSystem.current.SetSelectedGameObject(defaultButton);
    }
}
