using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ExitDoorSpawner : MonoBehaviour
{
    public Transform exitDoor;
    public ExitDoorSpawnPoint[] spawnPoints;
    public ExitDoorSpawnPoint firstSpawnPoint;

    void Start()
    {
        SpawnFirstExitDoor();
    }

    public void SpawnExitDoor()
    {
        int index = Random.Range(0, spawnPoints.Length);

        exitDoor.position = spawnPoints[index].transform.position;
        exitDoor.rotation = spawnPoints[index].transform.rotation;
    }

    public void SpawnFirstExitDoor()
    {
        exitDoor.position = firstSpawnPoint.transform.position;
        exitDoor.rotation = firstSpawnPoint.transform.rotation;
    }

    // void Update()
    // {
    //     if (Input.GetKey(KeyCode.Space)) SpawnExitDoor();
    // }

    // This is to assign spawn points automatically from inspector
    #if UNITY_EDITOR
    [ContextMenu("Auto Assign Exit Door Spawn Points")]
    void AutoAssign()
    {
        spawnPoints = FindObjectsOfType<ExitDoorSpawnPoint>();
    }
    #endif
}
