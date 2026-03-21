using UnityEngine;

public class ReferencesManager : MonoBehaviour
{
    public static ReferencesManager instance;

   public GameObject baldi;
   public collectedisplay collecteddisplay;  
   public BaldiWarningHide baldiWarning;
   public MusicManager musicManager;
   public DialogueSoundManager dialogueSoundManager;
   public SpawnManager spawnManager;
   public BaldiEnemy baldiEnemy;
   //public Renderer targetRenderer;
   //public GameObject lockedIcon;
   public ExitDoor exitDoor;
   public BoostsHandler boostsHandler;
   public Transform player;

    void Awake () 
    {
        instance = this;
    }
}
