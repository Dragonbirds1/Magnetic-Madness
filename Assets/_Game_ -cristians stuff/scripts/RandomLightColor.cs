using System.Collections;
using UnityEngine;

public class RandomLightColor : MonoBehaviour
{
    // Reference to the Light component
    private UnityEngine.Rendering.Universal.Light2D light2D;

    // Time between color changes
    public float colorChangeDelay = 2.0f;

    void Start()
    {
        // Get the Light2D component attached to this GameObject
        light2D = GetComponent<UnityEngine.Rendering.Universal.Light2D>();

        // Start the coroutine to change colors
        if (light2D != null)
        {
            StartCoroutine(ChangeColorRoutine());
        }
        else
        {
            Debug.LogError("Light2D component not found! Please attach one to the GameObject.");
        }
    }

    IEnumerator ChangeColorRoutine()
    {
        while (true) // Infinite loop
        {
            // Wait for the specified delay before changing the color
            yield return new WaitForSeconds(colorChangeDelay);

            // Generate a new random color using Random.ColorHSV
            // This function generates a color with random H (hue), S (saturation), and V (value/brightness)
            // The parameters here (0f, 1f, 0.5f, 1f, 0.5f, 1f) ensure the color is vibrant and visible,
            // avoiding very dark or desaturated colors.
            Color newColor = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);

            // Apply the new color to the light
            light2D.color = newColor;
        }
    }
}