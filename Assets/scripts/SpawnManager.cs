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

   public float minDistanceFromPlayer = 200f;
   public float minDistanceFromOtherSpawns = 100f;

   List<Vector3> spawnedPositions = new List<Vector3>();

   private GameObject currentItemToBeSpawned;

   public int spawnCount = 3;
   public int spawnedCount = 0;

    void Start()
    {
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
            Debug.Log("Player position: " + player.position);
            Debug.Log("Spawned an object at distance " + (int)Vector3.Distance(chosen.transform.position, player.position));

            foreach (var pos in spawnedPositions)
            {
                Debug.Log("Item position: " + pos);
                Debug.Log("Distance from said object: " + (int)Vector3.Distance(chosen.transform.position, pos));
            }

            Debug.Log(" ");
        }
        else
        {
            //for when no valid spawn point
            chosen = GetFarthestSpawnPoint();
            Debug.Log("No valid spawn point. Chose the farthest one possible");
        }

        GameObject obj = Instantiate(currentItemToBeSpawned, chosen.transform.position, Quaternion.identity);
        spawnedCount += 1;

        if (currentItemToBeSpawned == chalk) 
            currentItemToBeSpawned = homework;
        else currentItemToBeSpawned = chalk;

        spawnedPositions.Add(chosen.transform.position);
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

    // This is to assign spawn points automatically from inspector
    #if UNITY_EDITOR
    [ContextMenu("Auto Assign Spawn Points")]
    void AutoAssign()
    {
        spawnPoints = FindObjectsOfType<SpawnPoint>();
    }
    #endif
}
