using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class ExitDoor : MonoBehaviour
{
    public collectedisplay collectedisplay;

    public Renderer targetRenderer;
    private MaterialPropertyBlock mpb;
    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    public GameObject lockedIcon;
    public AudioSource BGM;
    public ScoreCalculator scoreCalculator;
    public GameObject rewardsUI;
    public GameObject player;
    Vector3 playerspawn = new Vector3(40f, 2.1f, 100f);
    public SpawnManager spawnManager;
    public BaldiEnemy baldiEnemy;
    public UnityEngine.AI.NavMeshAgent uiia;
    public EnemyController oggy;
    public ExitDoorSpawner exitDoorSpawner;
    public RarityManager rarityManager;
    public UIIAController uIIAController;
    public AudioSource exitDoorFailSound;
    public GameObject exitDoorFailText;
    public GameObject baldiWarningcanvas;
    public int requiredItems = 3;
    
    private bool isProcessing = false;
    TextMeshProUGUI exitDoorFailTMP;
    
    void Awake()
    {
        mpb = new MaterialPropertyBlock();

        exitDoorFailTMP = exitDoorFailText.GetComponent<TextMeshProUGUI>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (isProcessing) return;

        if (collectedisplay.collected < requiredItems)
        {
            isProcessing = true; // Lock the trigger
            exitDoorFailSound.Play();
            StartCoroutine(ExitDoorFAilText());
        } 
        else
        {
            isProcessing = true; // Lock the trigger
            RoundEndSequence();
        }
    }

    IEnumerator ExitDoorFAilText()
    {
        if (collectedisplay.collected == 0)
        {
            exitDoorFailTMP.text = "You need atleast " + requiredItems + " items";
        } else
        {
            exitDoorFailTMP.text = "You need atleast " + (requiredItems-collectedisplay.collected) + " more items";
        }


        exitDoorFailText.SetActive(true);

        // Since timeScale might be 0 later, it's safer to use real time for UI delays just in case
        yield return new WaitForSecondsRealtime(3f); 
        
        exitDoorFailText.SetActive(false);
        isProcessing = false;
    }

    public void ActivateExitDoor()
    {
        targetRenderer.GetPropertyBlock(mpb);
        mpb.SetColor(BaseColorID, Color.green);
        targetRenderer.SetPropertyBlock(mpb);

        lockedIcon.SetActive(false);
    }

    public void DeactivateExitDoor()
    {
        targetRenderer.GetPropertyBlock(mpb);
        mpb.SetColor(BaseColorID, Color.red);
        targetRenderer.SetPropertyBlock(mpb);

        lockedIcon.SetActive(true);
        // THE FIX: We removed 'isProcessing = false;' from here so it doesn't unlock instantly!
    }

    public void RoundEndSequence()
    {
        baldiWarningcanvas.SetActive(false);

        Time.timeScale = 0f;
        BGM.Pause();

        Cursor.lockState = CursorLockMode.None;
        scoreCalculator.CalculateScore();

        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = playerspawn;
        player.GetComponent<CharacterController>().enabled = true;

        spawnManager.SpawnAllEnemies();
        spawnManager.DeleteAllCollectibles();

        for (int i = 0; i < spawnManager.spawnCount; i++)
        {
            spawnManager.SpawnItem();
        }

        baldiEnemy.baldiBaseSpeed *= 1.2f;
        baldiEnemy.agent.speed = baldiEnemy.baldiBaseSpeed;
        uiia.speed *= 1.2f;
        oggy.oggyBAseSpeed *= 1.2f;

        uIIAController.DeactivateAllWalls();
        
        exitDoorSpawner.SpawnExitDoor();
        DeactivateExitDoor();

        rewardsUI.SetActive(true);

        // THE FIX: Start a coroutine to unlock the door safely after physics is done
        StartCoroutine(UnlockDoorSafely());
    }

    // THE FIX: This coroutine waits 0.1 real-time seconds before unlocking the trigger
    IEnumerator UnlockDoorSafely()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        isProcessing = false;
    }
}