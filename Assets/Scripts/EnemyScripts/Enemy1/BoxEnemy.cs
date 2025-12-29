using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class BoxEnemy : MonoBehaviour
{
    [Header("Patrol Nodes")]
    public List<Transform> waypoints;
    private Transform currentTarget;

    [Header("Chase")]
    public GameObject player, player2;
    public bool player1IsDead, player2IsDead;
    public float detectionRange = 5f;

    [Header("Animations")]
    public Animator enemyAnim;

    [Header("Sleep / Idle")]
    public float minSleepTime = 2f, maxSleepTime = 5f;
    public float minIdleTime = 1f, maxIdleTime = 3f;
    private bool isSleeping = false, isIdling = false;
    private float sleepTimer = 0f, idleTimer = 0f;
    [Range(0f, 1f)] public float baseSleepChance = 0.3f;
    public float sleepChanceIncrement = 0.05f;
    private float timeSinceLastSleep = 0f;

    [Header("Chase / Dash")]
    public float chaseSpeed = 3f;
    public float dashSpeed = 5f;
    public float timeTillDash = 2f, dashTime = 1f;

    [Header("Gameplay Freeze")]
    public WackAMole wackAMole;

    private bool playerFound = false, player2Found = false, isChasing = false;
    private float chaseTimer = 5f;

    private AIPath aiPath;

    [Header("Wall Push")]
    public LayerMask wallLayer;
    public float pushStrength = 0.05f;
    public float pushRadius = 0.1f;

    void Awake()
    {
        aiPath = GetComponent<AIPath>();

        aiPath.canMove = true;
        aiPath.canSearch = true;
        aiPath.updateRotation = false; // top-down 2D
        aiPath.enableRotation = false;
        aiPath.endReachedDistance = 0.1f;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Start()
    {
        if (waypoints.Count > 0)
            PickRandomNode();
    }

    void Update()
    {
        if (wackAMole.freezeGameplay) return;

        if (!isSleeping && !isIdling)
            timeSinceLastSleep += Time.deltaTime;

        // Detect players
        playerFound = PlayerInRange(player, player1IsDead);
        player2Found = PlayerInRange(player2, player2IsDead);

        if (playerFound || player2Found)
        {
            isChasing = true;
            chaseTimer -= Time.deltaTime;
        }
        else if (chaseTimer <= 0f)
        {
            isChasing = false;
            chaseTimer = 5f;
        }

        HandleState();
        PushFromWalls(); // handle tiny overlaps
    }

    bool PlayerInRange(GameObject target, bool isDead)
    {
        return target != null && !isDead && Vector2.Distance(transform.position, target.transform.position) <= detectionRange;
    }

    void HandleState()
    {
        if (isChasing)
            HandleChase();
        else
            PatrolNode();
    }

    void HandleChase()
    {
        Vector3 targetPos = playerFound ? player.transform.position :
                             player2Found ? player2.transform.position : transform.position;

        aiPath.maxSpeed = chaseSpeed;
        aiPath.destination = targetPos;
        enemyAnim.SetBool("isWalk", true);

        Dash();
    }

    void PatrolNode()
    {
        if (currentTarget == null || waypoints.Count == 0) return;

        // Sleep
        if (isSleeping)
        {
            sleepTimer -= Time.deltaTime;
            enemyAnim.SetBool("isSleep", true);
            enemyAnim.SetBool("isWalk", false);
            enemyAnim.SetBool("isIdle", false);

            if (sleepTimer <= 0f) WakeUp();
            return;
        }

        // Idle
        if (isIdling)
        {
            idleTimer -= Time.deltaTime;
            enemyAnim.SetBool("isIdle", true);
            enemyAnim.SetBool("isWalk", false);
            enemyAnim.SetBool("isSleep", false);

            if (idleTimer <= 0f)
            {
                isIdling = false;
                PickRandomNode();
            }
            return;
        }

        // Move to node
        aiPath.maxSpeed = chaseSpeed;
        aiPath.destination = currentTarget.position;
        enemyAnim.SetBool("isWalk", true);
        enemyAnim.SetBool("isIdle", false);
        enemyAnim.SetBool("isSleep", false);

        if (!aiPath.pathPending && aiPath.reachedEndOfPath)
        {
            float currentSleepChance = Mathf.Clamp(baseSleepChance + timeSinceLastSleep * sleepChanceIncrement, 0f, 0.9f);
            if (Random.value < currentSleepChance)
            {
                StartSleep();
                timeSinceLastSleep = 0f;
            }
            else
            {
                StartIdle();
            }
        }
    }

    void StartSleep()
    {
        isSleeping = true;
        sleepTimer = Random.Range(minSleepTime, maxSleepTime);
        enemyAnim.SetBool("isSleep", true);
        enemyAnim.SetBool("isWalk", false);
        enemyAnim.SetBool("isIdle", false);
    }

    void WakeUp()
    {
        isSleeping = false;
        enemyAnim.SetBool("isSleep", false);
        enemyAnim.SetBool("isWake", true);
        StartCoroutine(WakeCoroutine());
    }

    IEnumerator WakeCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        enemyAnim.SetBool("isWake", false);
        enemyAnim.SetBool("isIdle", true);
        PickRandomNode();
    }

    void StartIdle()
    {
        isIdling = true;
        idleTimer = Random.Range(minIdleTime, maxIdleTime);
        enemyAnim.SetBool("isIdle", true);
        enemyAnim.SetBool("isWalk", false);
        enemyAnim.SetBool("isSleep", false);
    }

    void PickRandomNode()
    {
        if (waypoints.Count == 0) return;
        Transform newTarget = currentTarget;
        int attempts = 0;

        while (newTarget == currentTarget && attempts < 10)
        {
            newTarget = waypoints[Random.Range(0, waypoints.Count)];
            attempts++;
        }

        currentTarget = newTarget;
    }

    void Dash()
    {
        timeTillDash -= Time.deltaTime;
        if (timeTillDash <= 0)
        {
            aiPath.maxSpeed = dashSpeed;
            dashTime -= Time.deltaTime;
            if (dashTime <= 0)
            {
                aiPath.maxSpeed = chaseSpeed;
                timeTillDash = 2f;
                dashTime = 1f;
            }
        }
    }

    // Small push to avoid minor wall overlaps
    void PushFromWalls()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, pushRadius, wallLayer);
        if (hit != null)
        {
            Vector2 pushDir = ((Vector2)transform.position - (Vector2)hit.ClosestPoint(transform.position)).normalized;
            transform.position += (Vector3)(pushDir * pushStrength);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (waypoints != null)
        {
            Gizmos.color = Color.green;
            foreach (var wp in waypoints)
                Gizmos.DrawSphere(wp.position, 0.2f);
        }
    }
}
