using UnityEngine;
using System.Collections; 
using UnityEngine.Rendering.Universal; 

public class TimedLightOff : MonoBehaviour
{
    public Light2D targetLight; // Assign your Light2D component here in the Inspector
    public float timeBeforeOff = 5f; // Time in seconds before the light turns off

    void Start()
    {
        // Start the coroutine when the script starts
        StartCoroutine(TurnOffLightAfterTime());
    }

    IEnumerator TurnOffLightAfterTime()
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(timeBeforeOff);

        // Turn off the light
        if (targetLight != null)
        {
            targetLight.enabled = false; // or targetLight.intensity = 0f;
        }
    }
}