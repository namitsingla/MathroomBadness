using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;

public class RoundManager : MonoBehaviour
{
    public static RoundManager instance;

    [Header("References")]
    public GameManager gameManager;
    public SpawnManager spawnManager;
    public EnemyManager enemyManager;
    public collectedisplay collectedisplay;
    public ExitDoor exitDoor;
    public ExitDoorSpawner exitDoorSpawner;
    public RarityManager rarityManager;
    public ScoreCalculator scoreCalculator;
    public MusicManager musicManager;
    public BoostsHandler boostsHandler;
    public PowerSystem powerSystem;
    public ExitDoorManager exitDoorManager;

    [Header("Player")]
    public GameObject player;
    public Vector3 playerSpawn = new Vector3(40f, 3.1f, 100f);

    [Header("UI")]
    public GameObject rewardsUI;
    public GameObject baldiWarningCanvas;
    public AudioSource BGM;

    [Header("Enemy Speed Scaling")]
    public float enemySpeedScale = 1.15f;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        DifficultyManager.instance.ApplyDifficultySettings();
        StartCoroutine(ResetMap());
    }

    void CacheEverything()
    {
        NavigationPointsManager.instance.CacheAllPoints();
        spawnManager.CacheSpawnPoints();
        ExitDoorManager.instance.CacheSpawnPoints();
    }

    public void OnRoundEnd()
    {
        StartCoroutine(OnRoundEndSequence());
    }

    IEnumerator OnRoundEndSequence()
    {
        rarityManager.isRewardScreenUp = true;
        rarityManager.isReadyToShowRewards = false;
        baldiWarningCanvas.SetActive(false);

        Time.timeScale = 0f;
        enemyManager.PauseUIIAMusic();
        BGM.Pause();
        Cursor.lockState = CursorLockMode.None;

        gameManager.round += 1;
        yield return StartCoroutine(ResetMap());

        rewardsUI.SetActive(true);
        scoreCalculator.CalculateScore();

        yield return new WaitUntil(() => rarityManager.isReadyToShowRewards);
        StartCoroutine(rarityManager.GenerateRewards());

        yield return new WaitForSecondsRealtime(0.1f);

        // unlock doors after reward screen is ready
        foreach (ExitDoor door in exitDoorManager.activeDoors)
            door.isProcessing = false;
    }

    public void OnRewardPicked()
    {
        rewardsUI.SetActive(false);
        rarityManager.isRewardScreenUp = false;

        Time.timeScale = gameManager.gameSpeed;
        enemyManager.UnpauseUIIAMusic();
        BGM.UnPause();
        Cursor.lockState = CursorLockMode.Locked;

        ResetCollectedDisplay();

        StartCoroutine(powerSystem.StunAllEnemies(3f));

        if (boostsHandler.ifRandomPowerUpEachRound)
            boostsHandler.GetRandomPowerUp();

        musicManager.UpdateBackgroundMusic();
    }

    void ResetPlayerPosition()
    {
        spawnManager.TeleportPlayerAway();
    }

    void ResetCollectedDisplay()
    {
        collectedisplay.collected = 0;
        collectedisplay.homework = 0;
        collectedisplay.chalk = 0;
        collectedisplay.mult += 0.1f;
        collectedisplay.UpdateDisplay();
    }

    IEnumerator ResetMap()
    {
        // despawning everything
        spawnManager.DeleteAllCollectibles();
        exitDoorManager.DespawnAllExitDoors();
        enemyManager.DespawnAllEnemies();

        enemyManager.CheckPrisonReleases(gameManager.round);
        RoundEvents();

        // so when map changes we have all the points
        CacheEverything();

        yield return null;

        ResetPlayerPosition();

        yield return null;

        enemyManager.SpawnDefaultEnemies();
        ScaleEnemySpeeds();

        // warp each enemy to a valid spawn point
        foreach (BaseEnemy enemy in enemyManager.GetAllEnemies())
            spawnManager.SpawnEnemy(enemy);

        WallManager.instance.DeactivateAllWalls();

        for (int i = 0; i < spawnManager.spawnCount; i++)
            spawnManager.SpawnItem();

        // despawn old doors and spawn fresh ones
        exitDoorManager.SpawnExitDoors();
    }

    void ScaleEnemySpeeds()
    {
        foreach (BaldiEnemy baldi in enemyManager.GetAllEnemiesOfType<BaldiEnemy>())
        {
            baldi.baldiBaseSpeed *= enemySpeedScale;
            baldi.baseSpeed = baldi.baldiBaseSpeed;
            baldi.agent.speed = baldi.baseSpeed;
        }

        foreach (UIIAController uiia in enemyManager.GetAllEnemiesOfType<UIIAController>())
        {
            uiia.uiiaBaseSpeed *= enemySpeedScale;
            uiia.baseSpeed = uiia.uiiaBaseSpeed;
            uiia.agent.speed = uiia.baseSpeed;
        }

        foreach (EnemyController oggy in enemyManager.GetAllEnemiesOfType<EnemyController>())
        {
            oggy.oggyBaseSpeed *= enemySpeedScale;
            oggy.baseSpeed = oggy.oggyBaseSpeed;
        }
    }

    void RoundEvents()
    {
        if (gameManager.round % 3 == 0)
            enemyManager.AddToRoster(enemyManager.uiiaTag);

        if (gameManager.round % 7 == 0)
            enemyManager.AddToRoster(enemyManager.baldiTag);

        if (gameManager.round % 10 == 0)
            enemyManager.AddToRoster(enemyManager.oggyTag);

        if (gameManager.round % 4 == 0)
            exitDoorManager.exitDoorCount += 1;

    }
}