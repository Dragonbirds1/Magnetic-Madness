using UnityEngine;

public class BowlingGameManager : MonoBehaviour
{
    public BowlingBall ball;
    public BowlingPin[] pins;
    public BowlingScoreManager scoreManager;
    public Transform ballStart;

    int rollInFrame = 1;
    bool rollScored;

    void Start()
    {
        ResetPinsForNewFrame();
    }

    void Update()
    {
        if (!ball.hasBeenThrown)
            return;

        if (!rollScored && ball.rb.linearVelocity.magnitude < 0.05f)
        {
            rollScored = true;
            Invoke(nameof(ScoreRoll), 0.3f);
        }
    }

    void ScoreRoll()
    {
        int knockedThisRoll = CountNewlyKnockedPins();
        scoreManager.RegisterRoll(knockedThisRoll);

        // STRIKE
        if (rollInFrame == 1 && knockedThisRoll == 10)
        {
            AdvanceFrame();
        }
        // END FRAME
        else if (rollInFrame == 2)
        {
            AdvanceFrame();
        }
        // SECOND ROLL
        else
        {
            rollInFrame = 2;
            PrepareForSecondRoll();
        }

        ball.ResetBall(ballStart.position);
        rollScored = false;
    }

    int CountNewlyKnockedPins()
    {
        int count = 0;

        foreach (var pin in pins)
        {
            if (pin.isKnockedOver && !pin.alreadyScored)
            {
                pin.alreadyScored = true;
                count++;
            }
        }

        return count;
    }

    void PrepareForSecondRoll()
    {
        foreach (var pin in pins)
        {
            if (pin.alreadyScored)
            {
                pin.gameObject.SetActive(false);
            }
            else
            {
                pin.ResetPin();
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
            pin.ResetForNewFrame();
    }
}
