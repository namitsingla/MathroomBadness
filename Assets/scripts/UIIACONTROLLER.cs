using UnityEngine;
using UnityEngine.AI;

public class UIIAController : MonoBehaviour
{
    public float lookRadius = 40f;

    Transform target;
    NavMeshAgent agent;
    public GameManager targetScript;
    public bool isEnraged = false;
    public bool hasSwitched = false;
    public UIIAmusicmanager MusicManager;

    //for patrolling
    public Transform[] wallPoints;
    private Transform currentTarget;
    private LockableWall targetWall;
    private bool chasingPlayer = false;

    void Start()
    {
        target = PlayerManager.instance.player.transform;
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
                    chasingPlayer = true;

                    //to remove wall so it doesnt activate wall even if near the target
                    targetWall = null;
                    Debug.Log("wall removed.");

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
        chasingPlayer = false;
        hasSwitched = false;

        if (!agent.pathPending && (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance))
        {
             if (targetWall != null)
            {
                targetWall.ActivateWall();
                Debug.Log("wall activated");
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
        targetScript.KhelKhatam();
    }
}

void ChooseRandomWall()
    {
        if (wallPoints.Length == 0) return;

        do 
        {
            currentTarget = wallPoints[Random.Range(0, wallPoints.Length)];
            agent.SetDestination(currentTarget.position);
            Debug.Log("target chosen");

            targetWall = currentTarget.GetComponentInChildren<LockableWall>(true);
            Debug.Log("wall chosen");
        } while (targetWall.gameObject.activeSelf);

         return;
    }

public void ActivateAllWalls()
    {
        for (int i = 0; i < wallPoints.Length; i++)
        {
            wallPoints[i].GetComponentInChildren<LockableWall>(true).ActivateWall();
        }
    }
}
