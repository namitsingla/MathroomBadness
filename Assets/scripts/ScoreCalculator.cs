using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    public collectedisplay collectedisplay;

    public void CalculateScore()
    {
        collectedisplay.score += (int) (collectedisplay.collected *10000 *collectedisplay.mult/collectedisplay.timer);

        collectedisplay.collected = 0;
        collectedisplay.chalk = 0;
        collectedisplay.homework = 0;

        collectedisplay.timer = 0f;
    }
}
