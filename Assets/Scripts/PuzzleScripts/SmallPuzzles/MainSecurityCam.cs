using UnityEngine;

public class MainSecurityCam : MonoBehaviour
{
    [Header("SecurityCamScript")]
    public SecurityCam securityCam;

    [Header("Security Camera Settings")]
    public float rotationSpeed = 30f; // Speed of camera rotation
    public float leftLimit = -206f;    // Left rotation limit
    public float rightLimit = -154f;    // Right rotation limit
    private float currentAngle = 0f;  // Current rotation angle
    private float currentRotation = 0f;
    private int rotationDirection = 1; // 1 for right, -1 for left

    [Header("Range Settings")]
    public float detectionRange = 10f; // Detection range of the camera
    public float detectionAngle = 45f; // Detection angle of the camera
    public float detectionRotation = 0f; // Detection rotation offset

    [Header("Player Detection")]
    public GameObject player;


    [Header("Cheak if player in range")]
    public bool playerInRange = false;

    [Header("Thing that activates the player detection")]
    public bool activateDetection = true;

    [Header("Make detection follow the player when detected")]
    public bool followPlayer = false;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the detection area back and forth between the left and right limits
        detectionRotation += rotationSpeed * rotationDirection * Time.deltaTime;
        if (detectionRotation >= rightLimit)
        {
            detectionRotation = rightLimit;
            rotationDirection = -1; // Change direction to left
        }
        else if (detectionRotation <= leftLimit)
        {
            detectionRotation = leftLimit;
            rotationDirection = 1; // Change direction to right
        }

    }

    void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.red;
        // Draw detection angle
        Vector3 leftBoundary = Quaternion.Euler(0, 0, -detectionAngle / 2 + detectionRotation) * transform.up * detectionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, detectionAngle / 2 + detectionRotation) * transform.up * detectionRange;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
}
