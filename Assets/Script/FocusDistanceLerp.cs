using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // Use this for URP, use HDRP if needed

public class FocusDistanceLerp : MonoBehaviour
{
    public Volume volume; // Change to Volume instead of PostProcessVolume
    private DepthOfField depthOfField;

    public float startFocusDistance = 0.1f;
    public float targetFocusDistance = 2.5f;
    public float transitionDuration = 0.2f;

    void Start()
    {
        if (volume.profile.TryGet(out depthOfField))
        {
            depthOfField.focusDistance.value = startFocusDistance;
            StartCoroutine(ChangeFocusDistance(targetFocusDistance, transitionDuration));
        }
        else
        {
            Debug.LogError("Depth of Field effect not found in Volume Profile!");
        }
    }

    private IEnumerator ChangeFocusDistance(float targetValue, float duration)
    {
        float startValue = depthOfField.focusDistance.value;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            depthOfField.focusDistance.value = Mathf.Lerp(startValue, targetValue, t);
            yield return null;
        }

        depthOfField.focusDistance.value = targetValue;
    }
}
