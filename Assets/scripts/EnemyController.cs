using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float lookRadius = 20f;

    Transform target;
    NavMeshAgent agent;
    public GameManager targetScript;
    public bool isEnraged = false;
    public CatchType catchType = CatchType.oggy;

    void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
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
                }
            }

            /*if (distance <= agent.stoppingDistance)
                {
                    //so enemy faces the player when attacking
                    FaceTarget();
                }*/
        }
    }

    void FaceTarget ()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
    targetScript.KhelKhatam(transform, catchType);
    }
}
}
