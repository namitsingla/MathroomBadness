using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BaldiEnemy : MonoBehaviour
{
    public float lookRadius = 150f;

    public NavMeshAgent agent;
    public Transform target; 
    public GameManager targetScript;

    public float moveDuration = 1f; 
    public float pauseDuration = 2f;

     private bool isMoving = true;
    private float timer;
    public bool isEnraged = false;
    
    //for patrolling
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    //for baldi look animation
    public BaldiLookAnimation baldiLookAnimation;
    void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        timer = moveDuration;
    }

    void Update()
    {
        // face the player at all times
        Vector3 lookDirection = target.position - transform.position;
        lookDirection.y = 0; // ignore vertical tilt
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }

        //checking if within range
        float distance = Vector3.Distance(target.position, transform.position);
        timer -= Time.deltaTime;
        
        if (isMoving)
                    {
                        if (timer <= 0f)
                        {
                            // stop moving
                            agent.isStopped = true;
                            isMoving = false;
                            timer = pauseDuration;
                        }
                    }
                    // else
                    // {
                    //     if (timer <= 0f)
                    //     {
                    //         // resume moving
                    //         agent.isStopped = false;
                    //         isMoving = true;
                    //         timer = moveDuration;
                    //     }
                    // }

        if (distance <= lookRadius || isEnraged)
        {
            Vector3 direction = (target.position - transform.position).normalized;

            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, lookRadius) || isEnraged)
            {
                if (hit.collider.CompareTag("Player") || isEnraged)
                {
                   // Debug.Log("Player detected!");

                    //set target
                    if (target != null)
                        agent.SetDestination(target.position);

                } 
            }
        }
        //to set agent to patrolling
        if (!agent.hasPath || agent.remainingDistance < 1f)
        {
            GoToRandomPoint();
        }
    }

private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetScript.KhelKhatam();
        }
    }

void GoToRandomPoint()
    {

        currentPatrolIndex = Random.Range(0, patrolPoints.Length);
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        Debug.Log("Set a new patrol point for Baldi");
        baldiLookAnimation.baldiLook();
    }

public void ResumeMoving()
{
    //resume moving
    agent.isStopped = false;
    isMoving = true;
    timer = moveDuration;
}
}