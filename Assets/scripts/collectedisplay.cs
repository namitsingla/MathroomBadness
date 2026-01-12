using UnityEngine;
using TMPro;

public class collectedisplay : MonoBehaviour
{
   public TextMeshProUGUI textMeshPro; // Assign in Inspector

   public int homework = 0;
   public int chalk = 0;
    public int collected = 0;

    void Update()
    {
        textMeshPro.text = "Collected: " + collected + "/7";
    }
}
