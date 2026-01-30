using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public BaldiEnemy baldiEnemy;
    public UnityEngine.AI.NavMeshAgent baldiNavMesh;
    public EnemyController oggyController;
    public UnityEngine.AI.NavMeshAgent oggyNavMesh;
    public UIIAController uIIAController;
    public UnityEngine.AI.NavMeshAgent uiiaNavMesh;
    public SpawnManager spawnManager;
    public GameManager gameManager;
    void Start()
    {
        //easy
        if (SettingsData.Difficulty == 0)
        {
            baldiEnemy.lookRadius *=  0.67f;
            baldiEnemy.speedIncrease *= 0.67f;
            baldiNavMesh.speed *= 0.7f;

            oggyController.lookRadius *= 0.67f;
            oggyNavMesh.speed *= 0.7f;

            uIIAController.lookRadius *= 0.67f;
            uIIAController.wallLifetime *= 0.5f;
            uiiaNavMesh.speed *= 0.7f;

            spawnManager.spawnCount += 1;
            spawnManager.minDistanceFromPlayer *= 0.75f;
            spawnManager.minDistanceFromOtherSpawns *= 0.75f;

            RenderSettings.fogDensity = 0.02f;
        }

        //hard
        if (SettingsData.Difficulty == 2)
        {
            baldiEnemy.lookRadius *=  1.5f;
            baldiNavMesh.speed *= 1.5f;

            oggyController.lookRadius *= 1.5f;
            oggyNavMesh.speed *= 1.5f;

            uIIAController.lookRadius *= 1.5f;
            uIIAController.wallLifetime *= 1.5f;
            uiiaNavMesh.speed *= 1.5f;

            //spawnManager.spawnCount -= 1;
            spawnManager.minDistanceFromPlayer *= 1.25f;
            spawnManager.minDistanceFromOtherSpawns *= 1.25f;

            RenderSettings.fogDensity = 0.05f;
        }

        //madness
        if (SettingsData.Difficulty == 3)
        {
            baldiEnemy.lookRadius *=  2f;
            baldiNavMesh.speed *= 2f;

            oggyController.lookRadius *= 2f;
            oggyNavMesh.speed *= 2f;

            uIIAController.lookRadius *= 2f;
            uIIAController.wallLifetime *= 2f;
            uiiaNavMesh.speed *= 2f;

            spawnManager.spawnCount -= 1;
            spawnManager.minDistanceFromPlayer *= 1.25f;
            spawnManager.minDistanceFromOtherSpawns *= 1.25f;

            RenderSettings.fogDensity = 0.06f;
            gameManager.lives = 1;
        }
    }
}
