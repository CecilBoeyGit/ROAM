using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
public class SetMPB : MonoBehaviour
{
    [ColorUsage(true, true)]
    public Color color = Color.white;

    [Range(0, 10)]
    public int index = 0;

    public float panTime = 0;

    private MaterialPropertyBlock _mpb;
    private Renderer _renderer;

    private void OnEnable()
    {
        ApplyProperties();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        ApplyProperties();
    }
#endif

    private void ApplyProperties()
    {
        if (_renderer == null)
            _renderer = GetComponent<Renderer>();

        if (_mpb == null)
            _mpb = new MaterialPropertyBlock();

        _renderer.GetPropertyBlock(_mpb);

        _mpb.SetColor("_EmissiveColor", color);
        _mpb.SetFloat("_AtlasIndex", index);
        _mpb.SetFloat("_PanTime", panTime);

        _renderer.SetPropertyBlock(_mpb);
    }
}
