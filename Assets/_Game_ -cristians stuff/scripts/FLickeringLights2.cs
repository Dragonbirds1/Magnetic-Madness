using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;
public class FLickeringLights2 : MonoBehaviour
{
    public Light2D targetLight; // Assign your 2D Light in the Inspector
    public float flickerDuration = 1.0f; // How long the light flickers
    public float minFlickerIntensity = 0.5f; // Minimum intensity during flicker
    public float maxFlickerIntensity = 1.5f; // Maximum intensity during flicker
    public float flickerDelayMin = 0.05f; // Minimum time between intensity changes
    public float flickerDelayMax = 0.15f; // Maximum time between intensity changes
    public float finalIntensity = 1.0f; // Intensity after flickering stops

    private bool isFlickering = false;

    void Start()
    {
        if (targetLight == null)
        {
            targetLight = GetComponent<Light2D>();
            if (targetLight == null)
            {
                Debug.LogError("No Light2D component found or assigned to FlickerLight script.");
                enabled = false; // Disable script if no light is found
                return;
            }
        }
        StartCoroutine(FlickerAndStayOn());
    }

    IEnumerator FlickerAndStayOn()
    {
        isFlickering = true;
        float startTime = Time.time;

        while (Time.time < startTime + flickerDuration)
        {
            targetLight.intensity = Random.Range(minFlickerIntensity, maxFlickerIntensity);
            yield return new WaitForSeconds(Random.Range(flickerDelayMin, flickerDelayMax));
        }

        // Ensure the light is set to the final intensity after flickering
        targetLight.intensity = finalIntensity;
        isFlickering = false;
    }
}