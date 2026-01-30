using UnityEngine;
using TMPro;

public class collectedisplay : MonoBehaviour
{
   public TextMeshProUGUI textMeshPro; // Assign in Inspector

   public int homework = 0;
   public int chalk = 0;
    public int collected = 0;

    public void UpdateDisplay()
    {
        textMeshPro.text = "Collected: " + collected;
    }
}
