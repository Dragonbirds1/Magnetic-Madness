using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class SecurityCam : MonoBehaviour
{
    enum CamState { Patrol, Alert, MakeSure, Search }
    CamState currentState = CamState.Patrol;

    [Header("Rotation Settings")]
    public float rotationSpeed = 30f;
    public float leftLimit = -206f;
    public float rightLimit = -154f;

    private float currentAngle;
    private int rotationDirection = 1;

    [Header("Smoothing")]
    public float rotationSmoothSpeed = 6f;

    private float targetAngle;


    [Header("Light Settings")]
    public Light2D spotlight;

    [Header("Detection")]
    public Transform player;
    public float detectionDistance = 8f;
    public LayerMask obstacleMask;

    [Header("Field of View")]
    public float viewAngle = 45f; // half-angle of spotlight cone

    [Header("Make Sure Settings")]
    public float makeSureDuration = 1.25f;
    private float makeSureTimer;

    [Header("Search Settings")]
    public float searchDuration = 3f;
    private float searchTimer;

    private float lastSeenAngle;

    private bool canDetect = false;

    IEnumerator Start()
    {
        currentAngle = transform.eulerAngles.z;
        targetAngle = currentAngle;
        yield return null; // wait 1 frame
        canDetect = true;
    }

    void Update()
    {
        DetectPlayer();

        switch (currentState)
        {
            case CamState.Patrol:
                Patrol();
                break;

            case CamState.Alert:
                FollowPlayer();
                break;

            case CamState.MakeSure:
                MakeSure();
                break;

            case CamState.Search:
                Search();
                break;
        }
    }

    // ---------------- DETECTION ----------------
    void DetectPlayer()
    {
        if (!canDetect) return;

        if (player == null) return;

        Vector2 dirToPlayer = player.position - spotlight.transform.position;

        // Distance check
        if (dirToPlayer.magnitude > detectionDistance)
        {
            if (currentState == CamState.Alert)
                EnterMakeSure();
            return;
        }

        // 🔑 ANGLE CHECK (prevents instant detection)
        Vector2 forward = spotlight.transform.right;
        float angleToPlayer = Vector2.Angle(forward, dirToPlayer);

        if (angleToPlayer > viewAngle)
        {
            if (currentState == CamState.Alert)
                EnterMakeSure();
            return;
        }

        // Line-of-sight check
        RaycastHit2D hit = Physics2D.Raycast(
            spotlight.transform.position,
            dirToPlayer.normalized,
            detectionDistance,
            obstacleMask
        );

        Debug.DrawRay(spotlight.transform.position, dirToPlayer, Color.red);

        if (hit && hit.transform == player)
        {
            lastSeenAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg - 90f;
            currentState = CamState.Alert;
        }
        else
        {
            if (currentState == CamState.Alert)
                EnterMakeSure();
        }
    }


    // ---------------- STATES ----------------
    void Patrol()
    {
        rotationSmoothSpeed = 4f;

        currentAngle += rotationSpeed * rotationDirection * Time.deltaTime;

        if (currentAngle >= rightLimit)
        {
            currentAngle = rightLimit;
            rotationDirection = -1;
        }
        else if (currentAngle <= leftLimit)
        {
            currentAngle = leftLimit;
            rotationDirection = 1;
        }

        ApplyRotation(currentAngle);
    }

    void FollowPlayer()
    {
        Debug.Log("ALERT: Player Detected!");
        rotationSmoothSpeed = 10f;

        Vector2 dir = player.position - spotlight.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

        lastSeenAngle = angle;
        ApplyRotation(angle);
    }

    void MakeSure()
    {
        makeSureTimer -= Time.deltaTime;

        // Hold steady on last seen position
        ApplyRotation(lastSeenAngle);

        if (makeSureTimer <= 0f)
        {
            EnterSearch();
        }
    }

    void Search()
    {
        rotationSmoothSpeed = 5f;

        searchTimer -= Time.deltaTime;

        float searchAngle = lastSeenAngle + Mathf.Sin(Time.time * 2f) * 25f;
        ApplyRotation(searchAngle);

        if (searchTimer <= 0f)
        {
            currentState = CamState.Patrol;
        }
    }

    // ---------------- HELPERS ----------------
    void EnterMakeSure()
    {
        currentState = CamState.MakeSure;
        makeSureTimer = makeSureDuration;
    }

    void EnterSearch()
    {
        currentState = CamState.Search;
        searchTimer = searchDuration;
    }

    void ApplyRotation(float angle)
    {
        targetAngle = angle;

        currentAngle = Mathf.LerpAngle(
            currentAngle,
            targetAngle,
            rotationSmoothSpeed * Time.deltaTime
        );

        Quaternion rot = Quaternion.Euler(0, 0, currentAngle);
        transform.rotation = rot;
        spotlight.transform.rotation = rot;
    }
}
