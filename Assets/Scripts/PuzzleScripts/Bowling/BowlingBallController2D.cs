using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// REWRITE: Top-down 2D bowling ball controller (Player2 gated) with:
/// - Input System (Aim/Charge/Throw actions)
/// - Aim rotate, hold-to-charge, throw
/// - Perfect release -> slowmo + shake + burst
/// - Combo API: Strike() / ResetCombo()
/// - Camera follow switches only on throw/reset (no fighting)
/// - GUARANTEED STOP system (snaps + locks at end of roll)
///
/// Requires a BowlingInputRefs that exposes:
///   InputAction Aim, Charge, Throw
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class BowlingBallController2D : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public BowlingInputRefs input;         // MUST provide Aim/Charge/Throw InputActions

    [Header("Player 2 Gate (blocks INPUT only)")]
    public Transform player2;
    public float controlDistance = 2.5f;

    [Header("Camera Follow (optional)")]
    public CameraFollowing followCam;      // has: public Transform target;
    public Transform defaultCamTarget;     // Player2
    public bool followBallAfterThrow = true;

    [Header("Aim")]
    public Transform aimPivot;             // rotate this for aim; if null uses ball transform
    public float aimRotateSpeed = 200f;
    public float minAimAngle = -25f;
    public float maxAimAngle = 25f;
    [Range(0f, 0.5f)] public float stickDeadzone = 0.15f;
    public bool forwardIsRight = false;    // if your "forward" is right instead of up

    [Header("Power (hold CHARGE)")]
    public float minPower = 4f;
    public float maxPower = 18f;
    public float chargeSpeed = 10f;
    public bool pingPongCharge = true;

    [Header("Throw")]
    public float throwDriftDegrees = 3.5f;

    [Header("Perfect Release")]
    [Range(0f, 1f)] public float perfectTarget = 0.75f;
    [Range(0f, 0.25f)] public float perfectWindow = 0.06f;
    public float perfectBonusPower = 1.0f;

    [Header("Combo")]
    public int combo = 0;
    public int comboMax = 10;

    [Header("Stop System (GUARANTEED STOP)")]
    public bool lockWhenStopped = true;    // keeps it frozen once stopped

    [Header("Hard Freeze Stop (guaranteed)")]
    public bool hardFreezeOnStop = true;
    public float stopSpeed = 0.08f;
    public float stopHoldTime = 0.20f;

    float stopTimer;
    bool isFrozen;

    [Header("Stop After Pins")]
    public bool stopAfterHittingPins = true;
    public float stopAfterHitDelay = 0.12f;  // lets impact happen first
    public LayerMask pinMask;                // set to your Pins layer
    bool hasHitPins;
    Coroutine stopAfterPinsRoutine;

    [Header("Perfect Slow-mo + Shake")]
    public Camera shakeCamera;
    public float slowmoDuration = 0.22f;
    [Range(0.05f, 1f)] public float slowmoScale = 0.35f;
    public float shakeDuration = 0.18f;
    public float shakeStrength = 0.25f;

    [Header("VFX")]
    public ParticleSystem throwDust;
    public ParticleSystem throwDustBig;
    public ParticleSystem perfectCorePop;
    public ParticleSystem perfectStreaks;
    public TrailRenderer trail;

    [Header("Charge UI Ring (optional)")]
    public Transform powerRing;
    public float ringMinScale = 0.7f;
    public float ringMaxScale = 1.3f;
    public SpriteRenderer powerRingRenderer;
    public Color ringLowColor = Color.white;
    public Color ringHighColor = Color.red;

    [Header("Read-only")]
    public bool hasBeenThrown;

    [SerializeField] bool isCharging;
    [SerializeField, Range(0f, 1f)] float charge01;
    [SerializeField] float currentPower;
    public bool IsCharging => isCharging;
    public float Charge01 => charge01;
    public float CurrentPower => currentPower;

    // UI readouts
    public bool InRange => player2 && Vector2.Distance(player2.position, transform.position) <= controlDistance;
    public bool InPerfectWindow { get; private set; }
    public bool PerfectJustTriggeredThisFrame { get; private set; }
    public float PerfectTarget01 => perfectTarget;
    public float PerfectWindow01 => perfectWindow;

    // stop internals
    bool isStopped;
    public bool IsStopped => isStopped;

    [Header("BowlingBallStopSystem")]
    public GameObject stopSystemObject;
    public GameObject stopSystemObject2;
    public GameObject stopSystemObject3;
    public GameObject stopSystemObject4;

    // power internals
    float power;
    float chargeDir = 1f;

    // coroutines
    Coroutine slowmoRoutine;
    Coroutine shakeRoutine;

    void Awake()
    {
        stopSystemObject.SetActive(false);
        stopSystemObject2.SetActive(false);
        stopSystemObject3.SetActive(false);
        stopSystemObject4.SetActive(false);

        if (!rb) rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

#if UNITY_6000_0_OR_NEWER
        rb.linearDamping = 0f;
        rb.angularDamping = 999f;
#else
        rb.drag = 0f;
        rb.angularDrag = 999f;
#endif
        if (!shakeCamera) shakeCamera = Camera.main;

        power = Mathf.Lerp(minPower, maxPower, 0.5f);
        currentPower = power;
        charge01 = Mathf.InverseLerp(minPower, maxPower, currentPower);

        if (trail) trail.emitting = false;

        if (followCam && defaultCamTarget)
            followCam.target = defaultCamTarget;
    }

    void OnEnable()
    {
        input?.Aim?.Enable();
        input?.Charge?.Enable();
        input?.Throw?.Enable();
    }

    void OnDisable()
    {
        input?.Aim?.Disable();
        input?.Charge?.Disable();
        input?.Throw?.Disable();
    }

    void Update()
    {
        // reset one-frame flag for UI
        PerfectJustTriggeredThisFrame = false;

        bool canInput = InRange;

        // If ball is rolling, we do NOT block update logic (stop system runs in FixedUpdate)
        if (!hasBeenThrown && canInput)
        {
            // Aim
            Vector2 aim = ReadAim();
            AimStep(aim.x);

            // Charge
            isCharging = ChargeHeld();
            if (isCharging) ChargePower();

            currentPower = power;
            charge01 = Mathf.InverseLerp(minPower, maxPower, currentPower);

            InPerfectWindow = Mathf.Abs(charge01 - perfectTarget) <= perfectWindow;

            UpdatePowerRing(true);

            // Throw
            if (ThrowPressedThisFrame())
                ThrowNow(power);
        }
        else
        {
            isCharging = false;
            InPerfectWindow = false;
            UpdatePowerRing(false);
        }
    }

    void FixedUpdate()
    {
        if (!hasBeenThrown) return;

        if (hardFreezeOnStop && isFrozen)
            return; // already frozen

        float speed =
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity.magnitude;
#else
        rb.velocity.magnitude;
#endif

        if (speed < stopSpeed)
        {
            stopTimer += Time.fixedDeltaTime;

            if (stopTimer >= stopHoldTime)
            {
                if (hardFreezeOnStop)
                    FreezePhysics();
                else
                    StopBallImmediate(); // fallback

                return;
            }
        }
        else
        {
            stopTimer = 0f;
        }

        // IMPORTANT: if you have hook/oil forces, they run only while NOT frozen.
    }

    // ==========================
    // Public API for manager
    // ==========================
    public void Strike() => combo = Mathf.Clamp(combo + 1, 0, comboMax);
    public void ResetCombo() => combo = 0;

    public void StopBallImmediate()
    {
#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
#else
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
#endif
        rb.Sleep();
    }

    public void ResetBall(Vector3 pos)
    {
        // cancel perfect juice
        if (slowmoRoutine != null) StopCoroutine(slowmoRoutine);
        if (shakeRoutine != null) StopCoroutine(shakeRoutine);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        hasHitPins = false;
        if (stopAfterPinsRoutine != null) StopCoroutine(stopAfterPinsRoutine);
        UnfreezePhysics();

        UnfreezePhysics();
        stopTimer = 0f;

        hasBeenThrown = false;
        isCharging = false;
        InPerfectWindow = false;
        PerfectJustTriggeredThisFrame = false;

        stopSystemObject.SetActive(false);
        stopSystemObject2.SetActive(false);
        stopSystemObject3.SetActive(false);
        stopSystemObject4.SetActive(false);

        // stop system reset
        isStopped = false;
        stopTimer = 0f;

        // motion reset
        StopBallImmediate();

#if UNITY_6000_0_OR_NEWER
        rb.linearDamping = 0f;
        rb.angularDamping = 999f;
#else
        rb.drag = 0f;
        rb.angularDrag = 999f;
#endif

        rb.position = pos;
        transform.position = pos;

        if (aimPivot) aimPivot.localRotation = Quaternion.identity;

        if (trail) trail.emitting = false;

        // reset power
        power = Mathf.Lerp(minPower, maxPower, 0.5f);
        currentPower = power;
        charge01 = Mathf.InverseLerp(minPower, maxPower, currentPower);

        UpdatePowerRing(false);

        // camera back to player
        if (followCam && defaultCamTarget)
            followCam.target = defaultCamTarget;
    }



    // ==========================
    // Throw + Perfect
    // ==========================
    void ThrowNow(float finalPower)
    {
        Transform pivot = aimPivot ? aimPivot : transform;

        float norm = Mathf.InverseLerp(minPower, maxPower, finalPower);
        bool perfect = Mathf.Abs(norm - perfectTarget) <= perfectWindow;

        if (perfect)
        {
            finalPower += perfectBonusPower;
            PerfectJustTriggeredThisFrame = true;
        }

        hasHitPins = false;
        if (stopAfterPinsRoutine != null) StopCoroutine(stopAfterPinsRoutine);

        // reset stop system when we throw
        isStopped = false;
        stopTimer = 0f;

        float driftT = Mathf.Clamp01(norm);
        float driftDeg = Random.Range(-throwDriftDegrees, throwDriftDegrees) * driftT;

        Vector2 forward = GetForward2D(pivot);
        Vector2 dir = (Quaternion.Euler(0, 0, driftDeg) * forward).normalized;

        hasBeenThrown = true;
        isCharging = false;
        UpdatePowerRing(false);

        UnfreezePhysics();
        stopTimer = 0f;

        // camera follows ball ONLY here (no per-frame fighting)
        if (followBallAfterThrow && followCam)
            followCam.target = transform;

#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = dir * finalPower;
#else
        rb.velocity = dir * finalPower;
#endif

        if (throwDust) throwDust.Play();
        if (throwDustBig) throwDustBig.Play();
        if (trail) trail.emitting = true;

        if (perfect) TriggerPerfectRelease();
    }

    void TriggerPerfectRelease()
    {
        if (perfectCorePop) perfectCorePop.Play();
        if (perfectStreaks) perfectStreaks.Play();

        if (slowmoRoutine != null) StopCoroutine(slowmoRoutine);
        slowmoRoutine = StartCoroutine(SlowMoRoutine());

        if (shakeRoutine != null) StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(ShakeRoutine());
    }

    IEnumerator SlowMoRoutine()
    {
        float old = Time.timeScale;

        Time.timeScale = slowmoScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        float t = 0f;
        while (t < slowmoDuration)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = old;
        Time.fixedDeltaTime = 0.02f;
    }

    IEnumerator ShakeRoutine()
    {
        if (!shakeCamera) yield break;

        Transform camT = shakeCamera.transform;
        Vector3 start = camT.localPosition;

        float t = 0f;
        while (t < shakeDuration)
        {
            t += Time.unscaledDeltaTime;
            Vector2 r = Random.insideUnitCircle * shakeStrength;
            camT.localPosition = start + new Vector3(r.x, r.y, 0f);
            yield return null;
        }

        camT.localPosition = start;
    }

    // ==========================
    // Charge visuals
    // ==========================
    void UpdatePowerRing(bool enable)
    {
        if (!powerRing) return;

        powerRing.gameObject.SetActive(enable);
        if (!enable) return;

        float t = Mathf.Clamp01(charge01);
        float baseS = Mathf.Lerp(ringMinScale, ringMaxScale, t);
        float pulse = isCharging ? (1f + Mathf.Sin(Time.time * 18f) * 0.06f) : 1f;

        // extra pop when you're inside perfect
        float perfectPop = InPerfectWindow ? (1f + Mathf.Sin(Time.time * 26f) * 0.08f) : 1f;

        powerRing.localScale = Vector3.one * baseS * pulse * perfectPop;

        if (powerRingRenderer)
            powerRingRenderer.color = Color.Lerp(ringLowColor, ringHighColor, t);
    }

    // ==========================
    // Input helpers
    // ==========================
    Vector2 ReadAim()
    {
        var a = input?.Aim;
        if (a == null) return Vector2.zero;

        Vector2 v = a.ReadValue<Vector2>();
        if (v.magnitude < stickDeadzone) return Vector2.zero;
        return Vector2.ClampMagnitude(v, 1f);
    }

    bool ChargeHeld()
    {
        var a = input?.Charge;
        if (a == null) return false;

        if (a.type == InputActionType.Button) return a.IsPressed();
        return a.ReadValue<float>() > 0.2f;
    }

    bool ThrowPressedThisFrame()
    {
        var a = input?.Throw;
        if (a == null) return false;
        return a.WasPressedThisFrame();
    }

    // ======== HARD STOP API (manager uses these) ========
    public bool IsFrozen { get; private set; }

    public void FreezePhysics()
    {
        if (IsFrozen) return;

#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = Vector2.zero;
#else
    rb.velocity = Vector2.zero;
#endif
        rb.angularVelocity = 0f;

        rb.simulated = false;   // <-- nuclear stop
        IsFrozen = true;
    }

    public void UnfreezePhysics()
    {
        if (!IsFrozen) return;

        rb.simulated = true;
        IsFrozen = false;

#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = Vector2.zero;
#else
    rb.velocity = Vector2.zero;
#endif
        rb.angularVelocity = 0f;
    }

    void AimStep(float x)
    {
        if (Mathf.Abs(x) < 0.001f) return;

        Transform pivot = aimPivot ? aimPivot : transform;

        float z = pivot.eulerAngles.z;
        if (z > 180f) z -= 360f;

        z += -x * aimRotateSpeed * Time.deltaTime;
        z = Mathf.Clamp(z, minAimAngle, maxAimAngle);

        pivot.rotation = Quaternion.Euler(0f, 0f, z);
    }

    void ChargePower()
    {
        if (!pingPongCharge)
        {
            power = Mathf.Clamp(power + chargeSpeed * Time.deltaTime, minPower, maxPower);
            return;
        }

        power += chargeDir * chargeSpeed * Time.deltaTime;

        if (power >= maxPower) { power = maxPower; chargeDir = -1f; }
        else if (power <= minPower) { power = minPower; chargeDir = 1f; }
    }

    // ==========================
    // Utils
    // ==========================
    float GetSpeed()
    {
#if UNITY_6000_0_OR_NEWER
        return rb.linearVelocity.magnitude;
#else
        return rb.velocity.magnitude;
#endif
    }

    IEnumerator StopSoonAfterPins()
    {
        yield return new WaitForSeconds(stopAfterHitDelay);

        // NUCLEAR STOP
        UnfreezePhysics();   // ensure it’s simulated before toggling (safe)
        FreezePhysics();     // sets rb.simulated=false, so NOTHING can move it
    }

    Vector2 GetForward2D(Transform pivot)
    {
        Vector3 d = forwardIsRight ? pivot.right : pivot.up;
        d.z = 0f;
        if (d.sqrMagnitude < 0.0001f) d = Vector3.up;
        return ((Vector2)d).normalized;
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Pin"))
        {
            stopSystemObject.SetActive(true);
            stopSystemObject2.SetActive(true);
            stopSystemObject3.SetActive(true);
            stopSystemObject4.SetActive(true);
        }

        if (!stopAfterHittingPins) return;
        if (!hasBeenThrown) return;
        if (hasHitPins) return;

        // Layer-based check (recommended)
        if (((1 << col.collider.gameObject.layer) & pinMask) == 0)
            return;

        hasHitPins = true;

        if (stopAfterPinsRoutine != null) StopCoroutine(stopAfterPinsRoutine);
        stopAfterPinsRoutine = StartCoroutine(StopSoonAfterPins());
    }
}
