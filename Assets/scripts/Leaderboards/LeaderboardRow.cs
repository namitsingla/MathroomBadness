using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeaderboardRow : MonoBehaviour
{
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;
    public Image backgroundImage; // The panel behind the text so we can color it

    public void Setup(string rank, string name, string score, Color bgColor)
    {
        rankText.text = rank + ".";
        nameText.text = name;
        scoreText.text = score;
        if (backgroundImage != null)
        {
            backgroundImage.color = bgColor;
        }
    }
}