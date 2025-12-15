using UnityEngine;
using System.Collections; // Required for IEnumerator

public class LightRotator : MonoBehaviour
{
    // Adjust these in the Inspector to control the rotation range and speed
    public float minAngle = -45f; // Max left rotation
    public float maxAngle = 45f; // Max right rotation
    public float rotationSpeed = 2f; // Speed of rotation
    public float newTargetInterval = 3f; // Time until a new target angle is chosen

    private Quaternion targetRotation;
    private Quaternion originalRotation;

    void Start()
    {
        // Store the original rotation as the center point
        originalRotation = transform.rotation;
        // Start the coroutine to pick new random targets
        StartCoroutine(PickNewTargetRotation());
    }

    void Update()
    {
        // Smoothly rotate towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    IEnumerator PickNewTargetRotation()
    {
        while (true)
        {
            // Generate a random angle within the specified range (minAngle to maxAngle)
            float randomAngle = Random.Range(minAngle, maxAngle);
            // Calculate the new target rotation around the Z-axis
            targetRotation = originalRotation * Quaternion.Euler(0, 0, randomAngle);

            // Wait for the specified interval before picking a new target
            yield return new WaitForSeconds(newTargetInterval);
        }
    }
}