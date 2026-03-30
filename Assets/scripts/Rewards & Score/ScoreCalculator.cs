using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    public collectedisplay collectedisplay;
    public ScoreCounter scoreCounter;
    public RarityManager rarityManager;

    public void CalculateScore()
    {
        float highRewardRate = 1f; 
        float lowRewardRate = 0.25f;   
        float itemScore = (collectedisplay.collected <= 6) ? (collectedisplay.collected * highRewardRate) : ((6 * highRewardRate) + ((collectedisplay.collected - 6) * lowRewardRate));

        // --- 2. Effective Time (Piecewise Decay) ---
        float effectiveTime = 0f;

        // Time thresholds
        float phase1Limit = 30f; 
        float phase2Limit = 150f; 

        // Decay Multipliers (Tweak these to balance the curve)
        float dSlow = 0.25f;   // Very forgiving
        float dNormal = 1f; // Punishing
        float dLow = 0.1f;    // Flattens out

        if (collectedisplay.timer <= phase1Limit) 
        {
            // Player finished in under 2 minutes
            effectiveTime = collectedisplay.timer * dSlow;
        } 
        else if (collectedisplay.timer <= phase2Limit) 
        {
            // Player finished between 2 and 5 minutes
            float phase1Penalty = phase1Limit * dSlow;
            effectiveTime = phase1Penalty + ((collectedisplay.timer - phase1Limit) * dNormal);
        } 
        else 
        {
            // Player took longer than 4 minutes
            float phase1Penalty = phase1Limit * dSlow;
            float phase2Penalty = (phase2Limit - phase1Limit) * dNormal;
            effectiveTime = phase1Penalty + phase2Penalty + ((collectedisplay.timer - phase2Limit) * dLow);
        }

        // --- 3. Final Calculation ---
        float globalMultiplier = 10000f; // Scales the final number up
        float timeConstant = 22.5f;     // Prevents dividing by tiny numbers/zero

        float denominator = effectiveTime + timeConstant;
        int scorewithoutMult = (int) ((itemScore * globalMultiplier) / denominator);

        //int scorewithoutMult = (int) (finalScore);
        scoreCounter.AnimateScore(scorewithoutMult, collectedisplay.mult);

        collectedisplay.score += Mathf.RoundToInt(scorewithoutMult * collectedisplay.mult);

        rarityManager.UpdateRarityDrops(collectedisplay.collected);

        collectedisplay.collected = 0;
        collectedisplay.chalk = 0;
        collectedisplay.homework = 0;

        collectedisplay.timer = 0f;
    }
}
