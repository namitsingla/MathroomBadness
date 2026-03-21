using UnityEngine;
using TMPro;

public class Livesdisplay : MonoBehaviour
{
  public TextMeshProUGUI livestext; // Assign in Inspector

  public GameManager targetScript;
  void Update()
  {
    livestext.text = "Lives Left: " + targetScript.lives;
    }  
}
