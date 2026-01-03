using UnityEngine;

public class BowlingGameManager2D_TopDown : MonoBehaviour
{
    [Header("Refs")]
    public BowlingBallController2D ball;
    public Transform ballSpawn;
    public BowlingPin[] pins;
    public BowlingScoreManager score;
    public BowlingStrikeJuice strikeJuice; // optional

    [Header("Roll end detection")]
    public float stopSpeed = 0.08f;          // near-stopped threshold
    public float stopHoldTime = 0.25f;       // must stay slow this long
    public float settleDelayBeforeScore = 0.55f; // let pins finish falling

    float stopTimer;
    bool scoring;

    int rollInFrame = 1;
    int standingBeforeRoll = 10;

    void Start()
    {
        StartNewFrame();
        ball.ResetBall(ballSpawn.position);
    }

    void Update()
    {
        if (!ball.hasBeenThrown || scoring) return;

        float speed = GetBallSpeed();

        if (speed < stopSpeed)
        {
            stopTimer += Time.deltaTime;

            if (stopTimer >= stopHoldTime)
            {
                scoring = true;

                // Freeze now so nothing keeps pushing the ball/pins weirdly
                ball.FreezePhysics();

                Invoke(nameof(ScoreRoll), settleDelayBeforeScore);
            }
        }
        else
        {
            stopTimer = 0f;
        }
    }

    float GetBallSpeed()
    {
#if UNITY_6000_0_OR_NEWER
        return ball.rb.linearVelocity.magnitude;
#else
        return ball.rb.velocity.magnitude;
#endif
    }

    void OnEnable()
    {
        stopTimer = 0f;
        scoring = false;
    }

    // Call this from your throw (optional). If you don't want to, we auto-calc below.
    void BeginRollSnapshot()
    {
        standingBeforeRoll = CountStandingPins();
    }

    void ScoreRoll()
    {
        // If we never snapshotted, do it right now using "pins standing before throw"
        // (fallback: assumes full rack on first roll)
        if (rollInFrame == 1 && standingBeforeRoll == 0) standingBeforeRoll = 10;

        int standingAfter = CountStandingPins();
        int knockedThisRoll = Mathf.Clamp(standingBeforeRoll - standingAfter, 0, 10);

        score.RegisterRoll(knockedThisRoll);

        bool strike = (rollInFrame == 1 && standingAfter == 0);
        bool spare = (rollInFrame == 2 && standingAfter == 0);

        if (strike)
        {
            ball.Strike();
            if (strikeJuice) strikeJuice.TriggerStrike();
            AdvanceFrame();
        }
        else if (rollInFrame == 2)
        {
            if (!spare) ball.ResetCombo();
            AdvanceFrame();
        }
        else
        {
            rollInFrame = 2;
            PrepareSecondRoll();
        }

        // Reset ball for next roll
        ball.UnfreezePhysics();
        ball.ResetBall(ballSpawn.position);

        // Reset roll-end tracking
        stopTimer = 0f;
        scoring = false;

        // Snapshot next roll
        standingBeforeRoll = CountStandingPins();
    }

    void StartNewFrame()
    {
        rollInFrame = 1;
        ResetPinsForNewFrame();
        standingBeforeRoll = CountStandingPins();
    }

    void AdvanceFrame()
    {
        StartNewFrame();
    }

    void PrepareSecondRoll()
    {
        // Keep only standing pins, reset their moved drift
        foreach (var p in pins)
        {
            if (p.IsStanding())
            {
                p.ResetPin();
            }
            else
            {
                p.gameObject.SetActive(false);
            }
        }

        standingBeforeRoll = CountStandingPins();
    }

    void ResetPinsForNewFrame()
    {
        foreach (var p in pins)
            p.ResetForNewFrame();
    }

    int CountStandingPins()
    {
        int count = 0;
        foreach (var p in pins)
        {
            if (!p.gameObject.activeInHierarchy) continue;
            if (p.IsStanding()) count++;
        }
        return count;
    }
}
