
using UnityEngine;

public class OculusHapticsController : MonoBehaviour
{
    [SerializeField]
    OVRInput.Controller controllerMask;

    private OVRHapticsClip clipLight;
    private OVRHapticsClip clipMedium;
    private OVRHapticsClip clipHard;

    private void Start()
    {
        InitializeOVRHaptics();
    }

    private void InitializeOVRHaptics()
    {
        int cnt = 10;
        clipLight = new OVRHapticsClip(cnt);
        clipMedium = new OVRHapticsClip(cnt);
        clipHard = new OVRHapticsClip(cnt);
        for (int i = 0; i < cnt; i++)
        {
            clipLight.Samples[i] = i % 2 == 0 ? (byte)0 : (byte)75;
            clipMedium.Samples[i] = i % 2 == 0 ? (byte)0 : (byte)150;
            clipHard.Samples[i] = i % 2 == 0 ? (byte)0 : (byte)255;
        }

        clipLight = new OVRHapticsClip(clipLight.Samples, clipLight.Samples.Length);
        clipMedium = new OVRHapticsClip(clipMedium.Samples, clipMedium.Samples.Length);
        clipHard = new OVRHapticsClip(clipHard.Samples, clipHard.Samples.Length);
    }

    public void Vibrate(VibrationForce vibrationForce)
    {
        var channel = OVRHaptics.RightChannel;
        if (controllerMask == OVRInput.Controller.LTouch)
            channel = OVRHaptics.LeftChannel;

        switch (vibrationForce)
        {
            case VibrationForce.Light:
                channel.Preempt(clipLight);
                break;
            case VibrationForce.Medium:
                channel.Preempt(clipMedium);
                break;
            case VibrationForce.Hard:
                channel.Preempt(clipHard);
                break;
        }
    }
}