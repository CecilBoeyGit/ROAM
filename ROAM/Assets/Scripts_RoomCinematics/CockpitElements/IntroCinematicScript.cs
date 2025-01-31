using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCinematicScript : MonoBehaviour
{

    //public bool PlayerSpawn = false;
   public  GameObject screen;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void InitiateVoice()
    {
        screen.SetActive(true);
    }
}
