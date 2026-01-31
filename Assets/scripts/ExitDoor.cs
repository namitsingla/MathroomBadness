using UnityEngine;
using UnityEngine.SceneManagement;

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
    public UnityEngine.AI.NavMeshAgent oggy;
    public ExitDoorSpawner exitDoorSpawner;
    public RarityManager rarityManager;
    void Awake()
    {
        //targetRenderer = ReferencesManager.instance.targetRenderer;
        //lockedIcon = ReferencesManager.instance.lockedIcon;
        mpb = new MaterialPropertyBlock();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (collectedisplay.collected < 3)
        {
            Debug.Log("You must collect atleast 3 items to open the exit.");
        } 
        else
        {
            RoundEndSequence();
        }
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
    }

    public void RoundEndSequence()
    {
        Time.timeScale = 0f;
        BGM.Pause();

        Cursor.lockState = CursorLockMode.None;
        scoreCalculator.CalculateScore();

        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = playerspawn;
        player.GetComponent<CharacterController>().enabled = true;

        spawnManager.SpawnAllEnemies();

        baldiEnemy.baldiBaseSpeed *= 1.1f;
        baldiEnemy.agent.speed = baldiEnemy.baldiBaseSpeed;
        uiia.speed *= 1.1f;
        oggy.speed *= 1.1f;
        
        DeactivateExitDoor();
        exitDoorSpawner.SpawnExitDoor();

        rewardsUI.SetActive(true);
        rarityManager.GenerateRewards();
    }
}
