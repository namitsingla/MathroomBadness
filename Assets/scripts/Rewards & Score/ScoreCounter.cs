using TMPro;
using UnityEngine;
using System.Collections;

public class ScoreCounter : MonoBehaviour
{
    public TextMeshProUGUI baseScoreText;
    public TextMeshProUGUI multiplierText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI totalScoreText;
    public AudioSource countingSound;
    public AudioSource totalScoreSound;
    private int totalScore;
    private int finalScore;

    public float minCountDuration = 1f;
    public float maxCountDuration = 3f;
    public float crashDuration = 0.25f;

    private Vector2 originalBasePos;
    private Vector2 originalMultPos;
    private Vector3 originalBaseScale;
    private Vector3 originalMultScale;

    private Coroutine currentRoutine;

    public RarityManager rarityManager;

    void Awake()
    {
        originalBasePos = baseScoreText.rectTransform.anchoredPosition;
        originalMultPos = multiplierText.rectTransform.anchoredPosition;

        originalBaseScale = baseScoreText.rectTransform.localScale;
        originalMultScale = multiplierText.rectTransform.localScale;
    }

    public void AnimateScore(int baseScore, float multiplier)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        countingSound.Play();
        currentRoutine = StartCoroutine(CountThenCrash(baseScore, multiplier));
    }

    private IEnumerator CountThenCrash(int baseScore, float multiplier)
    {
        // -------------------------
        // RESET STATE (Reusable)
        // -------------------------

        baseScoreText.gameObject.SetActive(true);
        multiplierText.gameObject.SetActive(true);
        finalScoreText.gameObject.SetActive(false);

        baseScoreText.rectTransform.anchoredPosition = originalBasePos;
        multiplierText.rectTransform.anchoredPosition = originalMultPos;

        baseScoreText.rectTransform.localScale = originalBaseScale;
        multiplierText.rectTransform.localScale = originalMultScale;

        finalScoreText.color = Color.white;
        finalScoreText.transform.localScale = Vector3.one;

        multiplierText.text = "X" + multiplier.ToString("0.00");

        // -------------------------
        // 1️⃣ COUNT BASE SCORE
        // -------------------------

        float duration = Mathf.Clamp(baseScore / 1000f, minCountDuration, maxCountDuration);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = elapsed / duration;
            t = 1 - Mathf.Pow(1 - t, 3); // easeOutCubic
            countingSound.pitch = 1f + (t * 0.3f);

            int current = Mathf.RoundToInt(Mathf.Lerp(0, baseScore, t));
            baseScoreText.text = current.ToString("N0");

            yield return null;
        }

        baseScoreText.text = baseScore.ToString("N0");
        countingSound.Stop();

        yield return new WaitForSecondsRealtime(0.25f);

        // -------------------------
        // 2️⃣ CRASH TOGETHER
        // -------------------------

        RectTransform baseRect = baseScoreText.rectTransform;
        RectTransform multRect = multiplierText.rectTransform;

        Vector2 baseStart = baseRect.anchoredPosition;
        Vector2 multStart = multRect.anchoredPosition;

        float centerX = (baseStart.x + multStart.x) / 2f;
        float offset = 0f;

        Vector2 baseTarget = new Vector2(centerX - offset, baseStart.y);
        Vector2 multTarget = new Vector2(centerX + offset, multStart.y);

        float crashElapsed = 0f;

        while (crashElapsed < crashDuration)
        {
            crashElapsed += Time.unscaledDeltaTime;
            float t = crashElapsed / crashDuration;
            t = 1 - Mathf.Pow(1 - t, 3); // easeOutCubic

            baseRect.anchoredPosition = Vector2.Lerp(baseStart, baseTarget, t);
            multRect.anchoredPosition = Vector2.Lerp(multStart, multTarget, t);

            yield return null;
        }

        // Impact shrink effect
        float shrinkDuration = 0.1f;
        float shrinkElapsed = 0f;

        while (shrinkElapsed < shrinkDuration)
        {
            shrinkElapsed += Time.unscaledDeltaTime;
            float t = shrinkElapsed / shrinkDuration;

            baseRect.localScale = Vector3.Lerp(originalBaseScale, Vector3.zero, t);
            multRect.localScale = Vector3.Lerp(originalMultScale, Vector3.zero, t);

            yield return null;
        }

        baseScoreText.gameObject.SetActive(false);
        multiplierText.gameObject.SetActive(false);

        // -------------------------
        // 3️⃣ FINAL SCORE COUNT-UP
        // -------------------------

        countingSound.Play();

        finalScore = Mathf.RoundToInt(baseScore * multiplier);
        finalScoreText.gameObject.SetActive(true);

        float finalDuration = 0.6f;
        float finalElapsed = 0f;

        finalScoreText.color = Color.yellow;
        finalScoreText.transform.localScale = Vector3.one * 1.5f;

        while (finalElapsed < finalDuration)
        {
            finalElapsed += Time.unscaledDeltaTime;

            float t = finalElapsed / finalDuration;
            t = 1 - Mathf.Pow(1 - t, 3); // easeOutCubic
            countingSound.pitch = 1f + (t * 1f);

            int current = Mathf.RoundToInt(Mathf.Lerp(0, finalScore, t));
            finalScoreText.text = current.ToString("N0");

            yield return null;
        }

        finalScoreText.text = finalScore.ToString("N0");
        countingSound.Stop();

        // Smooth scale back + color fade
        yield return StartCoroutine(FinalPolish());
    }

    private IEnumerator FinalPolish()
    {
        float duration = 0.3f;
        float elapsed = 0f;

        Vector3 startScale = finalScoreText.transform.localScale;
        Vector3 endScale = Vector3.one;

        Color startColor = Color.yellow;
        Color endColor = Color.white;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;

            finalScoreText.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            finalScoreText.color = Color.Lerp(startColor, endColor, t);

            yield return null;
        }

        finalScoreText.transform.localScale = Vector3.one;
        finalScoreText.color = Color.white;

        yield return StartCoroutine(FlyToTotal(finalScore));
    }

    private IEnumerator FlyToTotal(int amount)
    {
        // Hide the original final score text
        finalScoreText.gameObject.SetActive(false);

        // Create temporary flying text
        TextMeshProUGUI flyingText = Instantiate(finalScoreText, finalScoreText.transform.parent);
        flyingText.gameObject.SetActive(true);

        flyingText.text = amount.ToString("N0");
        flyingText.color = Color.white;

        RectTransform flyRect = flyingText.rectTransform;
        RectTransform totalRect = totalScoreText.rectTransform;

        // Start position = where final score was
        flyRect.anchoredPosition = finalScoreText.rectTransform.anchoredPosition;

        // Target position = total score position
        Vector2 startPos = flyRect.anchoredPosition;
        Vector2 targetPos = totalRect.anchoredPosition + new Vector2(-250f, -300f);

        // Capture scale sizes
        Vector3 startScale = Vector3.one;
        Vector3 targetScale = new Vector3(0.1f, 0.1f, 0.1f);

        float duration = 0.3f;
        float elapsed = 0f;

        totalScoreSound.Play();

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = elapsed / duration;
            t = 1 - Mathf.Pow(1 - t, 3); // easeOutCubic

            // Create arc control point (above midpoint)
            Vector2 controlPoint = (startPos + targetPos) / 2f + Vector2.up * -500f;

            // Quadratic Bezier curve
            Vector2 a = Vector2.Lerp(startPos, controlPoint, t);
            Vector2 b = Vector2.Lerp(controlPoint, targetPos, t);
            Vector2 curvedPos = Vector2.Lerp(a, b, t);

            flyRect.anchoredPosition = curvedPos;

            // Keep your scaling exactly the same
            flyRect.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        // Add to total score
        totalScore += amount;
        totalScoreText.text = "Score: " + totalScore.ToString("N0");

        Destroy(flyingText.gameObject);

        yield return StartCoroutine(PunchTotal());
    }

    private IEnumerator PunchTotal()
    {
        float duration = 0.2f;
        float elapsed = 0f;

        Vector3 start = Vector3.one * 1.4f;
        Vector3 end = Vector3.one;

        totalScoreText.transform.localScale = start;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);

            totalScoreText.transform.localScale = Vector3.Lerp(start, end, t);
            yield return null;
        }

        totalScoreText.transform.localScale = Vector3.one;

        rarityManager.StartCoroutine(rarityManager.GenerateRewards());
    }
}