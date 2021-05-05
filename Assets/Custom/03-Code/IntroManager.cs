using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public IntroStates introState = IntroStates.recognizingHands;
    public AudioClip NatalinaIntro1;
    public AudioClip NatalinaIntro2;

    public AudioClip doorReminder1;
    public AudioClip doorReminder2;

    private int phoneReminderIndex;
       
    public AudioClip phoneReminder1;
    public AudioClip phoneReminder2;
    public AudioClip phoneReminder3;

    float fadeLengthMax;
    float fadeLengthCurrent = 0f;


    private void Start()
    {
        //run the stuff related to "this is a hand experience please use your hands"
        //AFTER HANDS ARE REGISTERED, go to wait for phone ring state
        fadeLengthMax = NatalinaIntro2.length;
        //handsRecognized();

        
    }

    public void handsRecognized()
    {
        introState = IntroStates.waitForPhoneRing;
        Invoke("enterPhoneRingState", 5f);
    }

    public void enterPhoneRingState()
    {
        introState = IntroStates.phoneRinging;
        phoneAudioSourceRingtone.Play();
        phoneAudioSourceVibrate.Play();
        InvokeRepeating("phoneReminderAudio", 30f, 15f);
    }

    private void phoneReminderAudio()
    {
        switch(phoneReminderIndex)
        {
            case 0:
                characterAudioSource.clip = phoneReminder1;
                break;
            case 1:
                characterAudioSource.clip = phoneReminder2;
                break;
            case 2:
                characterAudioSource.clip = phoneReminder3;
                break;

        }
        characterAudioSource.Play();
        phoneReminderIndex++;
        if (phoneReminderIndex > 2)
        {
            phoneReminderIndex = 0;
        }
    }

    public void enterPhoneAnswered()
    {
        if (introState == IntroStates.phoneRinging)
        {
            //stop the reminders to pick up the phone
            CancelInvoke();
            introState = IntroStates.phoneAnswered;
            phoneAudioSourceRingtone.Stop();
            phoneAudioSourceVibrate.Stop();
            characterAudioSource.clip = NatalinaIntro1;
            characterAudioSource.Play();
            //phoneAudioSourceCall.Play();
            Invoke("phoneCallEnded", NatalinaIntro1.length);
        }
    }

    public void phoneCallEnded()
    {
        introState = IntroStates.doorLight;
        doorAudioSource.volume = 0;
        doorAudioSource.Play();
    }

    public void Update()
    {
        if (introState == IntroStates.recognizingHands)
        {
            if (OVRPlugin.GetHandTrackingEnabled())
            {
                handsRecognized();
            } else
            {
                //...what if they don't have hands
            }
        }
        if (introState == IntroStates.doorLight)
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
                characterAudioSource.clip = doorReminder1;
                characterAudioSource.PlayDelayed(30f);
                InvokeRepeating("doorOpenReminders", 60f, 15f);
            }
        }
    }

    public void doorOpenReminders()
    {
        //some logic here to play audio clips looped through a list. 
        characterAudioSource.clip = doorReminder2;
        characterAudioSource.Play();
    }

    public void doorOpened()
    {
        CancelInvoke();
        introState = IntroStates.doorOpened;
        OVRScreenFade screenFade = FindObjectOfType<OVRScreenFade>();
        screenFade.FadeOut();
        Invoke("initiateSceneTransition", screenFade.fadeTime);
    }

    public void initiateSceneTransition()
    {
        SceneManager.LoadScene("Nathan-Sandbox");
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

