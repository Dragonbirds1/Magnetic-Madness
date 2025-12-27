using UnityEngine;

public class HideAndSeekEnemy : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Alert,
        Search,
        Chase
    }

    public EnemyState currentState = EnemyState.Patrol;

    [Header("Vision Light Offset")]
    [Tooltip("Rotation offset to align Light2D with visionDirection")]
    public float lightRotationOffset = -90f;

    [Header("Hide and Seek Enemy GameObject")]
    public GameObject hideAndSeekEnemy;

    [Header("Player Death Scripts")]
    public Player1Death player1Death;
    public Player2Death player2Death;

    [Header("Enemy Animator")]
    public Animator enemyAnimator;

    [Header("Player GameObject")]
    public GameObject player;

    [Header("Waypoints")]
    public Transform[] waypoints;

    [Header("Movement Speed")]
    public float movementSpeed = 2f;

    [Header("Detection")]
    public float detectionRadius;

    [Header("Vision Cone")]
    public float viewDistance = 6f;
    public float viewAngle = 60f;
    public float visionRotateSpeed = 90f;
    public LayerMask obstacleMask;
    public LayerMask playerMask;

    [Header("State Timers")]
    public float alertDuration = 1.5f;
    public float searchDuration = 4f;

    [Header("Vision Light")]
    public UnityEngine.Rendering.Universal.Light2D visionLight;

    [Header("Chase Settings")]
    public float chaseSpeedMultiplier = 2.2f;
    public float lostSightGraceTime = 1.5f;

    [Header("Vision Light Colors")]
    public Color patrolColor = Color.white;
    public Color alertColor = Color.yellow;
    public Color chaseColor = Color.red;
    public Color searchColor = new Color(1f, 0.6f, 0f); // orange

    [Header("Vision Light Effects")]
    public float colorLerpSpeed = 5f;
    public float patrolViewAngle = 60f;
    public float chaseViewAngle = 90f;
    public float alertPulseSpeed = 6f;
    public float alertPulseAmount = 0.15f;

    private Color targetLightColor;
    private float baseLightIntensity;

    private float lostSightTimer;



    private float stateTimer;
    private Vector3 lastKnownPlayerPos;
    private float searchSweepAngle = 45f;
    private float sweepDirection = 1f;


    private float currentVisionAngle = 0f;


    private int currentWaypointIndex = 0;

    private Vector2 visionDirection = Vector2.right;

    public bool isMoving = false;
    public bool runPlayerRun = false;

    private bool isPlayerHidden = false;   // 🔴 IMPORTANT
    private bool PLAYONCE = false;

    void Start()
    {
        if (visionLight != null)
            baseLightIntensity = visionLight.intensity;
    }

    void Update()
    {
        if (isMoving)
        {
            switch (currentState)
            {
                case EnemyState.Patrol:
                    PatrolState();
                    break;
                case EnemyState.Alert:
                    AlertState();
                    break;
                case EnemyState.Chase:
                    ChaseState();
                    break;
                case EnemyState.Search:
                    SearchState();
                    break;
            }
        }
        UpdateVisionLight();
    }


    void PatrolState()
    {
        enemyAnimator.SetBool("Idle", false);
        enemyAnimator.SetBool("Down", true);

        hideAndSeekEnemy.transform.position =
            Vector3.MoveTowards(
                hideAndSeekEnemy.transform.position,
                waypoints[currentWaypointIndex].position,
                movementSpeed * Time.deltaTime
            );

        if (CanSeePlayer())
        {
            lastKnownPlayerPos = player.transform.position;
            stateTimer = alertDuration;
            currentState = EnemyState.Alert;
            playSFX();
            return;
        }

        if (Vector3.Distance(hideAndSeekEnemy.transform.position,
            waypoints[currentWaypointIndex].position) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            RotateVision(90f);
        }
    }

    void RotateVision(float angle)
    {
        visionLight.transform.localRotation =
        Quaternion.Euler(0f, 0f, angle + lightRotationOffset);

    }


    void AlertState()
    {
        enemyAnimator.SetBool("Idle", true);
        enemyAnimator.SetBool("Down", false);

        stateTimer -= Time.deltaTime;

        if (CanSeePlayer())
        {
            lastKnownPlayerPos = player.transform.position;
            stateTimer = alertDuration;
            return;
        }

        if (stateTimer <= 0f)
        {
            stateTimer = searchDuration;
            currentState = EnemyState.Search;
        }
    }

    void SearchState()
    {
        enemyAnimator.SetBool("Idle", false);
        enemyAnimator.SetBool("Down", true);

        hideAndSeekEnemy.transform.position =
            Vector3.MoveTowards(
                hideAndSeekEnemy.transform.position,
                lastKnownPlayerPos,
                movementSpeed * 1.2f * Time.deltaTime
            );

        // Sweep vision cone
        visionDirection =
        Quaternion.Euler(0, 0, sweepDirection * visionRotateSpeed * Time.deltaTime)
        * visionDirection;

        if (Mathf.Abs(currentVisionAngle) > searchSweepAngle)
            sweepDirection *= -1f;

        if (CanSeePlayer())
        {
            lastKnownPlayerPos = player.transform.position;
            lostSightTimer = lostSightGraceTime;
            currentState = EnemyState.Chase;
            return;
        }

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            currentState = EnemyState.Patrol;
        }
    }

    void ChaseState()
    {
        enemyAnimator.SetBool("Idle", false);
        enemyAnimator.SetBool("Down", true);

        // Move toward player
        hideAndSeekEnemy.transform.position =
            Vector3.MoveTowards(
                hideAndSeekEnemy.transform.position,
                player.transform.position,
                movementSpeed * chaseSpeedMultiplier * Time.deltaTime
            );

        // Lock vision cone onto player
        visionDirection = (player.transform.position - transform.position).normalized;

        if (CanSeePlayer())
        {
            lastKnownPlayerPos = player.transform.position;
            lostSightTimer = lostSightGraceTime;
            return;
        }

        // Lost sight countdown
        lostSightTimer -= Time.deltaTime;
        if (lostSightTimer <= 0f)
        {
            stateTimer = searchDuration;
            currentState = EnemyState.Search;
        }
    }


    bool CanSeePlayer()
    {
        if (isPlayerHidden) return false;

        Vector2 dir = (player.transform.position - transform.position).normalized;
        float dist = Vector2.Distance(transform.position, player.transform.position);

        if (dist > viewDistance) return false;

        float angle = Vector2.Angle(visionDirection, dir);
        if (angle > viewAngle / 2f) return false;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            dir,
            viewDistance,
            obstacleMask | playerMask
        );

        return hit && hit.collider.CompareTag("Player");
    }

    // 🔴 HIDING LOGIC
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("HideZone") && other.gameObject == player)
        {
            isPlayerHidden = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("HideZone") && other.gameObject == player)
        {
            isPlayerHidden = false;
        }
    }

    void playSFX()
    {
        // your audio logic
    }

    void UpdateVisionLight()
    {
        if (visionLight == null) return;

        // Light2D points along +Y by default
        // So compute the angle between Vector2.up (light forward) and visionDirection
        float angle = Vector2.SignedAngle(Vector2.up, visionDirection);

        // Apply rotation relative to parent
        visionLight.transform.localRotation = Quaternion.Euler(0f, 0f, angle);

        // Update light cone shape
        visionLight.pointLightOuterAngle = viewAngle;
        visionLight.pointLightInnerAngle = viewAngle * 0.8f;
        visionLight.pointLightOuterRadius = viewDistance;

        // Smooth color transition and pulse
        switch (currentState)
        {
            case EnemyState.Patrol:
                targetLightColor = patrolColor;
                visionLight.intensity = baseLightIntensity;
                break;
            case EnemyState.Alert:
                targetLightColor = alertColor;
                visionLight.intensity =
                    baseLightIntensity + Mathf.Sin(Time.time * alertPulseSpeed) * alertPulseAmount;
                break;
            case EnemyState.Chase:
                targetLightColor = chaseColor;
                visionLight.intensity = baseLightIntensity;
                break;
            case EnemyState.Search:
                targetLightColor = searchColor;
                visionLight.intensity = baseLightIntensity;
                break;
        }

        visionLight.color = Color.Lerp(
            visionLight.color,
            targetLightColor,
            Time.deltaTime * colorLerpSpeed
        );
    }



    void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.yellow;

        Vector3 left = Quaternion.Euler(0, 0, -viewAngle / 2) * visionDirection;
        Vector3 right = Quaternion.Euler(0, 0, viewAngle / 2) * visionDirection;

        Gizmos.DrawLine(transform.position, transform.position + left * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + right * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)visionDirection * viewDistance);
    }
}
