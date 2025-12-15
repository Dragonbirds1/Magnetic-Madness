using UnityEngine;
using System.Collections.Generic;

public class BowlingScoreManager : MonoBehaviour
{
    public List<int> rolls = new List<int>();

    public void RegisterRoll(int pins)
    {
        rolls.Add(pins);
        Debug.Log($"Roll {rolls.Count}: {pins} pins | Total Score: {CalculateScore()}");
    }

    int CalculateScore()
    {
        int score = 0;
        int rollIndex = 0;

        for (int frame = 0; frame < 10; frame++)
        {
            if (rollIndex >= rolls.Count)
                break;

            // Strike
            if (rolls[rollIndex] == 10)
            {
                if (rollIndex + 2 < rolls.Count)
                    score += 10 + rolls[rollIndex + 1] + rolls[rollIndex + 2];

                rollIndex++;
            }
            // Spare
            else if (rollIndex + 1 < rolls.Count &&
                     rolls[rollIndex] + rolls[rollIndex + 1] == 10)
            {
                if (rollIndex + 2 < rolls.Count)
                    score += 10 + rolls[rollIndex + 2];

                rollIndex += 2;
            }
            // Open frame
            else
            {
                if (rollIndex + 1 < rolls.Count)
                    score += rolls[rollIndex] + rolls[rollIndex + 1];

                rollIndex += 2;
            }
        }

        return score;
    }

    public void ResetGame()
    {
        rolls.Clear();
    }
}
