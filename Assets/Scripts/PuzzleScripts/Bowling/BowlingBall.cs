using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BowlingBall : MonoBehaviour
{
    [Header("Refs")]
    public Rigidbody2D rb;
    public Transform aimPivot; // optional child that rotates to aim (recommended)

    [Header("Input System (drag these from your InputActions)")]
    public CustomInput aim;    // set valueAction = Gameplay/Aim
    public CustomInput throwIn; // set buttonAction = Gameplay/Throw

    [Header("Aim")]
    public float aimRotateSpeed = 140f;
    public float minAimAngle = -25f;
    public float maxAimAngle = 25f;

    [Header("Power (press once to charge, press again to throw)")]
    public float minPower = 4f;
    public float maxPower = 18f;
    public float chargeSpeed = 10f;
    public float power = 8f;

    [Header("Rolling Feel")]
    public float linearDampingWhileRolling = 0.6f;
    public float angularDampingAlways = 999f;
    public float maxSpeed = 22f;

    [Header("Hook / Curve")]
    public float hookStrength = 8f;
    public float hookResponse = 12f;
    public float hookMinSpeed = 1.5f;

    [Header("State")]
    public bool hasBeenThrown;

    bool charging;
    float targetHook;
    float currentHook;

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

#if UNITY_6000_0_OR_NEWER
        rb.linearDamping = 0f;
        rb.angularDamping = angularDampingAlways;
#else
        rb.drag = 0f;
        rb.angularDrag = angularDampingAlways;
#endif
    }

    void OnEnable()
    {
        aim.Enable();
        throwIn.Enable();
    }

    void OnDisable()
    {
        aim.Disable();
        throwIn.Disable();
    }

    void Update()
    {
        if (!hasBeenThrown)
        {
            AimStep();

            // tap to toggle charge, tap again to throw
            if (throwIn.PressedThisFrame())
            {
                charging = !charging;
                if (!charging) Throw();
            }

            if (charging)
            {
                power += chargeSpeed * Time.deltaTime;
                if (power > maxPower) power = minPower; // arcade cycle
            }
        }
    }

    void FixedUpdate()
    {
        if (!hasBeenThrown) return;

        // rolling damping
#if UNITY_6000_0_OR_NEWER
        rb.linearDamping = linearDampingWhileRolling;
#else
        rb.drag = linearDampingWhileRolling;
#endif

        ClampSpeed();
        HookStep();
    }

    void AimStep()
    {
        int dir = aim.GetDigital();
        if (dir == 0) return;

        Transform pivot = aimPivot ? aimPivot : transform;

        float z = pivot.eulerAngles.z;
        if (z > 180f) z -= 360f;

        z += -dir * aimRotateSpeed * Time.deltaTime;
        z = Mathf.Clamp(z, minAimAngle, maxAimAngle);

        pivot.rotation = Quaternion.Euler(0f, 0f, z);
    }

    void Throw()
    {
        Transform pivot = aimPivot ? aimPivot : transform;
        Vector2 dir = pivot.up.normalized;

        hasBeenThrown = true;

#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = dir * power;
#else
        rb.velocity = dir * power;
#endif

        // once thrown, stop charging
        charging = false;
    }

    void HookStep()
    {
        float speed = GetSpeed();
        if (speed < hookMinSpeed)
            targetHook = 0f;
        else
            targetHook = aim.ReadValue(); // same as aim input (smooth analog)

        currentHook = Mathf.MoveTowards(currentHook, targetHook, hookResponse * Time.fixedDeltaTime);

        Vector2 v = GetVelocity();
        if (v.sqrMagnitude < 0.0001f) return;

        Vector2 forward = v.normalized;
        Vector2 right = new Vector2(forward.y, -forward.x);

        rb.AddForce(right * (currentHook * hookStrength), ForceMode2D.Force);
    }

    void ClampSpeed()
    {
        Vector2 v = GetVelocity();
        float spd = v.magnitude;
        if (spd <= maxSpeed) return;

        v = v.normalized * maxSpeed;
#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = v;
#else
        rb.velocity = v;
#endif
    }

    float GetSpeed() => GetVelocity().magnitude;

    Vector2 GetVelocity()
    {
#if UNITY_6000_0_OR_NEWER
        return rb.linearVelocity;
#else
        return rb.velocity;
#endif
    }

    public void ResetBall(Vector3 pos)
    {
        hasBeenThrown = false;
        charging = false;
        targetHook = 0f;
        currentHook = 0f;

        transform.position = pos;
        if (aimPivot) aimPivot.localRotation = Quaternion.identity;

#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.linearDamping = 0f;
#else
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.drag = 0f;
#endif
    }
}
