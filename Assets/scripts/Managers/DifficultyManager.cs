using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager instance;
    public SpawnManager spawnManager;
    public GameManager gameManager;
    public Volume globalVolume;
    private SplitToning splitToning;
    private Vignette vignette;

    private bool difficultyApplied = false;

    void Awake()
    {
        instance = this;
        globalVolume.profile.TryGet(out splitToning);
        globalVolume.profile.TryGet(out vignette);
    }

    public void ApplyDifficultySettings()
    {
        if (difficultyApplied) return;
        difficultyApplied = true;

        if (SettingsData.Difficulty == 0) ApplyEasyGlobal();
        if (SettingsData.Difficulty == 2) ApplyHardGlobal();
        if (SettingsData.Difficulty == 3) ApplyMadnessGlobal();
    }

    // called for every enemy that spawns so new instances get the right values
    public void ApplyDifficultyToEnemy(BaseEnemy enemy)
    {
        if (!difficultyApplied) return;
        if (SettingsData.Difficulty == 0) ApplyEasyToEnemy(enemy);
        if (SettingsData.Difficulty == 2) ApplyHardToEnemy(enemy);
        if (SettingsData.Difficulty == 3) ApplyMadnessToEnemy(enemy);
    }

    void ApplyEasyGlobal()
    {
        spawnManager.spawnCount += 1;
        spawnManager.minDistanceFromPlayer *= 0.75f;
        spawnManager.minDistanceFromOtherSpawns *= 0.75f;
        spawnManager.minEnemyDistanceFromPlayer *= 1.25f;
        spawnManager.maxEnemyDistanceFromPlayer *= 1.25f;
        RenderSettings.fogDensity = 0.025f;
    }

    void ApplyHardGlobal()
    {
        spawnManager.spawnCount -= 1;
        spawnManager.minDistanceFromPlayer *= 1.25f;
        spawnManager.minDistanceFromOtherSpawns *= 1.25f;
        spawnManager.minEnemyDistanceFromPlayer *= 0.67f;
        spawnManager.maxEnemyDistanceFromPlayer *= 0.9f;
        RenderSettings.fogDensity = 0.055f;
    }

    void ApplyMadnessGlobal()
    {
        spawnManager.spawnCount -= 2;
        spawnManager.minDistanceFromPlayer *= 1.25f;
        spawnManager.minDistanceFromOtherSpawns *= 1.25f;
        spawnManager.minEnemyDistanceFromPlayer *= 0.25f;
        spawnManager.maxEnemyDistanceFromPlayer *= 0.9f;
        RenderSettings.fogDensity = 0.06f;
        gameManager.lives = 1;
        SetSplitToning(true);
    }

    void ApplyEasyToEnemy(BaseEnemy enemy)
    {
        enemy.lookRadius *= 0.67f;
        if (enemy is BaldiEnemy baldi)
        {
            baldi.baldiBaseSpeed *= 0.7f;
            baldi.speedIncrease *= 0.67f;
            baldi.baseSpeed = baldi.baldiBaseSpeed;
            baldi.agent.speed = baldi.baseSpeed;
        }
        else if (enemy is UIIAController uiia)
        {
            uiia.uiiaBaseSpeed *= 0.7f;
            uiia.wallLifetime *= 0.5f;
            uiia.baseSpeed = uiia.uiiaBaseSpeed;
            uiia.agent.speed = uiia.baseSpeed;
        }
        else if (enemy is EnemyController oggy)
        {
            oggy.oggyBaseSpeed *= 0.7f;
            oggy.baseSpeed = oggy.oggyBaseSpeed;
        }
    }

    void ApplyHardToEnemy(BaseEnemy enemy)
    {
        enemy.lookRadius *= 1.5f;
        if (enemy is BaldiEnemy baldi)
        {
            baldi.baldiBaseSpeed *= 1.5f;
            baldi.baseSpeed = baldi.baldiBaseSpeed;
            baldi.agent.speed = baldi.baseSpeed;
        }
        else if (enemy is UIIAController uiia)
        {
            uiia.uiiaBaseSpeed *= 1.5f;
            uiia.wallLifetime *= 1.5f;
            uiia.baseSpeed = uiia.uiiaBaseSpeed;
            uiia.agent.speed = uiia.baseSpeed;
        }
        else if (enemy is EnemyController oggy)
        {
            oggy.oggyBaseSpeed *= 1.5f;
            oggy.baseSpeed = oggy.oggyBaseSpeed;
        }
    }

    void ApplyMadnessToEnemy(BaseEnemy enemy)
    {
        enemy.lookRadius *= 2f;
        if (enemy is BaldiEnemy baldi)
        {
            baldi.baldiBaseSpeed *= 2f;
            baldi.baseSpeed = baldi.baldiBaseSpeed;
            baldi.agent.speed = baldi.baseSpeed;
        }
        else if (enemy is UIIAController uiia)
        {
            uiia.uiiaBaseSpeed *= 2f;
            uiia.wallLifetime *= 2f;
            uiia.baseSpeed = uiia.uiiaBaseSpeed;
            uiia.agent.speed = uiia.baseSpeed;
        }
        else if (enemy is EnemyController oggy)
        {
            oggy.oggyBaseSpeed *= 2f;
            oggy.baseSpeed = oggy.oggyBaseSpeed;
        }
    }

    public void SetSplitToning(bool state)
    {
        if (splitToning == null) return;
        splitToning.active = state;
        vignette.intensity.value = Mathf.Clamp01(0.25f);
    }
}