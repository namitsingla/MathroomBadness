using UnityEngine;
using TMPro;

public class collectedisplay : MonoBehaviour
{
   public TextMeshProUGUI collectedtext;
   public TextMeshProUGUI scoretext;
   public TextMeshProUGUI multtext;

   public int homework = 0;
   public int chalk = 0;
    public int collected = 0;
    public int score = 0;
    public float timer;
    public float mult = 1f;

    public void UpdateDisplay()
    {
        collectedtext.text = "Collected: " + collected;
        scoretext.text = "Score: " + score;
        multtext.text = "Multiplier: " + mult.ToString("F2") + "X";
    }

    public void Update()
    {
        UpdateDisplay();
        timer += Time.deltaTime;
    }
}
