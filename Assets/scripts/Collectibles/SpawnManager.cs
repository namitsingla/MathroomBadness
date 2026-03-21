using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
   public GameObject homework;
   public GameObject chalk;
   public SpawnPoint[] spawnPoints;
   public Transform player;

   public float minDistanceFromPlayer = 100f;
   public float minDistanceFromOtherSpawns = 80f;

   public UnityEngine.AI.NavMeshAgent baldi;
   public UnityEngine.AI.NavMeshAgent uiia;
   public UnityEngine.AI.NavMeshAgent oggy;

   public float minEnemyDistanceFromPlayer = 200f;
   public float maxEnemyDistanceFromPlayer = 400f;

   List<Vector3> spawnedPositions = new List<Vector3>();
    List<GameObject> spawnedCollectibles = new List<GameObject>();

   private GameObject currentItemToBeSpawned;

   public int spawnCount = 4;
   public int spawnedCount = 0;

   public Transform baldiTrans;
   public Transform oggyTrans;
   public Transform uiiaTrans;
   public GameObject playerGameObject;

    void Start()
    {
        SpawnAllEnemies();

        currentItemToBeSpawned = homework;

        for (int i = 0; i < spawnCount; i++)
        {
            SpawnItem();
        }
    }



   public void SpawnItem()
    {
        // Reset all spawn points
        foreach (var sp in spawnPoints)
            sp.active = true;

        // Disable near player
        foreach (var sp in spawnPoints)
        {
            if (Vector3.Distance(sp.transform.position, player.position) < minDistanceFromPlayer)
                sp.active = false;
        }

        // Disable near other spawned items
        foreach (var pos in spawnedPositions)
        {
            foreach (var sp in spawnPoints)
            {
                if (Vector3.Distance(sp.transform.position, pos) < minDistanceFromOtherSpawns)
                    sp.active = false;
            }
        }

        // Collect valid points
        List<SpawnPoint> valid = new List<SpawnPoint>();
        foreach (var sp in spawnPoints)
            if (sp.active) valid.Add(sp);

        // Pick random and spawn
        SpawnPoint chosen;

        if (valid.Count != 0) 
        {
            chosen = valid[Random.Range(0, valid.Count)]; 
            //Debug.Log("Player position: " + player.position);
            //Debug.Log("Spawned an object at distance " + (int)Vector3.Distance(chosen.transform.position, player.position));

            foreach (var pos in spawnedPositions)
            {
                //Debug.Log("Item position: " + pos);
                //Debug.Log("Distance from said object: " + (int)Vector3.Distance(chosen.transform.position, pos));
            }
        }
        else
        {
            //for when no valid spawn point
            chosen = GetRandomSpawnPoint();
            Debug.Log("No valid spawn point. Chose the farthest one possible");
        }

        GameObject obj = Instantiate(currentItemToBeSpawned, chosen.transform.position, Quaternion.identity);
        spawnedCollectibles.Add(obj);
        spawnedCount += 1;

        if (currentItemToBeSpawned == chalk) 
            currentItemToBeSpawned = homework;
        else currentItemToBeSpawned = chalk;

        spawnedPositions.Add(chosen.transform.position);
    }

    SpawnPoint GetRandomSpawnPoint()
    {
        SpawnPoint randoSpawn = null;

        // Reset all spawn points
        foreach (var sp in spawnPoints)
            sp.active = true;

        // Disable near player
        foreach (var sp in spawnPoints)
        {
            if (Vector3.Distance(sp.transform.position, player.position) < minDistanceFromPlayer)
                sp.active = false;
        }

        // Disable already spawned positions
        foreach (var pos in spawnedPositions)
        {
            foreach (var sp in spawnPoints)
            {
                if (Vector3.Distance(sp.transform.position, pos) < 1f)
                    sp.active = false;
            }
        }

        // Collect valid points
        List<SpawnPoint> valid = new List<SpawnPoint>();
        foreach (var sp in spawnPoints)
            if (sp.active) valid.Add(sp);

        randoSpawn = valid[Random.Range(0, valid.Count)]; 

        return randoSpawn;

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

    public void DeleteAllCollectibles()
    {
        foreach (GameObject item in spawnedCollectibles)
        {
            if (item != null)
                Destroy(item);
        }

        spawnedCollectibles.Clear();

        //to clear spawned positions
        spawnedPositions.Clear();
    }

    public void SpawnAllEnemies()
    {
        SpawnEnemy(baldi);
        SpawnEnemy(uiia);
        SpawnEnemy(oggy);
    }

    public void SpawnEnemy(UnityEngine.AI.NavMeshAgent enemy)
    {
        // Reset all spawn points
        foreach (var sp in spawnPoints)
            sp.active = true;

        // Disable near player
        foreach (var sp in spawnPoints)
        {
            if (Vector3.Distance(sp.transform.position, player.position) < minEnemyDistanceFromPlayer)
                sp.active = false;
        }

        // Disable far from player
        foreach (var sp in spawnPoints)
        {
            if (Vector3.Distance(sp.transform.position, player.position) > maxEnemyDistanceFromPlayer)
                sp.active = false;
        }

        // Collect valid points
        List<SpawnPoint> valid = new List<SpawnPoint>();
        foreach (var sp in spawnPoints)
            if (sp.active) valid.Add(sp);

        // Pick random and spawn
        SpawnPoint chosen;

        if (valid.Count != 0) 
        {
            chosen = valid[Random.Range(0, valid.Count)]; 
        }
        else
        {
            //for when no valid spawn point
            chosen = GetFarthestSpawnPoint();
            Debug.Log("No valid spawn point. Chose the farthest one possible");
        }

        enemy.Warp(chosen.transform.position);
    }

    public void TeleportPlayerAway()
    {
        // Reset all spawn points
        foreach (var sp in spawnPoints)
            sp.active = true;

        // Disable near baldi
        foreach (var sp in spawnPoints)
        {
            if (Vector3.Distance(sp.transform.position, baldiTrans.position) < 150f)
                sp.active = false;
        }

        // Disable near oggy
        foreach (var sp in spawnPoints)
        {
            if (Vector3.Distance(sp.transform.position, oggyTrans.position) < 150f)
                sp.active = false;
        }

        // Disable near uiia
        foreach (var sp in spawnPoints)
        {
            if (Vector3.Distance(sp.transform.position, uiiaTrans.position) < 150f)
                sp.active = false;
        }

        // Collect valid points
        List<SpawnPoint> valid = new List<SpawnPoint>();
        foreach (var sp in spawnPoints)
            if (sp.active) valid.Add(sp);

        // Pick random and spawn
        SpawnPoint chosen;

        if (valid.Count != 0) 
        {
            chosen = valid[Random.Range(0, valid.Count)]; 
        }
        else
        {
            //for when no valid spawn point
            chosen = GetFarthestSpawnPoint();
            Debug.Log("No valid spawn point. Chose the farthest one possible");
        }

        playerGameObject.GetComponent<CharacterController>().enabled = false;
        playerGameObject.transform.position = chosen.transform.position;
        playerGameObject.transform.position += new Vector3(0f, 2f, 0f);
        playerGameObject.GetComponent<CharacterController>().enabled = true;

    }

    // This is to assign spawn points automatically from inspector
    #if UNITY_EDITOR
    [ContextMenu("Auto Assign Spawn Points")]
    void AutoAssign()
    {
        spawnPoints = FindObjectsOfType<SpawnPoint>();
    }
    #endif
}
