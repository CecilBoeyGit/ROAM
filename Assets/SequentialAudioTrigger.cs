using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;   // ← if you’re using TextMeshPro for your UI

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class SequentialLineTrigger : MonoBehaviour
{
    [Header("Audio Settings")]
    public List<AudioClip> audioClips = new List<AudioClip>();

    [Header("Subtitles")]
    [Tooltip("A subtitle string for each audio clip (must match audioClips.Count).")]
    public List<string> subtitles = new List<string>();

    [Header("Subtitle UI")]
    [Tooltip("Drag in your TextMeshProUGUI component here.")]
    public TextMeshProUGUI subtitleText;

    [Header("Target Object")]
    [Tooltip("The GameObject to disable when the audio clip is played.")]
    public GameObject objectToDisable;

    [Header("Reset Options")]
    [Tooltip("Enable to allow resetting the audio index via the R key.")]
    public bool enableResetKey = true;

    private AudioSource audioSource;
    public int currentClipIndex = 0;
    private const string PREFS_CLIP_INDEX = "CurrentClipIndex";

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Auto‑find subtitleText if you forgot to assign it
        if (subtitleText == null)
            subtitleText = GetComponentInChildren<TextMeshProUGUI>();

        if (subtitleText == null)
            Debug.LogError($"[{name}] subtitleText reference is missing!");

        // Load saved index
        if (PlayerPrefs.HasKey(PREFS_CLIP_INDEX))
            currentClipIndex = PlayerPrefs.GetInt(PREFS_CLIP_INDEX);
        else
            currentClipIndex = 0;

        // Sanity check
        if (subtitles.Count != audioClips.Count)
            Debug.LogWarning($"[{name}] subtitles.Count ({subtitles.Count}) != audioClips.Count ({audioClips.Count})");
    }

    private void Update()
    {
        if (enableResetKey && Input.GetKeyDown(KeyCode.R))
        {
            ResetAudioIndex();
            Debug.Log("Audio index reset!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            PlayLine();
    }

    private void PlayLine()
    {
        if (currentClipIndex >= audioClips.Count)
            return;

        AudioClip clip = audioClips[currentClipIndex];
        string subtitle = currentClipIndex < subtitles.Count
            ? subtitles[currentClipIndex]
            : "";

        StartCoroutine(PlayLineRoutine(clip, subtitle));

        // Advance & save progress
        PlayerPrefs.SetInt(PREFS_CLIP_INDEX, currentClipIndex);
        PlayerPrefs.Save();
    }

    private IEnumerator PlayLineRoutine(AudioClip clip, string subtitle)
    {
        if (clip == null)
            yield break;

        // Show subtitle & play audio
        subtitleText.text = subtitle;
        audioSource.clip = clip;
        audioSource.Play();

        // Disable target object immediately
        if (objectToDisable != null)
            objectToDisable.SetActive(false);

        // Disable collider so it can't retrigger
        var col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // Wait for clip to finish
        yield return new WaitForSeconds(clip.length);

        // Clear subtitle
        subtitleText.text = "";

        // Optionally shut off this entire script
        this.enabled = false;
    }

    public void ResetAudioIndex()
    {
        currentClipIndex = 0;
        PlayerPrefs.DeleteKey(PREFS_CLIP_INDEX);
        PlayerPrefs.Save();

        // Re‑enable trigger
        var col = GetComponent<Collider>();
        if (col != null)
            col.enabled = true;
        this.enabled = true;

        // Clear subtitle & optional re‑enable target
        subtitleText.text = "";
       
    }
}
