using UnityEngine;
using UnityEngine.AI;

public class EnemyController : BaseEnemy
{
    [Header("Oggy Specific")]
    public Animator animator;
    public float baseAnimationSpeed = 2f;
    public float oggyBaseSpeed = 10f;

    protected override void Awake()
    {
        base.Awake();
        catchType = CatchType.oggy;
    }

    void OnEnable()
    {
        baseSpeed = oggyBaseSpeed;
        agent.speed = baseSpeed;
    }

    public override void Enrage()
    {
        if (isEnraged) return;
        isEnraged = true;
        oggyBaseSpeed *= 1.3f;
        baseSpeed = oggyBaseSpeed;
    }

    public override void UnEnrage()
    {
        isEnraged = false;
        oggyBaseSpeed /= 1.3f;
        baseSpeed = oggyBaseSpeed;
    }

    public override void ResetEnemy()
    {
        base.ResetEnemy();
        if (isEnraged) UnEnrage();
        baseSpeed = oggyBaseSpeed;
        agent.speed = baseSpeed;
    }

    protected override void HandleMovement()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        // distance scaling: closer = slower, farther = faster, capped sensibly
        float distanceMultiplier = (10f + distance) / 50f;
        agent.speed = isStunned ? 0f : baseSpeed * distanceMultiplier;
        animator.SetFloat("MotionSpeed", agent.speed / baseAnimationSpeed);

        if (CanSeePlayer() || isEnraged)
            agent.SetDestination(target.position);
        else if (!agent.hasPath || agent.remainingDistance < 1f)
            GoToRandomPatrolPoint();
    }
}