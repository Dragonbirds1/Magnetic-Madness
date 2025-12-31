using UnityEngine;

public class GolfCamera : MonoBehaviour
{
    [Header("References")]
    public Transform targetBall;
    public Transform player;
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Follow Settings")]
    public float followSpeed = 5f;
    public float lookAheadFactor = 0.5f;
    [HideInInspector] public bool followBall = false;

    [Header("Zoom Settings")]
    public float baseSize = 5f;
    public float maxSize = 7f;
    public float zoomSpeed = 2f;
    public float speedToZoom = 10f;

    [Header("Shake Settings")]
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.2f;

    private Camera cam;
    private Vector3 velocity = Vector3.zero;
    private float shakeTimer = 0f;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
    }

    void LateUpdate()
    {
        if (targetBall == null || player == null) return;

        Vector3 targetPos;
        float targetSize;

        if (followBall)
        {
            // Get ball velocity from MiniGolf script
            MiniGolf golfScript = targetBall.GetComponent<MiniGolf>();
            Vector2 ballVel = golfScript != null ? golfScript.velocity : Vector2.zero;

            Vector3 lookAhead = new Vector3(ballVel.x, ballVel.y, 0f) * lookAheadFactor;
            targetPos = targetBall.position + lookAhead + offset;

            // Zoom based on ball speed
            float ballSpeed = ballVel.magnitude;
            targetSize = Mathf.Lerp(baseSize, maxSize, Mathf.Clamp01(ballSpeed / speedToZoom));
        }
        else
        {
            // Reset to player
            targetPos = player.position + offset;
            targetSize = baseSize;
        }

        // Smooth position
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 1f / followSpeed);

        // Smooth zoom
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);

        // Shake effect
        if (shakeTimer > 0f)
        {
            Vector3 shakeOffset = Random.insideUnitCircle * shakeMagnitude;
            transform.position += shakeOffset;
            shakeTimer -= Time.deltaTime;
        }
    }

    public void ShakeCamera()
    {
        shakeTimer = shakeDuration;
    }
}
