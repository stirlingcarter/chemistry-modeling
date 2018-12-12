/*******************************************************
 * Copyright (C) 2016 3lbGames - Chris Castaldi
 * 
 * Haptics Helper
 
 For additional Information or unity development services please contact services@3lbgames.com
 
 For technical support contact support@3lbgames.com

 
 *******************************************************/

/*******************************************************
V 1.0

Haptic Helper is a singleton that allows you to call haptics anywhere for the Oculus Touch. It also includes a test mode so you can test clips and procedural haptics.

Haptic Helper Features

-Predesigned Positive and Negative Clips
-Call Extra Preset Clips
-Convert AudioClip Directly to Haptic
-Generate Procedural Haptic Tone
-Haptic Test Mode for In-editor
-Sample Haptic Waveforms


Note: The Touch Controllers do not play Haptics unless the headset is on or you cover the sensor.

Example Calls:

        //Plays Bad Clip on Left Controller
        HapticHelper.instance.PlayBadClip(true);

         //Plays Bad Clip on Right Controller
        HapticHelper.instance.PlayBadClip(false);

        //Plays Good Clip on Right Controller
        HapticHelper.instance.PlayGoodClip(false);

          //Plays Good Clip on left Controller
        HapticHelper.instance.PlayGoodClip(true);

         //Plays Good Clip on left Controller and overRide all other Haptics on this Controller
        HapticHelper.instance.PlayGoodClip(true,true);

        //Plays ExtraAudioClips in Index Zero on Left Controller
        HapticHelper.instance.PlayExtraClips(true,0);

        //Plays Extra Clip in Index Zero on Left Controller
        HapticHelper.instance.PlayExtraClips(true,0);

        //Plays Procedural Tone at max Intensity for 20ms on  Left Controller
        HapticHelper.instance.ProceduralTone(true, 255, 20);

        //Plays Provided AudioClip on Left Controller
        HapticHelper.instance.PlayHapticAudioClip(true, aClip);

*/

using UnityEngine;
using System.Collections;
using System;
public class HapticHelper : MonoBehaviour {

    // Get double from another script //
    public double limitval;
    public double score;
    public int vibrationint;
    public GameObject Calculation;
    public CollidersVDWScore CalculationObject;

public static HapticHelper instance; 
    //Storage For 
   private OVRHaptics.OVRHapticsChannel m_hapticsChannelL = null;
   private OVRHaptics.OVRHapticsChannel m_hapticsChannelR = null;
    [Header("-ClipHookUps-")]
    public AudioClip GoodAudio; //Good Audio Clip
   public AudioClip BadAudio;   // Bad AudioClip;
   public AudioClip[] ExtraAudioClips;
   private OVRHapticsClip GoodClip;
   private OVRHapticsClip BadClip;
   private OVRHapticsClip proceduralClip;
   private OVRHapticsClip ExtraClip;
    [Header("-Settings-")]
    public bool AlwaysPreempt;
   [Header("-Haptics Test-")]
   public bool TestMode;
   public OVRInput.Controller myController; //Which Controller
   public OVRInput.Button ClipButton;       //Button to Test Clip;
   public OVRInput.Button ProceduralButton; //Button to Test procedrual;
    public AudioClip ClipToTest; //Audio Clip to Test 
   [Header("-Procedural Testing-")]
   public bool continuous;  //Call Procedural every frame the button is pressed
    [Range(0,255)]
   public int pIntensity;
   [Range(1,1000)] //Pulse length is in miliseconds
   public int pLength = 25; //Length of the pulse in ms

   //See Sin Example Below;
   public bool useSin;      //Test SinWave Generator
   public float SinSpeed = 25;//Sin Speed
   void Awake()
   {

       //Singleton Scripting
       if (instance == null)
           instance = this;
       else if (instance != this)
           Destroy(gameObject);

   }
   private void Start()
   {
        // Get double from another script
        CalculationObject = Calculation.GetComponent<CollidersVDWScore>();

        m_hapticsChannelL = OVRHaptics.LeftChannel;
       m_hapticsChannelR = OVRHaptics.RightChannel;
        if (GoodAudio) { GoodClip = new OVRHapticsClip(GoodAudio); }
        if (BadAudio) { BadClip = new OVRHapticsClip(BadAudio); }
        //Create EmptyClip
        proceduralClip = new OVRHapticsClip();
    }

    // For Testing Clips in Editor
    public void Update()
    {
        if (TestMode)
        {
            score = CalculationObject.vdw_score;

            if (255 * score / limitval > 2000000 || score > limitval)
            {
                vibrationint = 255;
            }
            else { vibrationint = Convert.ToInt32(255 * score / limitval); }
            if (score > 0)
            {
                ProceduralTone(false, vibrationint, 100);
                ProceduralTone(true, vibrationint, 100);
            }
        }
    }
    //Play the Bad Clip
    //isLeft Which Controller | preempt will Override all other Haptic Clips playing on this Controller
   public void PlayGoodClip(bool isLeft,bool preempt = false)
   {
        //Error Checking
        if (GoodClip == null)
        {
            Debug.Log("GoodClip not Setup Failed to Call Haptic");
            return;
        }
        if (preempt)
       {
           ClipPlayerNow(isLeft, GoodClip);
       }
       else
       {
           ClipPlayer(isLeft, GoodClip);
       }

   }

    //Play the Bad Clip
    //isLeft Which Controller | preempt will Override all other Haptic Clips playing on this Controller
   public void PlayBadClip(bool isLeft, bool preempt = false)
   {
        //Error Checking
        if (BadClip == null)
        {
            Debug.Log("BadClip not Setup Failed to Call Haptic");
            return;
        }
       if (preempt)
       {
           ClipPlayerNow(isLeft, BadClip);
       }
       else
       {
           ClipPlayer(isLeft, BadClip);
       }
   }


    // Play from ExtraAudioClips List
    //isLeft Which Controller | index which Index in ExtraClips | preempt will Override all other Haptic Clips playing on this Controller
   public void PlayExtraClips(bool isLeft,int index, bool preempt = false)
   {
       //No Error Checking So Be sure your indexes are Correct;
       OVRHapticsClip clip = new OVRHapticsClip(ExtraAudioClips[index]);
       if (preempt)
       {
           ClipPlayerNow(isLeft, clip);
       }
       else
       {
           ClipPlayer(isLeft, clip);
       }
   }

    public void PlayHapticAudioClip(bool isLeft,AudioClip aClip, bool preempt = false)
    {
        //No Error Checking So Be sure your indexes are Correct;
        OVRHapticsClip clip = new OVRHapticsClip(aClip);
        if (preempt)
        {
            ClipPlayerNow(isLeft, clip);
        }
        else
        {
            ClipPlayer(isLeft, clip);
        }
    }

    //Creates and Plays a Procedural Tone
    // Left: Which Controller | intensity: How Powerful | length: how long in ms 
    public void ProceduralTone(bool Left, int intensity, int length = 25)
    {
        //Stop intensity going out side of byte range
        intensity = Mathf.Clamp(intensity, 0, 255);
        byte holder = Convert.ToByte(intensity);
        proceduralClip.Reset();
        for (int i = 0; i < length; i++)
        {
            proceduralClip.WriteSample(holder);
        }
        ClipPlayerNow(Left, proceduralClip);
    }

    //GenerateSinPulse this is an example.
    //Left: Which Controller | intensity: How Powerful |speed: WaveRateofChange | length: is each change  
    public void GenerateSinPulse(bool Left, int intensity, float speed, int length = 25)
    {
        intensity = Mathf.Abs(Mathf.RoundToInt((intensity * Mathf.Sin(speed * Time.time))));
        ProceduralTone(Left, intensity, length);
    }

    //Internal Clip Player checks to see if it is always preempted is checked; 
    //isLeft Which Controller | clip: The Haptics Clip to Play
    public void ClipPlayer(bool isLeft,OVRHapticsClip clip)
   {
       if(isLeft)
       {
           if(AlwaysPreempt)
           {
               m_hapticsChannelL.Preempt(clip);
           }
           else
           {
               m_hapticsChannelL.Queue(clip);
           }
       }
       else
       {
           if (AlwaysPreempt)
           {
               m_hapticsChannelR.Preempt(clip);
           }
           else
           {
               m_hapticsChannelR.Queue(clip);
           }
       }
   }

    // Internal Clip Player that Always Preempted;
    public void ClipPlayerNow(bool isLeft, OVRHapticsClip clip)
   {
       if (isLeft)
       {
           m_hapticsChannelL.Preempt(clip);
       }
       else
       {
           m_hapticsChannelR.Preempt(clip);
       }
   }
  
}
