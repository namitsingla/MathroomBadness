using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallManager : MonoBehaviour
{
    public static WallManager instance;

    // tracks which UIIA has claimed which wall
    private Dictionary<LockableWall, UIIAController> claimedWalls = new Dictionary<LockableWall, UIIAController>();

    void Awake()
    {
        instance = this;
    }

    // claim a wall for a specific UIIA - returns false if already claimed by another
    public bool ClaimWall(LockableWall wall, UIIAController claimant)
    {
        if (claimedWalls.ContainsKey(wall) && claimedWalls[wall] != claimant)
            return false;

        claimedWalls[wall] = claimant;
        return true;
    }

    public void ReleaseWall(LockableWall wall)
    {
        claimedWalls.Remove(wall);
    }

    public bool IsWallClaimed(LockableWall wall)
    {
        return claimedWalls.ContainsKey(wall);
    }

    public void ActivateWall(LockableWall wall, UIIAController caller)
    {
        wall.gameObject.SetActive(true);
    }

    public void DeactivateWall(LockableWall wall)
    {
        ReleaseWall(wall);
        wall.gameObject.SetActive(false);
    }

    public void DeactivateAllWalls()
    {
        foreach (var wp in NavigationPointsManager.instance.wallPoints)
        {
            LockableWall wall = wp.GetComponentInChildren<LockableWall>(true);
            if (wall != null) DeactivateWall(wall);
        }
    }

    public void StartWallRemoveTimer(LockableWall wall, float lifetime)
    {
        StartCoroutine(RemoveWallAfterDelay(wall, lifetime));
    }

    IEnumerator RemoveWallAfterDelay(LockableWall wall, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        if (wall != null) DeactivateWall(wall);
    }

    // get a random unclaimed, inactive wall - returns null if none available
    public LockableWall GetAvailableWall(UIIAController requester)
    {
        Transform[] points = NavigationPointsManager.instance.wallPoints;
        if (points == null || points.Length == 0) return null;

        // shuffle to avoid bias
        List<Transform> shuffled = new List<Transform>(points);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }

        foreach (Transform wp in shuffled)
        {
            LockableWall wall = wp.GetComponentInChildren<LockableWall>(true);
            if (wall == null) continue;
            if (wall.gameObject.activeSelf) continue;
            if (IsWallClaimed(wall)) continue;

            ClaimWall(wall, requester);
            return wall;
        }

        return null;
    }
}