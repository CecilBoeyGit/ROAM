using UnityEngine;

[RequireComponent(typeof(Light))]
public class SmoothBlinkingLight : MonoBehaviour
{
    [Header("Intensity Settings")]
    [Tooltip("Intensity when “off” (usually 0)")]
    public float minIntensity = 0f;

    [Tooltip("Intensity when “on”")]
    public float maxIntensity = 1f;

    [Header("Timing Settings (seconds)")]
    [Tooltip("Time to fade from min → max")]
    public float fadeInDuration = 0.5f;

    [Tooltip("Time to stay at maxIntensity")]
    public float holdOnDuration = 0.5f;

    [Tooltip("Time to fade from max → min")]
    public float fadeOutDuration = 0.5f;

    [Tooltip("Time to stay at minIntensity")]
    public float holdOffDuration = 0.5f;

    private Light _light;
    private enum State { FadingIn, HoldingOn, FadingOut, HoldingOff }
    private State _state = State.FadingIn;
    private float _timer = 0f;

    void Awake()
    {
        _light = GetComponent<Light>();
        _light.intensity = minIntensity;
    }

    void Update()
    {
        _timer += Time.deltaTime;

        switch (_state)
        {
            case State.FadingIn:
                // Lerp from min → max
                if (fadeInDuration > 0f)
                    _light.intensity = Mathf.Lerp(minIntensity, maxIntensity, _timer / fadeInDuration);
                else
                    _light.intensity = maxIntensity;

                if (_timer >= fadeInDuration)
                    NextState(State.HoldingOn);
                break;

            case State.HoldingOn:
                _light.intensity = maxIntensity;
                if (_timer >= holdOnDuration)
                    NextState(State.FadingOut);
                break;

            case State.FadingOut:
                // Lerp from max → min
                if (fadeOutDuration > 0f)
                    _light.intensity = Mathf.Lerp(maxIntensity, minIntensity, _timer / fadeOutDuration);
                else
                    _light.intensity = minIntensity;

                if (_timer >= fadeOutDuration)
                    NextState(State.HoldingOff);
                break;

            case State.HoldingOff:
                _light.intensity = minIntensity;
                if (_timer >= holdOffDuration)
                    NextState(State.FadingIn);
                break;
        }
    }

    private void NextState(State newState)
    {
        _state = newState;
        _timer = 0f;
    }

    /// <summary>
    /// Immediately jump into fading in on the next frame.
    /// </summary>
    public void BlinkNow()
    {
        _state = State.FadingIn;
        _timer = 0f;
    }
}
