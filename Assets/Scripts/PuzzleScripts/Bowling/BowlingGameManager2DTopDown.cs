using UnityEngine;

public class BowlingGameManager2DTopDown : MonoBehaviour
{
    public BowlingBallController2D ball;
    public Transform ballSpawn;
    public BowlingPin[] pins;
    public BowlingScoreManager score;
    public BowlingStrikeJuice strikeJuice;

    [Header("Ball Stop Detection")]
    public float stopSpeed = 0.08f;   // how slow = stopped
    public float stopDelay = 0.6f;   // wait before scoring


    int rollInFrame = 1;
    bool rollScored;

    void Start()
    {
        ResetPinsForNewFrame();
        ball.ResetBall(ballSpawn.position);
    }

    void Update()
    {
        if (!ball.hasBeenThrown) return;

        float speed;

#if UNITY_6000_0_OR_NEWER
        speed = ball.rb.linearVelocity.magnitude;
#else
    speed = ball.rb.velocity.magnitude;
#endif

        if (!rollScored && speed < stopSpeed)
        {
            rollScored = true;
            ball.StopBallImmediate();          // HARD STOP
            Invoke(nameof(ScoreRoll), stopDelay);
        }
    }

    void ScoreRoll()
    {
        int knockedThisRoll = CountNewlyKnockedPins();
        score.RegisterRoll(knockedThisRoll);

        bool strike = (rollInFrame == 1 && knockedThisRoll == 10);

        if (strike)
        {
            ball.Strike();
            if (strikeJuice) strikeJuice.TriggerStrike();
            AdvanceFrame();
        }

        else if (rollInFrame == 2)
        {
            ball.ResetCombo();       // optional: reset combo on non-strike frame end
            AdvanceFrame();
        }
        else
        {
            rollInFrame = 2;
            PrepareForSecondRoll();
        }

        // ✅ This is what makes your reset work
        ball.ResetBall(ballSpawn.position);
        rollScored = false;
    }

    int CountNewlyKnockedPins()
    {
        int count = 0;
        foreach (var p in pins)
        {
            if (p.isKnockedOver && !p.alreadyScored)
            {
                p.alreadyScored = true;
                count++;
            }
        }
        return count;
    }

    void PrepareForSecondRoll()
    {
        foreach (var p in pins)
        {
            if (p.alreadyScored) p.gameObject.SetActive(false);
            else p.ResetPin();
        }
    }

    void AdvanceFrame()
    {
        rollInFrame = 1;
        ResetPinsForNewFrame();
    }

    void ResetPinsForNewFrame()
    {
        foreach (var p in pins)
            p.ResetForNewFrame();
    }
}
