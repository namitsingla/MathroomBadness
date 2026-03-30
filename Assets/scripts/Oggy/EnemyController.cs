using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float lookRadius = 60f;

    public Transform target;
    NavMeshAgent agent;
    public GameManager targetScript;
    public bool isEnraged = false;
    public CatchType catchType = CatchType.oggy;
    public float oggyBAseSpeed = 4f;
    public bool isStunned = false;

    // for patrolling
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;
    public PowerSystem powerSystem;
    public Animator animator;
    public float baseAnimationSpeed = 2f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        // speed oggy up if he is afar
        if (isStunned) agent.speed = 0f;
         else {agent.speed = oggyBAseSpeed *  (10 + distance)/50; }

        animator.SetFloat("MotionSpeed", agent.speed/baseAnimationSpeed);

        // checking if within range
        if (distance <= lookRadius || isEnraged)
        {
             Vector3 direction = (target.position - transform.position).normalized;

           if (Physics.Raycast(transform.position, direction, out RaycastHit hit, lookRadius) || isEnraged)
            {
                if (hit.collider.CompareTag("Player") || isEnraged)
                {
                    agent.SetDestination(target.position); //sets target on player
                    //Debug.Log("Player detected!");
                }
            }

            /*if (distance <= agent.stoppingDistance)
                {
                    //so enemy faces the player when attacking
                    FaceTarget();
                }*/
        }

        //to set agent to patrolling
        if (!isEnraged && !agent.hasPath || agent.remainingDistance < 1f)
        {
            GoToRandomPoint();
        }
    }

    void FaceTarget ()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void GoToRandomPoint()
    {
        currentPatrolIndex = Random.Range(0, patrolPoints.Length);
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        //Debug.Log("Set a new patrol point for Baldi");
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
