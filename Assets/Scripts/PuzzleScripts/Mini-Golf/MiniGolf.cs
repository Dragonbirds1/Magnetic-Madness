using UnityEngine;

public class MiniGolf : MonoBehaviour
{
    [Header("References")]
    public Transform visual; // child sprite for spin
    public Transform arrow;
    public GameObject player;
    public GolfCamera golfCam;

    [Header("Power Settings")]
    public float powerMultiplier;
    public float maxPower;

    [Header("Ball Settings")]
    public float stopThreshold = 0.02f;
    public float friction;
    public float radToHit;
    public float ballRadius = 0.25f; // for spin scaling

    [Header("Spin Settings")]
    public float spinAcceleration = 10f;
    private float currentSpinSpeed = 0f;

    [Header("Multi-Hole & Spiral")]
    public Transform[] holes;
    public Transform[] courseSpawns;
    private int currentSpawnIndex = 0;
    public float spiralSpeed = 720f;
    public float spiralShrink = 0.5f;
    public float spiralDistance = 0.1f;
    private bool spiraling = false;
    private Transform targetHole = null;

    private Vector2 dragStart;
    private bool isAiming;

    [HideInInspector] public Vector2 velocity;

    private CircleCollider2D ballCollider;

    void Start()
    {
        ballCollider = GetComponent<CircleCollider2D>();
        if (ballCollider == null)
            ballCollider = gameObject.AddComponent<CircleCollider2D>();
    }

    void Update()
    {
        HandleInput();
        HandleMovement();
        UpdateVisualSpin();
        HandleHoleSpiral();
    }

    // -----------------------------
    // Input & Aiming
    // -----------------------------
    void HandleInput()
    {
        if (spiraling) return;
        float playerDist = Vector2.Distance(transform.position, player.transform.position);
        if (playerDist > radToHit) return;

        if (velocity.magnitude > stopThreshold)
        {
            arrow.gameObject.SetActive(false);
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            dragStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isAiming = true;
            arrow.gameObject.SetActive(true);
            if (golfCam != null) golfCam.followBall = false;
        }

        if (Input.GetMouseButton(0) && isAiming)
        {
            Vector2 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = dragStart - currentPos;
            float power = Mathf.Clamp(direction.magnitude * powerMultiplier, 0, maxPower);

            arrow.position = transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            arrow.rotation = Quaternion.Euler(0, 0, angle);
            arrow.localScale = new Vector3(1f, power, 1f);
        }

        if (Input.GetMouseButtonUp(0) && isAiming)
        {
            ShootBall();
            isAiming = false;
            arrow.gameObject.SetActive(false);
        }
    }

    void ShootBall()
    {
        Vector2 endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dragDir = dragStart - endPos;
        float dragDistance = dragDir.magnitude;
        float power = Mathf.Clamp(dragDistance * powerMultiplier, 0, maxPower);

        velocity = dragDir.normalized * power;

        if (golfCam != null)
        {
            golfCam.ShakeCamera();
            golfCam.followBall = true;
        }
    }

    // -----------------------------
    // Movement & Manual Collisions
    // -----------------------------
    void HandleMovement()
    {
        if (spiraling || velocity.magnitude < 0.001f) return;
        Vector2 move = velocity * Time.deltaTime;

        // Manual collision detection
        RaycastHit2D[] hits = new RaycastHit2D[5];
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Obstacles")); // assign walls to "Obstacles" layer
        filter.useTriggers = false;

        int hitCount = ballCollider.Cast(move.normalized, filter, hits, move.magnitude + 0.01f);

        if (hitCount > 0)
        {
            Vector2 normal = hits[0].normal;
            velocity = Vector2.Reflect(velocity, normal) * 0.8f; // bounce damping
            transform.position = (Vector2)hits[0].point + normal * (ballCollider.bounds.extents.x + 0.001f);
        }
        else
        {
            transform.position += (Vector3)move;
        }

        // Apply friction
        velocity = Vector2.MoveTowards(velocity, Vector2.zero, friction * Time.deltaTime);

        if (velocity.magnitude <= stopThreshold)
        {
            velocity = Vector2.zero;
            if (golfCam != null) golfCam.followBall = false;
        }
    }

    // -----------------------------
    // Spin
    // -----------------------------
    void UpdateVisualSpin()
    {
        if (visual == null) return;
        float speed = velocity.magnitude;
        if (speed < stopThreshold) speed = 0f;

        float targetSpinSpeed = (speed / (2f * Mathf.PI * ballRadius)) * 360f;
        float spinDir = (speed > 0f) ? Mathf.Sign(velocity.x + velocity.y) : 0f;
        currentSpinSpeed = Mathf.Lerp(currentSpinSpeed, targetSpinSpeed * spinDir, spinAcceleration * Time.deltaTime);

        visual.Rotate(0f, 0f, -currentSpinSpeed * Time.deltaTime);
    }

    // -----------------------------
    // Hole Spiral
    // -----------------------------
    void HandleHoleSpiral()
    {
        if (holes == null || holes.Length == 0) return;

        float minDist = float.MaxValue;
        Transform closestHole = null;
        foreach (Transform h in holes)
        {
            float dist = Vector2.Distance(visual.position, h.position);
            if (dist < minDist)
            {
                minDist = dist;
                closestHole = h;
            }
        }

        if (!spiraling && minDist <= spiralDistance)
        {
            spiraling = true;
            targetHole = closestHole;
            velocity = Vector2.zero;
        }

        if (!spiraling || targetHole == null) return;

        visual.RotateAround(targetHole.position, Vector3.forward, spiralSpeed * Time.deltaTime);
        visual.position = Vector2.Lerp(visual.position, targetHole.position, 5f * Time.deltaTime);
        visual.localScale = Vector3.Lerp(visual.localScale, Vector3.one * spiralShrink, 5f * Time.deltaTime);

        if (Vector2.Distance(visual.position, targetHole.position) < 0.02f)
        {
            spiraling = false;
            targetHole = null;

            // Move to next spawn
            currentSpawnIndex = (currentSpawnIndex + 1) % courseSpawns.Length;
            Transform nextSpawn = courseSpawns[currentSpawnIndex];
            transform.position = nextSpawn.position;
            visual.position = transform.position;
            visual.localScale = Vector3.one;
            velocity = Vector2.zero;

            if (golfCam != null) golfCam.followBall = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radToHit);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spiralDistance);
    }
}
