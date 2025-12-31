using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class SecurityCam : MonoBehaviour
{
    enum CamState { Patrol, MakeSure, Alert, Search }
    CamState currentState = CamState.Patrol;

    private bool hasAlerted = false;

    [Header("Rotation")]
    public float leftLimit = -236f;
    public float rightLimit = -154f;
    public float patrolSpeed = 30f;

    private float currentAngle;
    private float targetAngle;
    private int dir = 1;

    [Header("Smoothing")]
    public float patrolSmooth = 4f;
    public float alertSmooth = 10f;
    public float searchSmooth = 5f;

    [Header("Detection")]
    public Transform player;
    public float detectionDistance = 12f;
    public float viewAngle = 45f;
    public LayerMask obstacleMask;

    [Header("Timing")]
    public float makeSureMin = 1f;
    public float makeSureMax = 1.5f;
    public float searchDuration = 3f;

    private float makeSureTimer;
    private float searchTimer;
    private float lastSeenAngle;
    private bool playerDetected;

    [Header("Light")]
    public Light2D spotlight;
    public float lightSmooth = 6f;

    public Color patrolColor = Color.white;
    public Color makeSureColor = Color.yellow;
    public Color alertColor = Color.red;
    public Color searchColor = new Color(1f, 0.5f, 0f);

    private Color targetLight;

    [Header("Startup Delay")]
    public float startupDelay = 0.5f; // seconds
    private bool canDetect = false;

    [Header("SFX")]
    public AudioSource audioSource;
    public AudioClip patrolSFX;
    public AudioClip makeSureSFX;
    public AudioClip alertSFX;
    public AudioClip searchSFX;

    [Header("Audio Distance Settings")]
    public float maxHearingDistance = 8f;      // for Patrol/Search/MakeSure
    public float alertMaxHearingDistance = 12f; // for Alert
    public float minVolume = 0f;
    public float maxVolume = 1f;

    IEnumerator Start()
    {
        currentAngle = transform.eulerAngles.z;
        targetAngle = currentAngle;
        spotlight.color = patrolColor;

        yield return new WaitForSeconds(startupDelay);
        canDetect = true;

        EnterPatrol();
    }

    void Update()
    {
        if (canDetect)
            HandleDetection();

        switch (currentState)
        {
            case CamState.Patrol: Patrol(); break;
            case CamState.MakeSure: MakeSure(); break;
            case CamState.Alert: Alert(); break;
            case CamState.Search: Search(); break;
        }

        RotateSmooth();
        UpdateLight();
        UpdateVolumeBasedOnDistance();
    }

    // ================= DETECTION =================
    bool PlayerInSight()
    {
        if (player == null) return false;

        Vector2 dirToPlayer = player.position - spotlight.transform.position;
        float distance = dirToPlayer.magnitude;

        if (distance > detectionDistance) return false;

        Vector2 forward = spotlight.transform.up;
        float angle = Vector2.Angle(forward, dirToPlayer);
        if (angle > viewAngle) return false;

        RaycastHit2D hit = Physics2D.Raycast(
            spotlight.transform.position,
            dirToPlayer.normalized,
            distance,
            obstacleMask
        );

        if (hit.collider != null) return false;

        return true;
    }

    void HandleDetection()
    {
        if (player == null) return;

        bool seenNow = PlayerInSight();
        playerDetected = seenNow;

        if (seenNow)
        {
            Vector2 dirToPlayer = player.position - spotlight.transform.position;
            lastSeenAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg - 90f;

            // Only enter MakeSure from Patrol or Search
            if (currentState == CamState.Patrol || currentState == CamState.Search)
            {
                EnterMakeSure();
            }
        }
        else
        {
            // Lost sight while in Alert → MakeSure
            if (currentState == CamState.Alert)
            {
                EnterMakeSure();
            }
        }
    }


    // ================= STATES =================
    void Patrol()
    {
        targetAngle += patrolSpeed * dir * Time.deltaTime;
        if (targetAngle >= rightLimit) { targetAngle = rightLimit; dir = -1; }
        if (targetAngle <= leftLimit) { targetAngle = leftLimit; dir = 1; }
    }

    void MakeSure()
    {
        targetAngle = lastSeenAngle;
        makeSureTimer -= Time.deltaTime;

        if (makeSureTimer <= 0f)
        {
            if (playerDetected)
                EnterAlert();
            else
                EnterSearch();
        }
    }

    void Alert()
    {
        if (playerDetected)
        {
            Vector2 dirToPlayer = player.position - spotlight.transform.position;
            targetAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg - 90f;
            lastSeenAngle = targetAngle;
        }
        else
        {
            EnterMakeSure();
        }
    }

    void Search()
    {
        searchTimer -= Time.deltaTime;
        targetAngle = lastSeenAngle + Mathf.Sin(Time.time * 2f) * 25f;

        if (searchTimer <= 0f)
            EnterPatrol();
    }

    // ================= HELPERS =================
    void EnterMakeSure()
    {
        if (currentState == CamState.MakeSure) return;

        if (currentState == CamState.Alert)
            ExitAlert();

        StopAudio();
        currentState = CamState.MakeSure;
        makeSureTimer = Random.Range(makeSureMin, makeSureMax);
        PlayStateAudio(makeSureSFX, false);
    }

    void EnterSearch()
    {
        if (currentState == CamState.Alert)
            ExitAlert();

        StopAudio();
        currentState = CamState.Search;
        searchTimer = searchDuration;
        PlayStateAudio(searchSFX, true);
    }

    void EnterAlert()
    {
        if (hasAlerted) return;

        StopAudio();

        hasAlerted = true;
        currentState = CamState.Alert;

        PlayStateAudio(alertSFX, true);

        Debug.Log($"CAM ALERT FIRED by {name}");
        SecurityAlertSystem.OnCameraAlert?.Invoke(transform.position);
    }

    void ExitAlert()
    {
        hasAlerted = false;
    }

    void EnterPatrol()
    {
        ExitAlert();
        StopAudio();
        currentState = CamState.Patrol;
        PlayStateAudio(patrolSFX, true);
    }

    void RotateSmooth()
    {
        float smooth = patrolSmooth;
        if (currentState == CamState.Alert) smooth = alertSmooth;
        if (currentState == CamState.Search) smooth = searchSmooth;

        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, smooth * Time.deltaTime);
        Quaternion rot = Quaternion.Euler(0, 0, currentAngle);
        transform.rotation = rot;
        spotlight.transform.rotation = rot;
    }

    void UpdateLight()
    {
        switch (currentState)
        {
            case CamState.Patrol: targetLight = patrolColor; break;
            case CamState.MakeSure: targetLight = makeSureColor; break;
            case CamState.Alert: targetLight = alertColor; break;
            case CamState.Search: targetLight = searchColor; break;
        }

        spotlight.color = Color.Lerp(spotlight.color, targetLight, lightSmooth * Time.deltaTime);
    }

    // ================= AUDIO =================
    void PlayStateAudio(AudioClip clip, bool loop)
    {
        if (audioSource == null || clip == null) return;

        audioSource.loop = loop;
        audioSource.clip = clip;
        audioSource.Play();
    }

    void StopAudio()
    {
        if (audioSource == null) return;
        audioSource.Stop();
        audioSource.clip = null;
    }

    void UpdateVolumeBasedOnDistance()
    {
        if (audioSource == null || player == null || !audioSource.isPlaying) return;

        float distance = Vector2.Distance(transform.position, player.position);

        float effectiveMaxDistance = (currentState == CamState.Alert) ? alertMaxHearingDistance : maxHearingDistance;

        float t = Mathf.Clamp01(distance / effectiveMaxDistance); // 0 = close, 1 = far
        audioSource.volume = Mathf.Lerp(maxVolume, minVolume, t);
    }

    // ================= FOV DEBUG =================
    void OnDrawGizmosSelected()
    {
        if (spotlight == null) return;

        Vector3 position = spotlight.transform.position;
        Vector3 forward = spotlight.transform.up;

        // FOV lines
        Gizmos.color = Color.green;
        Quaternion leftRotation = Quaternion.AngleAxis(-viewAngle, Vector3.forward);
        Quaternion rightRotation = Quaternion.AngleAxis(viewAngle, Vector3.forward);

        Vector3 leftDir = leftRotation * forward;
        Vector3 rightDir = rightRotation * forward;

        Gizmos.DrawLine(position, position + leftDir * detectionDistance);
        Gizmos.DrawLine(position, position + rightDir * detectionDistance);

        int segments = 20;
        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float angle = Mathf.Lerp(-viewAngle, viewAngle, t);
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.forward) * forward;
            Gizmos.DrawLine(position, position + dir * detectionDistance);
        }

        // Max hearing distance visualization
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(position, maxHearingDistance);

        // Separate Alert max hearing distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(position, alertMaxHearingDistance);
    }
}
