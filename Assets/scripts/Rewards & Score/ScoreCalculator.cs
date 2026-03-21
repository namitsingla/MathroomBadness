using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    public collectedisplay collectedisplay;
    public ScoreCounter scoreCounter;
    public RarityManager rarityManager;

    public void CalculateScore()
    {
        int scorewithoutMult = (int) (collectedisplay.collected *10000/collectedisplay.timer);
        scoreCounter.AnimateScore(scorewithoutMult, collectedisplay.mult);

        collectedisplay.score += Mathf.RoundToInt(scorewithoutMult * collectedisplay.mult);

        rarityManager.UpdateRarityDrops(collectedisplay.collected);

        collectedisplay.collected = 0;
        collectedisplay.chalk = 0;
        collectedisplay.homework = 0;

        collectedisplay.timer = 0f;
    }
}
