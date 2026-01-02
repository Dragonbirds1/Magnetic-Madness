using UnityEngine;

public class BowlingGameManager : MonoBehaviour
{
    [Header("References")]
    public BowlingBall ball;                 // your ball script
    public BowlingPin[] pins;
    public BowlingScoreManager scoreManager;
    public Transform ballStart;

    [Header("Stop Detection")]
    public float stopSpeed = 0.08f;          // speed threshold considered “stopped”
    public float settleTime = 0.35f;         // must stay slow this long

    private int rollInFrame = 1;             // 1 or 2
    private float settleTimer = 0f;
    private bool scoredThisRoll = false;

    void Start()
    {
        ResetPinsForNewFrame();
    }

    void Update()
    {
        if (!ball || !ball.hasBeenThrown) return;

        float speed = GetBallSpeed();

        if (speed <= stopSpeed)
            settleTimer += Time.deltaTime;
        else
            settleTimer = 0f;

        if (!scoredThisRoll && settleTimer >= settleTime)
        {
            scoredThisRoll = true;
            ScoreRoll();
        }
    }

    float GetBallSpeed()
    {
        if (!ball.rb) return 0f;

#if UNITY_6000_0_OR_NEWER
        return ball.rb.linearVelocity.magnitude;
#else
        return ball.rb.velocity.magnitude;
#endif
    }

    void ScoreRoll()
    {
        int knockedThisRoll = CountNewlyKnockedPins();
        scoreManager.RegisterRoll(knockedThisRoll);

        // STRIKE on first roll
        if (rollInFrame == 1 && knockedThisRoll == 10)
        {
            AdvanceFrame();
        }
        else if (rollInFrame == 1)
        {
            // Go to second roll
            rollInFrame = 2;
            PrepSecondRoll();
        }
        else
        {
            // End of frame after second roll
            AdvanceFrame();
        }

        // Reset ball for next roll
        ball.ResetBall(ballStart.position);

        // reset roll scoring state
        scoredThisRoll = false;
        settleTimer = 0f;
    }

    int CountNewlyKnockedPins()
    {
        int count = 0;

        foreach (var pin in pins)
        {
            if (!pin) continue;

            if (pin.isKnockedOver && !pin.alreadyScored)
            {
                pin.alreadyScored = true;
                count++;
            }
        }

        return count;
    }

    void PrepSecondRoll()
    {
        foreach (var pin in pins)
        {
            if (!pin) continue;

            // Remove knocked pins so they can't be hit again
            if (pin.alreadyScored)
            {
                pin.gameObject.SetActive(false);
            }
            else
            {
                // Standing pins: reset to exact starting pose (so “bumped” pins snap back)
                pin.ResetPin(recacheStart: false);
            }
        }
    }

    void AdvanceFrame()
    {
        rollInFrame = 1;
        ResetPinsForNewFrame();
    }

    void ResetPinsForNewFrame()
    {
        foreach (var pin in pins)
        {
            if (!pin) continue;

            pin.gameObject.SetActive(true);
            pin.alreadyScored = false;
            pin.ResetPin(recacheStart: false);
        }
    }
}

