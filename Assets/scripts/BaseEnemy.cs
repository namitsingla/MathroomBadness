using UnityEngine;
using UnityEngine.AI;

public abstract class BaseEnemy : MonoBehaviour
{
    [Header("Core")]
    public NavMeshAgent agent;
    public Transform target;
    GameManager gameManager;
    PowerSystem powerSystem;
    public CatchType catchType;

    [Header("Stats")]
    public float baseSpeed = 10f;
    public float lookRadius = 60f;
    public bool isEnraged = false;
    public bool isStunned = false;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        target = ReferencesManager.instance.player;
        gameManager = ReferencesManager.instance.gameManager;
        powerSystem = ReferencesManager.instance.powerSystem;
    }

    protected virtual void Update()
    {
        if (isStunned)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false; // ADD THIS BACK - explicitly clear it every frame when not stunned
        HandleMovement();
    }

    // Each enemy implements their own movement logic
    protected abstract void HandleMovement();

    // Shared patrol logic all enemies can use
    protected void GoToRandomPatrolPoint()
    {
        Transform[] points = NavigationPointsManager.instance.patrolPoints;
        if (points.Length == 0) return;
        int index = Random.Range(0, points.Length);
        agent.SetDestination(points[index].position);
    }

    // Shared player detection
    protected bool CanSeePlayer()
    {
        float distance = Vector3.Distance(target.position, transform.position);
        if (distance > lookRadius && !isEnraged) return false;

        Vector3 direction = (target.position - transform.position).normalized;
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, lookRadius))
            return hit.collider.CompareTag("Player");

        return isEnraged;
    }

    // Shared speed modifier - call this to apply boosts/slow downs
    public void SetSpeedMultiplier(float multiplier)
    {
        agent.speed *= multiplier;
    }

    public void Stun(float duration)
    {
        StartCoroutine(StunCoroutine(duration));
    }

    private System.Collections.IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }

    // Reset for pooling / round restart
    public virtual void ResetEnemy()
    {
        isStunned = false;
        isEnraged = false;
        agent.ResetPath();
        agent.speed = baseSpeed;
        agent.isStopped = false;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (powerSystem.isPowerDotOn)
        {
            if (powerSystem.isSealingInProgress) return;
            powerSystem.StartCoroutine(powerSystem.PowerDotTeleport(this));
            return;
        }

        gameManager.KhelKhatam(this, catchType);
    }

    void OnEnable()
    {
        if (EnemyManager.instance != null)
            EnemyManager.instance.RegisterEnemy(this);

        if (DifficultyManager.instance != null)
            DifficultyManager.instance.ApplyDifficultyToEnemy(this);
    }

    void OnDisable()
    {
        if (EnemyManager.instance != null)
            EnemyManager.instance.UnregisterEnemy(this);
    }
    public void WarpTo(Vector3 position)
    {
        agent.Warp(position);
        agent.ResetPath();
        isStunned = false;
        agent.isStopped = false;
    }

    public virtual void Enrage()
    {
        isEnraged = true;
    }

    public virtual void UnEnrage()
    {
        isEnraged = false;
    }
}