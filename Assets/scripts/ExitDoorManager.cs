using System.Collections.Generic;
using UnityEngine;

public class ExitDoorManager : MonoBehaviour
{
    public static ExitDoorManager instance;

    [Header("Pool")]
    public string exitDoorTag = "ExitDoor";

    [Header("Spawn Settings")]
    public int exitDoorCount = 3;
    public float minDistanceBetweenDoors = 150f;

    public List<ExitDoor> activeDoors = new List<ExitDoor>();
    public ExitDoorSpawnPoint[] spawnPoints;

    void Awake()
    {
        instance = this;
    }
    public void CacheSpawnPoints()
    {
        spawnPoints = FindObjectsByType<ExitDoorSpawnPoint>(FindObjectsSortMode.None);
        Debug.Log("Cached " + spawnPoints.Length + " exit door spawn points.");
    }

    // called at round start
    public void SpawnExitDoors()
    {
        for (int i = 0; i < exitDoorCount; i++)
            SpawnSingleExitDoor();
    }

    // spawn one additional exit door at runtime
    public void AddExitDoor()
    {
        SpawnSingleExitDoor();
    }

    // remove one exit door at runtime
    public void RemoveExitDoor()
    {
        if (activeDoors.Count == 0) return;

        ExitDoor door = activeDoors[activeDoors.Count - 1];
        ReturnDoorToPool(door);
    }

    public void DespawnAllExitDoors()
    {
        for (int i = activeDoors.Count - 1; i >= 0; i--)
            ReturnDoorToPool(activeDoors[i]);

        activeDoors.Clear();
    }

    public void ActivateAllExitDoors()
    {
        foreach (ExitDoor door in activeDoors)
            door.ActivateExitDoor();
    }

    public void DeactivateAllExitDoors()
    {
        foreach (ExitDoor door in activeDoors)
            door.DeactivateExitDoor();
    }

    void SpawnSingleExitDoor()
    {
        ExitDoorSpawnPoint chosen = GetValidSpawnPoint();
        if (chosen == null)
        {
            Debug.LogWarning("No valid exit door spawn point found.");
            return;
        }

        GameObject obj = ObjectPooler.instance.Get(exitDoorTag, chosen.transform.position, chosen.transform.rotation);
        if (obj == null) return;

        ExitDoor door = obj.GetComponent<ExitDoor>();
        if (door != null)
            activeDoors.Add(door);
    }

    void ReturnDoorToPool(ExitDoor door)
    {
        activeDoors.Remove(door);
        ObjectPooler.instance.ReturnToPool(exitDoorTag, door.gameObject);
    }

    ExitDoorSpawnPoint GetValidSpawnPoint()
    {
        List<ExitDoorSpawnPoint> valid = new List<ExitDoorSpawnPoint>(spawnPoints);

        // filter out points too close to already spawned doors
        foreach (ExitDoor door in activeDoors)
        {
            valid.RemoveAll(sp =>
                Vector3.Distance(sp.transform.position, door.transform.position) < minDistanceBetweenDoors);
        }

        if (valid.Count > 0)
            return valid[Random.Range(0, valid.Count)];

        // fallback - pick a random point ignoring distance
        Debug.Log("No valid exit door spawn point. Picking random.");
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
}