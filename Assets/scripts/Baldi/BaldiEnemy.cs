using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BaldiEnemy : BaseEnemy
{
    [Header("Baldi Specific")]
    public float speedIncrease = 15f;
    public float baldiBaseSpeed = 70f;
    BaldiLookAnimation baldiLookAnimation;
    DialogueSoundManager dialogueSoundManager;
    private static bool firstDetection = false;
    private float timer;
    public float pauseDuration = 2f;
    public float moveDuration = 1f;
    private bool isMoving = true;

    protected override void Awake()
    {
        base.Awake();
        catchType = CatchType.baldi;
        timer = moveDuration;
        dialogueSoundManager = ReferencesManager.instance.dialogueSoundManager;
        baldiLookAnimation = ReferencesManager.instance.baldiLookAnimation;
    }

    void OnEnable()
    {
        // read base speed fresh each time it's spawned from pool
        baseSpeed = baldiBaseSpeed;
        agent.speed = baseSpeed;
    }

    protected override void Update()
    {
        if (isStunned)
        {
            agent.isStopped = true;
            return;
        }
        HandleMovement();
    }

    protected override void HandleMovement()
    {
        Vector3 lookDirection = target.position - transform.position;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(lookDirection);

        // enraged = skip stop/start, always chase
        if (isEnraged)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            return;
        }

        timer -= Time.deltaTime;

        if (isMoving && timer <= 0f)
        {
            agent.isStopped = true;
            isMoving = false;
        }

        if (!isMoving) return;

        if (CanSeePlayer())
        {
            if (!firstDetection && Vector3.Distance(target.position, transform.position) <= 60f)
            {
                firstDetection = true;
                StartCoroutine(dialogueSoundManager.PlayBaldiFirstDetection());
            }
            agent.SetDestination(target.position);
        }
        else if (!agent.hasPath || agent.remainingDistance < 1f)
        {
            GoToRandomPatrolPoint();
            baldiLookAnimation.baldiLook();
        }
    }

    public override void Enrage()
    {
        if (isEnraged) return;
        isEnraged = true;
        baldiBaseSpeed *= 1.2f;
        baseSpeed = baldiBaseSpeed;
        agent.speed = baldiBaseSpeed;
        lookRadius = 1000f;
        agent.isStopped = false;
        isMoving = true;
    }

    public override void UnEnrage()
    {
        isEnraged = false;
        baldiBaseSpeed /= 1.2f;
        baseSpeed = baldiBaseSpeed;
        agent.speed = baldiBaseSpeed;
        lookRadius = 60f;
    }

    public void ResumeMoving()
    {
        agent.isStopped = false;
        isMoving = true;
        timer = moveDuration;
    }

    public override void ResetEnemy()
    {
        base.ResetEnemy();
        if (isEnraged) UnEnrage();
        isMoving = true;
        timer = moveDuration;
        baseSpeed = baldiBaseSpeed;
        agent.speed = baseSpeed;
    }

    public void UpdateSpeed(int collectedCount)
    {
        agent.speed = baldiBaseSpeed + speedIncrease * collectedCount;
    }
}