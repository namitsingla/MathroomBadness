using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public SpawnPoint[] spawnPoints;
    public Transform player;

    public float minDistanceFromPlayer = 100f;
    public float minDistanceFromOtherSpawns = 80f;

    public float minEnemyDistanceFromPlayer = 200f;
    public float maxEnemyDistanceFromPlayer = 400f;

    List<Vector3> spawnedPositions = new List<Vector3>();
    List<GameObject> spawnedCollectibles = new List<GameObject>();

    private string currentItemTag = "Homework";

    public int spawnCount = 4;
    public int spawnedCount = 0;
    public GameObject playerGameObject;
    public static SpawnManager instance;

    void Awake()
    {
        instance = this;
    }
    
    // Call this after procedural generation to update spawn points
    public void CacheSpawnPoints()
    {
        spawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
        Debug.Log("Cached " + spawnPoints.Length + " spawn points.");
    }

    List<SpawnPoint> GetValidItemSpawnPoints()
    {
        List<SpawnPoint> valid = new List<SpawnPoint>(spawnPoints);

        valid.RemoveAll(sp =>
            Vector3.Distance(sp.transform.position, player.position) < minDistanceFromPlayer);

        foreach (var pos in spawnedPositions)
            valid.RemoveAll(sp =>
                Vector3.Distance(sp.transform.position, pos) < minDistanceFromOtherSpawns);

        return valid;
    }

    List<SpawnPoint> GetValidEnemySpawnPoints()
    {
        List<SpawnPoint> valid = new List<SpawnPoint>(spawnPoints);

        valid.RemoveAll(sp =>
            Vector3.Distance(sp.transform.position, player.position) < minEnemyDistanceFromPlayer);

        valid.RemoveAll(sp =>
            Vector3.Distance(sp.transform.position, player.position) > maxEnemyDistanceFromPlayer);

        return valid;
    }

    List<SpawnPoint> GetValidPlayerSpawnPoints()
    {
        List<SpawnPoint> valid = new List<SpawnPoint>(spawnPoints);

        foreach (BaseEnemy enemy in EnemyManager.instance.GetAllEnemies())
        {
            valid.RemoveAll(sp =>
                Vector3.Distance(sp.transform.position, enemy.transform.position) < 150f);
        }

        return valid;
    }

    SpawnPoint GetFarthestSpawnPoint()
    {
        SpawnPoint farthest = null;
        float maxDist = 0f;

        foreach (var sp in spawnPoints)
        {
            float dist = Vector3.Distance(player.position, sp.transform.position);
            if (dist > maxDist)
            {
                maxDist = dist;
                farthest = sp;
            }
        }

        return farthest;
    }

    public void SpawnItem()
    {
        List<SpawnPoint> valid = GetValidItemSpawnPoints();

        SpawnPoint chosen = valid.Count != 0
            ? valid[Random.Range(0, valid.Count)]
            : GetFarthestSpawnPoint();

        GameObject obj = ObjectPooler.instance.Get(currentItemTag, chosen.transform.position, Quaternion.identity);
        spawnedCollectibles.Add(obj);
        spawnedCount += 1;

        currentItemTag = currentItemTag == "Homework" ? "Chalk" : "Homework";

        spawnedPositions.Add(chosen.transform.position);
    }

    public void DeleteAllCollectibles()
    {
        foreach (GameObject item in spawnedCollectibles)
        {
            if (item != null)
            {
                string tag = item.CompareTag("Homework") ? "Homework" : "Chalk";
                ObjectPooler.instance.ReturnToPool(tag, item);
            }
        }

        spawnedCollectibles.Clear();
        spawnedPositions.Clear();
    }

    public void SpawnAllEnemies()
    {
        EnemyManager.instance.SpawnDefaultEnemies();

        foreach (BaseEnemy enemy in EnemyManager.instance.GetAllEnemies())
            SpawnEnemy(enemy);
    }

    public void SpawnEnemy(BaseEnemy enemy)
    {
        List<SpawnPoint> valid = GetValidEnemySpawnPoints();

        SpawnPoint chosen = valid.Count != 0
            ? valid[Random.Range(0, valid.Count)]
            : GetFarthestSpawnPoint();

        enemy.WarpTo(chosen.transform.position);
    }

    public void TeleportPlayerAway()
    {
        List<SpawnPoint> valid = GetValidPlayerSpawnPoints();

        SpawnPoint chosen = valid.Count != 0
            ? valid[Random.Range(0, valid.Count)]
            : GetFarthestSpawnPoint();

        playerGameObject.GetComponent<CharacterController>().enabled = false;
        playerGameObject.transform.position = chosen.transform.position;
        playerGameObject.transform.position += new Vector3(0f, 2f, 0f);
        playerGameObject.GetComponent<CharacterController>().enabled = true;
    }

    // In SpawnManager.cs - add this method
    public void RespawnEnemiesInPlace()
    {
        foreach (BaseEnemy enemy in EnemyManager.instance.GetAllEnemies())
            SpawnEnemy(enemy);
    }
}