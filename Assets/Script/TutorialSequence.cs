using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSequence : MonoBehaviour
{
    public TutorialElementPlayScript audioSubtitlePlayer;
    public DisplayUITips displayuitips;
    // Public fields for audio clip and subtitle arrays

//Pickup1_1
    public AudioClip[] pickupAudioClips;
    public string[] pickupSubtitles;
    //Pickup1_2
    public AudioClip[] pickupAudioClips_2;
    public string[] pickupSubtitles_2;

    //Insert1_1
    public AudioClip[] insertAudioClips;
    public string[] insertSubtitles;
    //Insert1_2
    public AudioClip[] insertAudioClips_2;
    public string[] insertSubtitles_2;

  //Pickup2_1//
    public AudioClip[] pickupAudioClips2;
    public string[] pickupSubtitles2;

    //Pickup2_2//
    public AudioClip[] pickupAudioClips2_2;
    public string[] pickupSubtitles2_2;

    //Insert2_1//
    public AudioClip[] insertAudioClips2;
    public string[] insertSubtitles2;

    //Insert2_2//
    public AudioClip[] insertAudioClips2_2;
    public string[] insertSubtitles2_2;

    //FirstEncounter1//
    public AudioClip[] FirstEncounterAudioClips2;
    public string[] FirstEncounterSubtitles2;

    //FirstEncounter2//
    public AudioClip[] FirstEncounterAudioClips2_2;
    public string[] FirstEncounterSubtitles2_2;

    //PickUp3_1//
    public AudioClip[] pickupAudioClips3;
    public string[] pickupSubtitles3;

    //PickUp3_2//
    public AudioClip[] pickupAudioClips3_2;
    public string[] pickupSubtitles3_2;

    //Insert3_1//
    public AudioClip[] insertAudioClips3;
    public string[] insertSubtitles3;

    //Insert3_2//
    public AudioClip[] insertAudioClips3_2;
    public string[] insertSubtitles3_2;

    public AudioClip[] CockpitClips3;
    public string[] CockpitSubtitles3;

    public static TutorialSequence Instance;

    private void OnEnable()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public void DayLoop()
    {
        displayuitips.UpdateText("Maintain and Charge Generators till countdown completes");
    }

    public void PickUpPowerCore1() 
    {
        // Assign defined arrays to the AudioSubtitlePlayer
        audioSubtitlePlayer.audioClips = pickupAudioClips;
        audioSubtitlePlayer.subtitles = pickupSubtitles;

        // Start playing audio with subtitles
        audioSubtitlePlayer.PlayAudioWithSubtitles();
        displayuitips.UpdateText("Go to a socket and PICK UP a PowerCore");
    }


    public void PickUpPowerCore1_2()
    {
        // Assign defined arrays to the AudioSubtitlePlayer
        audioSubtitlePlayer.audioClips = pickupAudioClips_2;
        audioSubtitlePlayer.subtitles = pickupSubtitles_2;

        // Start playing audio with subtitles
        audioSubtitlePlayer.PlayAudioWithSubtitles();
        displayuitips.UpdateText("Picked Up Complete");
    }






    public void InsertPowerCore1_1()
    {
        // Assign defined arrays to the AudioSubtitlePlayer
        audioSubtitlePlayer.audioClips = insertAudioClips;
        audioSubtitlePlayer.subtitles = insertSubtitles;

        // Start playing audio with subtitles
        audioSubtitlePlayer.PlayAudioWithSubtitles();
        displayuitips.UpdateText("Find the GENERATOR SOCKET and INSERT the PowerCore");
    }
    public void InsertPowerCore1_2()
    {
        // Assign defined arrays to the AudioSubtitlePlayer
        audioSubtitlePlayer.audioClips = insertAudioClips;
        audioSubtitlePlayer.subtitles = insertSubtitles;

        // Start playing audio with subtitles
        audioSubtitlePlayer.PlayAudioWithSubtitles();
        //displayuitips.UpdateText("PressÅhEÅh to insert powercore", 5f);
    }






    public void PickUpPowerCore2()
    {
        // Assign defined arrays to the AudioSubtitlePlayer
        audioSubtitlePlayer.audioClips = pickupAudioClips2;
        audioSubtitlePlayer.subtitles = pickupSubtitles2;

        // Start playing audio with subtitles
        audioSubtitlePlayer.PlayAudioWithSubtitles();
        displayuitips.UpdateText("Go PICKUP a second PowerCore");
    }
    public void PickUpPowerCore2_2()
    {
        // Assign defined arrays to the AudioSubtitlePlayer
        audioSubtitlePlayer.audioClips = pickupAudioClips2_2;
        audioSubtitlePlayer.subtitles = pickupSubtitles2_2;

        // Start playing audio with subtitles
        audioSubtitlePlayer.PlayAudioWithSubtitles();
        displayuitips.UpdateText("Find the GENERATOR SOCKET in HUB and INSERT the PowerCore");
    }








        public void FirstEncounter()
    {
        // Assign defined arrays to the AudioSubtitlePlayer
        audioSubtitlePlayer.audioClips = FirstEncounterAudioClips2;
        audioSubtitlePlayer.subtitles = FirstEncounterSubtitles2;

        // Start playing audio with subtitles
        audioSubtitlePlayer.PlayAudioWithSubtitles();
        displayuitips.UpdateText("'RMB' to SCAN to reveal the Kenemorph and 'LMB' to ELIMINATE");
    }
    public void FirstEncounter_2()
    {
        // Assign defined arrays to the AudioSubtitlePlayer
        audioSubtitlePlayer.audioClips = FirstEncounterAudioClips2_2;
        audioSubtitlePlayer.subtitles = FirstEncounterSubtitles2_2;

        // Start playing audio with subtitles
        audioSubtitlePlayer.PlayAudioWithSubtitles();
        displayuitips.UpdateText("Threat neutralized. INSERT PowerCore to Generator");
    }




    public void InsertPowerCore2()
    {
        // Assign defined arrays to the AudioSubtitlePlayer
        audioSubtitlePlayer.audioClips = insertAudioClips2;
        audioSubtitlePlayer.subtitles = insertSubtitles2;

        // Start playing audio with subtitles
        audioSubtitlePlayer.PlayAudioWithSubtitles();
        displayuitips.UpdateText("You can monitor the generatorÅ's power status. If the power drops to 0 the ship will lose hall integrity. A higher generator power reserve will grant you extra perks.");
    }


    public void InsertPowerCore2_2()
    {
        // Assign defined arrays to the AudioSubtitlePlayer
        audioSubtitlePlayer.audioClips = insertAudioClips2_2;
        audioSubtitlePlayer.subtitles = insertSubtitles2_2;

        // Start playing audio with subtitles
        audioSubtitlePlayer.PlayAudioWithSubtitles();
        //displayuitips.UpdateText("You can monitor the generatorÅfs power situation. If the power drops to 0 the ship will lose hall integrity. A higher generator power reserve will grant you extra perks.", 5f);
    }










    public void PickUpPowerCore3()
    {
        // Assign defined arrays to the AudioSubtitlePlayer
        audioSubtitlePlayer.audioClips = pickupAudioClips3;
        audioSubtitlePlayer.subtitles = pickupSubtitles3;

        // Start playing audio with subtitles
        audioSubtitlePlayer.PlayAudioWithSubtitles();
        displayuitips.UpdateText("PressÅ 'E' to pick up PowerCore");
    }
    public void PickUpPowerCore3_2()
    {
        // Assign defined arrays to the AudioSubtitlePlayer
        audioSubtitlePlayer.audioClips = pickupAudioClips3_2;
        audioSubtitlePlayer.subtitles = pickupSubtitles3_2;

        // Start playing audio with subtitles
        audioSubtitlePlayer.PlayAudioWithSubtitles();
       // displayuitips.UpdateText("PressÅhEÅh to pick up powercore", 3f);
    }










    public void InsertPowerCore3()
    {
        // Assign defined arrays to the AudioSubtitlePlayer
        audioSubtitlePlayer.audioClips = insertAudioClips3;
        audioSubtitlePlayer.subtitles = insertSubtitles3;

        // Start playing audio with subtitles
        audioSubtitlePlayer.PlayAudioWithSubtitles();
        displayuitips.UpdateText("PressÅ 'EÅ' to insert PowerCore");
    }
    public void InsertPowerCore3_2()
    {
        // Assign defined arrays to the AudioSubtitlePlayer
        audioSubtitlePlayer.audioClips = insertAudioClips3_2;
        audioSubtitlePlayer.subtitles = insertSubtitles3_2;

        // Start playing audio with subtitles
        audioSubtitlePlayer.PlayAudioWithSubtitles();
        //displayuitips.UpdateText("PressÅhEÅh to insert powercore", 5f);
    }




    public void FinalCockpit()
    {
        // Assign defined arrays to the AudioSubtitlePlayer
        audioSubtitlePlayer.audioClips = CockpitClips3;
        audioSubtitlePlayer.subtitles = CockpitSubtitles3;

        // Start playing audio with subtitles
        audioSubtitlePlayer.PlayAudioWithSubtitles();
        displayuitips.UpdateText("/. COMPLETE");
    }
}
