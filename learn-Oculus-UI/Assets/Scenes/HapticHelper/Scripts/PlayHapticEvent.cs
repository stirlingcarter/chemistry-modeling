using UnityEngine;
using System.Collections;
using UnityEngine.Events;

//This is an example of PlayingHaptic Events Each Touch Controller has a Collider which is Tagged. You can easily take these Singletons to test with.
//For More information Look at the HapticHelper;

public class PlayHapticEvent : MonoBehaviour {

    public bool playGood;
    public bool playBad;
    public bool playIndex;
    public bool playProcedrual;
    public bool playClip;
    public int index;
    public AudioClip aClip;

    [Header("-Procedural Testing-")]
    public bool continuous;
    [Range(0, 255)]
    public int pIntensity;
    [Range(1, 1000)] //Pulse length is in miliseconds
    public int pLength = 25;

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "TouchL")
        {
            if(playGood)
            {
                HapticHelper.instance.PlayGoodClip(true);
            }
            if (playBad)
            {
                HapticHelper.instance.PlayBadClip(true);
            }
            if (playIndex)
            {
                HapticHelper.instance.PlayExtraClips(true,index);
            }
            if (playProcedrual)
            {
                HapticHelper.instance.ProceduralTone(true, pIntensity, pLength);
            }
            if (playGood)
            {
                HapticHelper.instance.PlayGoodClip(true);
            }
            if (playClip)
            {
                if (aClip != null)
                {
                    HapticHelper.instance.PlayHapticAudioClip(true, aClip);
                }
            }
        }
        if (col.tag == "TouchR")
        {
            if (playGood)
            {
                HapticHelper.instance.PlayGoodClip(false);
            }
            if (playBad)
            {
                HapticHelper.instance.PlayBadClip(false);
            }
            if (playIndex)
            {
                HapticHelper.instance.PlayExtraClips(false, index);
            }
            if (playProcedrual)
            {
                HapticHelper.instance.ProceduralTone(false, 255);
            }
            if (playClip)
            {
                if(aClip != null)
                {
                    HapticHelper.instance.PlayHapticAudioClip(false, aClip);
                }
            }
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "TouchL")
        {
            if (playProcedrual && continuous)
            {
                    HapticHelper.instance.ProceduralTone(true, pIntensity, pLength);
            }
        }
        if (col.tag == "TouchR")
        {
            if (playProcedrual && continuous)
            {
                HapticHelper.instance.ProceduralTone(false, pIntensity, pLength);
            }
        }
    }
}
