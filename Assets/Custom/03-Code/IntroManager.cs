using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    public AudioSource phoneAudioSourceCall;
    public AudioSource phoneAudioSourceVibrate;
    public AudioSource phoneAudioSourceRingtone;
    public AudioSource doorAudioSource;
    //plays from the head
    public AudioSource characterAudioSource;
    public Light doorLight;
    public float doorLightMaxIntensity = 10f;
    public Light phoneLight;
    IntroStates introState = IntroStates.recognizingHands;
    public AudioClip NatalinaIntro2;

    float fadeLengthMax;
    float fadeLengthCurrent = 0f;


    private void Start()
    {
        //run the stuff related to "this is a hand experience please use your hands"
        //AFTER HANDS ARE REGISTERED, go to wait for phone ring state
        fadeLengthMax = NatalinaIntro2.length;


        
    }

    public void handsRecognized()
    {
        introState = IntroStates.waitForPhoneRing;
        Invoke("enterPhoneRingState", 5f);
    }

    public void enterPhoneRingState()
    {
        introState = IntroStates.phoneRinging;
        phoneAudioSourceCall.Play();
        phoneAudioSourceVibrate.Play();
    }

    public void enterPhoneAnswered()
    {
        introState = IntroStates.phoneAnswered;
        phoneAudioSourceCall.Stop();
        phoneAudioSourceVibrate.Stop();
        phoneAudioSourceCall.Play();
    }

    public void phoneCallEnded()
    {
        introState = IntroStates.doorLight;
        doorAudioSource.volume = 0;
        doorAudioSource.Play();
    }

    public void Update()
    {
        if (introState == IntroStates.phoneAnswered 
            && !phoneAudioSourceCall.isPlaying)
        {
            phoneCallEnded();
        } else if (introState == IntroStates.doorLight)
        {
            fadeLengthCurrent += Time.deltaTime;
            float fadePercent = fadeLengthCurrent / fadeLengthMax;
            //will get louder as the dialogue completes.
            //This is directional, so there's no worry about it overriding the 2D audio
            //source coming from the narration. 
            //...or is there?
            doorLight.intensity = fadePercent * doorLightMaxIntensity;
            doorAudioSource.volume = fadePercent;
            if (fadeLengthCurrent >= fadeLengthMax)
            {
                introState = IntroStates.doorOpenReminders;
                InvokeRepeating("doorOpenReminders", 10f, 20f);
            }
        }
    }

    public void doorOpenReminders()
    {
        //some logic here to play audio clips looped through a list. 
    }

    public void doorOpened()
    {
        introState = IntroStates.doorOpened;
    }

    


}



public enum IntroStates
{
    recognizingHands,
    //give the player like 5 seconds before the phone starts ringing to look at their hands
    waitForPhoneRing,
    //The phone is ringing. Use an invoke repeating to remind the player (I had better pick up the phone
    //it might be important)
    phoneRinging,
    //Phone Answered. Give the player things to touch and pinch here so they can play around with their hands while
    //the dialogue is happening
    phoneAnswered,
    // door light is fading up and the ambient audio is coming in, to direct the player's gaze
    doorLight,
    //open door reminders in case the player REALLY doesn't know what to do at this point. 
    doorOpenReminders,
    //scene fade begins. I should include a "fact randomizer" for the load screen. 
    //Note: There is no REQUIREMENT that the player even picks up the gosh darn phone. They CAN just go
    //and pinch the door open outright if they are *that* kind of player.
    doorOpened
        
}

