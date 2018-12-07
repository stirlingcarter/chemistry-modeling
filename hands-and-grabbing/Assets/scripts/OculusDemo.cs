using UnityEngine;

public class OculusDemo : MonoBehaviour
{
    [SerializeField]
    OculusHapticsController leftControllerHaptics;

    [SerializeField]
    OculusHapticsController rightControllerHaptics;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            leftControllerHaptics.Vibrate(VibrationForce.Hard);
            rightControllerHaptics.Vibrate(VibrationForce.Light);
        }
    }
}