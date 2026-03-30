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
    
    // 1. Define both the Base Color and Emission Color IDs
    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");
    
    // 2. Add an adjustable intensity multiplier for the glow
    public float glowIntensity = 1.5f; 

    public GameObject lockedIcon;
    public AudioSource BGM;
    public ScoreCalculator scoreCalculator;
    public GameObject rewardsUI;
    public GameObject player;
    Vector3 playerspawn = new Vector3(40f, 3.1f, 100f);
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
        
        // 3. CRITICAL: Enable emission on the material so the shader knows to render it
        if (targetRenderer != null)
        {
            targetRenderer.material.EnableKeyword("_EMISSION");
        }
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
        
        // 4. Set both the base color and the HDR emission color (Color * Intensity)
        mpb.SetColor(BaseColorID, Color.green);
        mpb.SetColor(EmissionColorID, Color.green * glowIntensity);
        
        targetRenderer.SetPropertyBlock(mpb);
        lockedIcon.SetActive(false);
    }

    public void DeactivateExitDoor()
    {
        targetRenderer.GetPropertyBlock(mpb);
        
        // 5. Set both to red
        mpb.SetColor(BaseColorID, Color.red);
        mpb.SetColor(EmissionColorID, Color.red * glowIntensity);
        
        targetRenderer.SetPropertyBlock(mpb);

        lockedIcon.SetActive(true);
    }

    public void RoundEndSequence()
    {
        rarityManager.isRewardScreenUp = true;

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

        baldiEnemy.baldiBaseSpeed *= 1.17f;
        baldiEnemy.agent.speed = baldiEnemy.baldiBaseSpeed;
        uiia.speed *= 1.2f;
        oggy.oggyBAseSpeed *= 1.2f;

        uIIAController.DeactivateAllWalls();
        
        exitDoorSpawner.SpawnExitDoor();
        DeactivateExitDoor();

        rewardsUI.SetActive(true);

        StartCoroutine(UnlockDoorSafely());
    }

    IEnumerator UnlockDoorSafely()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        isProcessing = false;
    }
}