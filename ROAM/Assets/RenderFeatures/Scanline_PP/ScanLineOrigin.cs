using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ScanLineOrigin : MonoBehaviour
{
    public Material material;

    private void OnEnable()
    {
        material.SetVector("_Origin", transform.position);
    }
    void LateUpdate()
    {
        material.SetVector("_Origin", transform.position);
    }
}
