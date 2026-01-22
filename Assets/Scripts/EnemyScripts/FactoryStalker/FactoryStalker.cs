using UnityEngine;

public class FactoryStalker : MonoBehaviour
{
    private enum State { Patrol, Stalk, Alert, Attack, Retreat }
    [SerializeField] private State currentState = State.Patrol;

    [Header("FactoryStalker Reference")]
    [Tooltip("If left empty, this script uses the GameObject it's on.")]
    public Transform factoryStalker;

    [Header("Player Reference")]
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 3.0f;
    public float nodeReachDistance = 0.35f;

    [Header("Detection / Behavior")]
    public float detectionRange = 10.0f;
    public float minStalkingDistance = 5.0f;
    public float maxStalkingDistance = 15.0f;
    public float retreatDistance = 3.0f;

    [Range(0, 100)] public int attackChance = 50;
    [Range(0, 100)] public int alertChance = 50;

    public float decisionDelay = 2.0f;
    public float alertDuration = 5.0f;

    [Header("Patrol Nodes")]
    public Transform[] patrolNodes;

    [Header("Obstacle Avoidance (2D)")]
    public LayerMask obstacleMask;
    public float avoidCheckDistance = 0.6f;
    public float avoidRadius = 0.25f;
    public float avoidStrength = 1.25f;

    [Header("Collision Movement")]
    public float skinWidth = 0.02f;
    public int maxSlideIters = 2;

    [Header("Animation (4-way)")]
    public Animator animator;
    public string moveXParam = "MoveX";
    public string moveYParam = "MoveY";
    public string movingParam = "Moving";

    [Tooltip("How small movement can be before we consider it stopped (idle).")]
    public float animStopSpeed = 0.001f;

    [Tooltip("Playback speed when moving (snappy movement).")]
    public float animMoveSpeedMultiplier = 2.0f;

    private Vector2 facing = Vector2.down;     // last facing (cardinal)
    private Vector2 animIntent = Vector2.down; // intended direction this frame

    // Optional debug toggles
    [Header("Debug Toggles (Optional)")]
    public bool startStalking;
    public bool stopStalking;
    public bool patrolFactory = true;

    // Internal
    private Vector3 currentTarget;
    private bool hasTarget;
    private float decisionTimer;
    private float alertTimer;

    private Rigidbody2D rb;
    private Collider2D col;

    void Awake()
    {
        if (factoryStalker == null) factoryStalker = transform;

        if (animator == null)
            animator = factoryStalker.GetComponentInChildren<Animator>();

        // Kinematic Rigidbody2D for collision-safe MovePosition
        rb = factoryStalker.GetComponent<Rigidbody2D>();
        if (rb == null) rb = factoryStalker.gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = true;
        rb.freezeRotation = true;

        // Collider for casting
        col = factoryStalker.GetComponent<Collider2D>();
        if (col == null)
            Debug.LogError("FactoryStalker needs a Collider2D on the stalker object.");
    }

    void Start()
    {
        EnterPatrol();
    }

    void Update()
    {
        if (factoryStalker == null) return;

        // Debug overrides
        if (startStalking) { startStalking = false; EnterStalk(); }
        if (stopStalking) { stopStalking = false; EnterRetreat(); }
        if (!patrolFactory && currentState == State.Patrol) EnterStalk();

        switch (currentState)
        {
            case State.Patrol: TickPatrol(); break;
            case State.Stalk: TickStalk(); break;
            case State.Alert: TickAlert(); break;
            case State.Attack: TickAttack(); break;
            case State.Retreat: TickRetreat(); break;
        }
    }

    // ---------------- STATES ----------------

    private void TickPatrol()
    {
        if (patrolNodes == null || patrolNodes.Length == 0)
        {
            UpdateAnimator(false);
            return;
        }

        if (PlayerInRange(detectionRange))
        {
            DecideOnSightAction();
            return;
        }

        if (!hasTarget) PickNewNode();
        MoveTowards2D(currentTarget);

        if (Vector2.Distance(factoryStalker.position, currentTarget) <= nodeReachDistance)
        {
            hasTarget = false;
            decisionTimer = 0f;
        }
    }

    private void TickStalk()
    {
        if (player == null)
        {
            EnterPatrol();
            return;
        }

        float dist = Vector2.Distance(factoryStalker.position, player.position);

        if (dist > maxStalkingDistance)
        {
            EnterPatrol();
            return;
        }

        if (dist < retreatDistance)
        {
            EnterRetreat();
            return;
        }

        Vector2 toPlayer = (player.position - factoryStalker.position);
        Vector2 dirToPlayer = toPlayer.sqrMagnitude > 0.001f ? toPlayer.normalized : Vector2.right;

        Vector2 desiredPoint;
        if (dist < minStalkingDistance)
        {
            desiredPoint = (Vector2)player.position - dirToPlayer * minStalkingDistance;
        }
        else
        {
            Vector2 sideways = new Vector2(-dirToPlayer.y, dirToPlayer.x);
            desiredPoint = (Vector2)player.position - dirToPlayer * minStalkingDistance + sideways * 1.25f;
        }

        currentTarget = desiredPoint;
        hasTarget = true;
        MoveTowards2D(currentTarget);

        decisionTimer += Time.deltaTime;
        if (decisionTimer >= decisionDelay && dist <= detectionRange)
        {
            decisionTimer = 0f;
            DecideOnSightAction();
        }
    }

    private void TickAlert()
    {
        alertTimer += Time.deltaTime;

        if (player != null && PlayerInRange(maxStalkingDistance))
            MoveTowards2D(player.position);
        else
            UpdateAnimator(false);

        if (alertTimer >= alertDuration)
            EnterStalk();
    }

    private void TickAttack()
    {
        if (player == null)
        {
            EnterPatrol();
            return;
        }

        MoveTowards2D(player.position);

        float dist = Vector2.Distance(factoryStalker.position, player.position);
        if (dist < 1.2f)
            EnterRetreat();
    }

    private void TickRetreat()
    {
        if (player == null)
        {
            EnterPatrol();
            return;
        }

        Vector2 away = (factoryStalker.position - player.position);
        Vector2 awayDir = away.sqrMagnitude > 0.001f ? away.normalized : Vector2.right;

        Vector2 retreatPoint = (Vector2)factoryStalker.position + awayDir * 2.0f;
        currentTarget = retreatPoint;
        hasTarget = true;

        MoveTowards2D(currentTarget);

        float dist = Vector2.Distance(factoryStalker.position, player.position);
        if (dist >= minStalkingDistance)
            EnterStalk();
    }

    // ---------------- ENTER ----------------

    public void EnterPatrol()
    {
        currentState = State.Patrol;
        hasTarget = false;
        decisionTimer = 0f;
    }

    public void EnterStalk()
    {
        currentState = State.Stalk;
        hasTarget = false;
        decisionTimer = 0f;
    }

    public void EnterAlert()
    {
        currentState = State.Alert;
        alertTimer = 0f;
        decisionTimer = 0f;
    }

    public void EnterAttack()
    {
        currentState = State.Attack;
        decisionTimer = 0f;
    }

    public void EnterRetreat()
    {
        currentState = State.Retreat;
        decisionTimer = 0f;
    }

    // ---------------- HELPERS ----------------

    private void PickNewNode()
    {
        if (patrolNodes == null || patrolNodes.Length == 0) return;
        currentTarget = patrolNodes[Random.Range(0, patrolNodes.Length)].position;
        hasTarget = true;
    }

    private bool PlayerInRange(float range)
    {
        if (player == null) return false;
        return Vector2.Distance(factoryStalker.position, player.position) <= range;
    }

    private void DecideOnSightAction()
    {
        if (player == null) return;

        int roll = Random.Range(0, 100);

        if (roll < attackChance) EnterAttack();
        else if (roll < attackChance + alertChance) EnterAlert();
        else EnterStalk();
    }

    // ---------------- ANIMATION (INSTANT + FAST) ----------------

    private void SetIntentFromTarget(Vector2 from, Vector2 to)
    {
        Vector2 d = to - from;
        if (d.sqrMagnitude < 0.0001f) return;
        animIntent = d.normalized;
    }

    private Vector2 SnapCardinalInstant(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            return new Vector2(Mathf.Sign(dir.x), 0f); // Left / Right
        else
            return new Vector2(0f, Mathf.Sign(dir.y)); // Up / Down
    }

    private void UpdateAnimator(bool moving)
    {
        if (animator == null) return;

        animator.SetBool(movingParam, moving);

        if (moving)
        {
            facing = SnapCardinalInstant(animIntent);
            animator.speed = animMoveSpeedMultiplier; // 🔥 faster walk/run
        }
        else
        {
            animator.speed = 1f; // normal idle speed
        }

        animator.SetFloat(moveXParam, facing.x);
        animator.SetFloat(moveYParam, facing.y);
    }

    // ---------------- COLLISION-SAFE MOVEMENT + ANIM ----------------

    private void MoveTowards2D(Vector3 target)
    {
        if (col == null || rb == null) return;

        Vector2 pos = rb.position;

        // Accurate facing direction (intent)
        SetIntentFromTarget(pos, (Vector2)target);

        Vector2 toTarget = (Vector2)target - pos;
        if (toTarget.sqrMagnitude < 0.0001f)
        {
            UpdateAnimator(false);
            return;
        }

        Vector2 desired = toTarget.normalized;
        float moveDist = moveSpeed * Time.deltaTime;

        // Steering
        RaycastHit2D ahead = Physics2D.CircleCast(pos, avoidRadius, desired, avoidCheckDistance, obstacleMask);
        if (ahead.collider != null)
        {
            Vector2 perp = new Vector2(-ahead.normal.y, ahead.normal.x);
            desired = (desired + perp * avoidStrength).normalized;
        }

        Vector2 remaining = desired * moveDist;

        ContactFilter2D filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.layerMask = obstacleMask;
        filter.useTriggers = false;

        RaycastHit2D[] hits = new RaycastHit2D[8];

        bool movedThisCall = false;
        float stopSqr = animStopSpeed * animStopSpeed;

        for (int i = 0; i < maxSlideIters; i++)
        {
            float dist = remaining.magnitude;
            if (dist <= 0.0001f) break;

            int hitCount = col.Cast(remaining.normalized, filter, hits, dist + skinWidth);

            // No collision
            if (hitCount == 0)
            {
                Vector2 newPos = pos + remaining;
                rb.MovePosition(newPos);

                movedThisCall = (newPos - pos).sqrMagnitude > stopSqr;
                UpdateAnimator(movedThisCall);
                return;
            }

            // Collision: move partially
            float allowed = Mathf.Max(0f, hits[0].distance - skinWidth);
            Vector2 step = remaining.normalized * allowed;

            Vector2 oldPos = pos;
            pos += step;
            rb.MovePosition(pos);

            movedThisCall = movedThisCall || (pos - oldPos).sqrMagnitude > stopSqr;
            UpdateAnimator(movedThisCall);

            // Slide
            Vector2 n = hits[0].normal;
            remaining = Vector2.Perpendicular(n);
            if (Vector2.Dot(remaining, desired) < 0) remaining = -remaining;
            remaining *= Mathf.Max(0f, dist - allowed);
        }

        UpdateAnimator(false);
    }

    // ---------------- GIZMOS ----------------

    void OnDrawGizmosSelected()
    {
        Transform t = factoryStalker != null ? factoryStalker : transform;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(t.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(t.position, minStalkingDistance);
        Gizmos.DrawWireSphere(t.position, maxStalkingDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(t.position, retreatDistance);
    }
}
