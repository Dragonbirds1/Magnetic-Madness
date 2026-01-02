using UnityEngine;
using System.Collections.Generic;

public class BowlingScoreManager : MonoBehaviour
{
    public readonly List<int> rolls = new List<int>();

    public void RegisterRoll(int pins)
    {
        pins = Mathf.Clamp(pins, 0, 10);
        rolls.Add(pins);

        Debug.Log($"Roll {rolls.Count}: {pins} | Total: {CalculateScore()}");
    }

    public int CalculateScore()
    {
        int score = 0;
        int i = 0;

        for (int frame = 0; frame < 10; frame++)
        {
            if (i >= rolls.Count) break;

            // Strike
            if (rolls[i] == 10)
            {
                if (i + 2 < rolls.Count) score += 10 + rolls[i + 1] + rolls[i + 2];
                i += 1;
            }
            // Spare
            else if (i + 1 < rolls.Count && rolls[i] + rolls[i + 1] == 10)
            {
                if (i + 2 < rolls.Count) score += 10 + rolls[i + 2];
                i += 2;
            }
            // Open
            else
            {
                if (i + 1 < rolls.Count) score += rolls[i] + rolls[i + 1];
                i += 2;
            }
        }

        return score;
    }

    public void ResetGame()
    {
        rolls.Clear();
    }
}
