using UnityEngine;

public class ReferencesManager : MonoBehaviour
{
    public static ReferencesManager instance;
   public collectedisplay collecteddisplay;  
   public BaldiWarningHide baldiWarning;
   public MusicManager musicManager;
   public DialogueSoundManager dialogueSoundManager;
   public SpawnManager spawnManager;
   public BoostsHandler boostsHandler;
   public Transform player;
   public PowerSystem powerSystem;
   public GameManager gameManager;
   public BaldiLookAnimation baldiLookAnimation;
   public GameObject exitDoorFailText;
   public AudioSource exitDoorFailSound;

    void Awake () 
    {
        instance = this;
    }
}
