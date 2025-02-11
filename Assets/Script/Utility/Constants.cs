using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{

    public static Constants instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }

    public static Vector3 UPDIR { get { return new Vector3(-0.70710678118f, 0, 0.70710678118f); } }
    public static Vector3 RIGHTDIR { get { return new Vector3(0.70710678118f, 0, 0.70710678118f); } }
}
