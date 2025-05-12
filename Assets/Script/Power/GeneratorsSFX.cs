using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorsSFX : MonoBehaviour
{

    [SerializeField] List<AudioClip> adcp = new List<AudioClip>();
    AudioSource ads;

    [SerializeField] bool sfxOnDebug = false;
    [SerializeField] bool sfxOffDebug = false;

    public static GeneratorsSFX instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        ads = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sfxOnDebug)
            PlayOnAudio();
        else if (sfxOffDebug)
            PlayOffAudio();
    }

    public void PlayOnAudio()
    {
        ads.clip = adcp[0];
        ads.Play();
    }
    public void PlayOffAudio()
    {
        ads.clip = adcp[1];
        ads.Play();
    }
}
