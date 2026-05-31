using UnityEngine;

public class NavigationPointsManager : MonoBehaviour
{
    public static NavigationPointsManager instance;

    [Header("Patrol Points")]
    public Transform[] patrolPoints;

    [Header("Wall Points")]
    public Transform[] wallPoints;

    void Awake()
    {
        instance = this;
    }

    // Call this after procedurally generating the map to update all points
    public void CacheAllPoints()
    {
        CachePatrolPoints();
        CacheWallPoints();
    }

    public void CachePatrolPoints()
    {
        GameObject[] found = GameObject.FindGameObjectsWithTag("PatrolPoint");
        patrolPoints = new Transform[found.Length];
        for (int i = 0; i < found.Length; i++)
            patrolPoints[i] = found[i].transform;

        Debug.Log("Cached " + patrolPoints.Length + " patrol points.");
    }

    public void CacheWallPoints()
    {
        GameObject[] found = GameObject.FindGameObjectsWithTag("WallPoint");
        wallPoints = new Transform[found.Length];
        for (int i = 0; i < found.Length; i++)
            wallPoints[i] = found[i].transform;

        Debug.Log("Cached " + wallPoints.Length + " wall points.");
    }
}