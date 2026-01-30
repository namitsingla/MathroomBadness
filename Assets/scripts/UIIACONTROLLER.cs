using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class UIIAController : MonoBehaviour
{
    public float lookRadius = 40f;

    public Transform target;
    NavMeshAgent agent;
    public GameManager targetScript;
    public bool isEnraged = false;
    public bool hasSwitched = false;
    public UIIAmusicmanager MusicManager;

    //for patrolling
    public Transform[] wallPoints;
    private Transform currentTarget;
    private LockableWall targetWall;
    //private bool chasingPlayer = false;
    public float wallLifetime = 60f;
    //for death screen
    public CatchType catchType = CatchType.uiiacat;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        //Debug.Log($"HasPath: {agent.hasPath}, Pending: {agent.pathPending}, Dist: {agent.remainingDistance}");

        //checking if within range
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= lookRadius || isEnraged)
        {
            Vector3 direction = (target.position - transform.position).normalized;

            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, lookRadius) || isEnraged)
            {
                if (hit.collider.CompareTag("Player") || isEnraged)
                {
                    agent.SetDestination(target.position); //sets target on player
                    //Debug.Log("Player detected!");
                    //chasingPlayer = true;

                    //to remove wall so it doesnt activate wall even if near the target
                    targetWall = null;
                    //Debug.Log("wall removed from target.");

                    if (!hasSwitched)
                    {
                        //switches song when player gets in range
                        MusicManager.PlayRandomSong();
                        hasSwitched = true;
                    }
                    return;
                }
            }
        }

        //if cat lost the player
        //chasingPlayer = false;
        hasSwitched = false;

        if (!agent.pathPending && (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance))
        {
             if (targetWall != null)
            {
                targetWall.ActivateWall();
                Debug.Log("wall activated");

                //to deactivate the wall
                RemoveWall(targetWall);
            }

            ChooseRandomWall();
    }

        // if (agent.remainingDistance < 1f)
        // {
        //     Debug.Log("no path");
        // }
    }
    private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        targetScript.KhelKhatam(transform, catchType);
    }
}

void ChooseRandomWall()
    {
        if (wallPoints.Length == 0) return;

        do 
        {
            currentTarget = wallPoints[Random.Range(0, wallPoints.Length)];
            agent.SetDestination(currentTarget.position);
            //Debug.Log("target chosen");

            targetWall = currentTarget.GetComponentInChildren<LockableWall>(true);
            Debug.Log("wall chosen");
        } while (CheckIfAvailableWalls() && targetWall.gameObject.activeSelf);

         return;
    }

public bool CheckIfAvailableWalls()
    {
        for (int i = 0; i < wallPoints.Length; i++)
        {
            if (!targetWall.gameObject.activeSelf) return true;
        }

        return false;
    }

    public void ActivateAllWalls()
    {
        for (int i = 0; i < wallPoints.Length; i++) 
        {
            wallPoints[i].GetComponentInChildren<LockableWall>(true).ActivateWall();
        }
        
        Debug.Log("UIIAAAAA");
    }

    public void RemoveWall(LockableWall wallToBeRemoved) 
    {
       StartCoroutine(RemoveWallAfterDelay(wallToBeRemoved));
    }
    IEnumerator RemoveWallAfterDelay(LockableWall wallToBeRemoved)
    {
        yield return new WaitForSeconds(wallLifetime);
        wallToBeRemoved.DeactivateWall();
        Debug.Log("Wall removed.");
    }
}
