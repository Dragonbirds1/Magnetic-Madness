using UnityEngine;
using UnityEngine.InputSystem;

public class BowlingBall : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public Transform arrow;
    public GameObject player;

    [Header("Stats")]
    public float weight = 6f;
    public float hookStrength = 2f;
    public float radToStart;

    [Header("Shot")]
    public float powerStep = 0.5f;
    public float maxPower = 20f;

    [Header("Aim")]
    public float aimRotateSpeed = 120f;

    [Header("Ball Movement")]
    public float moveSpeed = 3f;
    public float laneMinX = -2f;
    public float laneMaxX = 2f;

    [Header("Input Actions")]
    public InputAction moveAction;        // ← NEW (Left / Right)
    public InputAction aimAction;         // Optional: X axis for aiming
    public InputAction powerUpAction;
    public InputAction powerDownAction;
    public InputAction throwAction;
    public InputAction togglePlayAction;

    [HideInInspector] public bool isActive;
    [HideInInspector] public bool hasBeenThrown;

    private float aimAngle = 90f;
    private float power = 0f;

    void OnEnable()
    {
        moveAction.Enable();
        aimAction.Enable();
        powerUpAction.Enable();
        powerDownAction.Enable();
        throwAction.Enable();
        togglePlayAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        aimAction.Disable();
        powerUpAction.Disable();
        powerDownAction.Disable();
        throwAction.Disable();
        togglePlayAction.Disable();
    }

    void Awake()
    {
        rb.mass = weight;
        arrow.gameObject.SetActive(false);
    }

    void Update()
    {
        float playerPos = Vector2.Distance(transform.position, player.transform.position);
        if (playerPos <= radToStart)
        {
            if (!hasBeenThrown && togglePlayAction.WasPressedThisFrame())
            {
                isActive = !isActive;
                power = 0f;
                arrow.gameObject.SetActive(false);
            }

            if (!isActive || hasBeenThrown) return;

            HandleMovement();
            HandleAim();
            HandlePower();
            UpdateArrow();

            if (throwAction.WasPressedThisFrame())
                Throw();
        }
        else if (playerPos > radToStart && isActive)
        {
            isActive = false;
            power = 0f;
            arrow.gameObject.SetActive(false);
            return;
        }
    }

    void FixedUpdate()
    {
        if (!hasBeenThrown) return;

        if (rb.linearVelocity.magnitude > 0.1f)
            rb.AddForce(Vector2.right * hookStrength * Time.fixedDeltaTime);
    }

    // ===== LEFT / RIGHT MOVEMENT =====
    void HandleMovement()
    {
        float move = moveAction.ReadValue<float>(); // -1 to +1

        if (Mathf.Abs(move) < 0.1f) return;

        Vector3 pos = transform.position;
        pos.x += move * moveSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, laneMinX, laneMaxX);
        transform.position = pos;
    }

    // ===== AIM =====
    void HandleAim()
    {
        float input = aimAction.ReadValue<float>();

        aimAngle += input * aimRotateSpeed * Time.deltaTime;
        aimAngle = Mathf.Clamp(aimAngle, 30f, 150f);
    }

    void HandlePower()
    {
        if (powerUpAction.IsPressed()) power += powerStep;
        if (powerDownAction.IsPressed()) power -= powerStep;

        power = Mathf.Clamp(power, 0f, maxPower);
    }

    void UpdateArrow()
    {
        if (power <= 0f)
        {
            arrow.gameObject.SetActive(false);
            return;
        }

        arrow.gameObject.SetActive(true);
        arrow.position = transform.position;

        Vector2 dir = AngleToVector(aimAngle);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

        arrow.rotation = Quaternion.Euler(0, 0, angle);
        arrow.localScale = new Vector3(1, power * 0.1f, 1);
    }

    void Throw()
    {
        if (power <= 0f) return;

        Vector2 dir = AngleToVector(aimAngle);
        rb.AddForce(dir * power, ForceMode2D.Impulse);

        hasBeenThrown = true;
        isActive = false;
        arrow.gameObject.SetActive(false);
    }

    Vector2 AngleToVector(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    public void ResetBall(Vector3 startPos)
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0;
        transform.position = startPos;

        hasBeenThrown = false;
        isActive = false;
        power = 0f;
        aimAngle = 90f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radToStart);
    }
}

