using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ExitDoorSpawner : MonoBehaviour
{
    public Transform exitDoor;
    public Transform[] spawnPoints;

    void Start()
    {
        SpawnExitDoor();
    }

    public void SpawnExitDoor()
    {
        int index = Random.Range(0, spawnPoints.Length);

        exitDoor.position = spawnPoints[index].position;
        exitDoor.rotation = spawnPoints[index].rotation;
    }

    // void Update()
    // {
    //     if (Input.GetKey(KeyCode.Space)) SpawnExitDoor();
    // }
}
