using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;


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
    public float baldiBaseSpeed = 70f;
    public float speedIncrease = 15f;
    
    //for patrolling
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    //for baldi look animation
    public BaldiLookAnimation baldiLookAnimation;
    //for death
    public CatchType catchType = CatchType.baldi;
    //for first detection audio clip
    private bool firstDetection = false;
    public DialogueSoundManager dialogueSoundManager;
    public PowerSystem powerSystem;
    void Start()
    {
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
                   if (!firstDetection && distance <= 60f)
                    {
                        firstDetection = true;
                        StartCoroutine(dialogueSoundManager.PlayBaldiFirstDetection());
                        Debug.Log("Player detected.");
                    }

                    //set target
                    if (target != null)
                        agent.SetDestination(target.position);

                } 
            }
        }
        //to set agent to patrolling
        if (!isEnraged && !agent.hasPath || agent.remainingDistance < 1f)
        {
            GoToRandomPoint();
        }
    }

void GoToRandomPoint()
    {

        currentPatrolIndex = Random.Range(0, patrolPoints.Length);
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        //Debug.Log("Set a new patrol point for Baldi");
        baldiLookAnimation.baldiLook();
    }

public void ResumeMoving()
{
    //resume moving
    agent.isStopped = false;
    isMoving = true;
    timer = moveDuration;
}

private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (powerSystem.isPowerDotOn)
            {
                powerSystem.StartCoroutine(powerSystem.PowerDotTeleport(agent));
                return;
            }

            targetScript.KhelKhatam(transform, catchType);
        }
    }
}