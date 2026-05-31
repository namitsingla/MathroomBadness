using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class UIIAController : BaseEnemy
{
    [Header("UIIA Specific")]
    public float uiiaBaseSpeed = 10f;
    public float wallLifetime = 60f;
    private Transform currentWallTarget;
    private LockableWall targetWall;
    public AudioSource UiiaAudioSource;

    protected override void Awake()
    {
        base.Awake();
        catchType = CatchType.uiiacat;
    }

    void OnEnable()
    {
        baseSpeed = uiiaBaseSpeed;
        agent.speed = baseSpeed;
    }

    public override void Enrage()
    {
        if (isEnraged) return;
        isEnraged = true;
        uiiaBaseSpeed *= 1.3f;
        baseSpeed = uiiaBaseSpeed;
        agent.speed = uiiaBaseSpeed;
        wallLifetime *= 2f;
    }

    public override void UnEnrage()
    {
        isEnraged = false;
        uiiaBaseSpeed /= 1.3f;
        baseSpeed = uiiaBaseSpeed;
        agent.speed = uiiaBaseSpeed;
        wallLifetime /= 2f;
    }

    public override void ResetEnemy()
    {
        base.ResetEnemy();
        if (isEnraged) UnEnrage();
        baseSpeed = uiiaBaseSpeed;
        agent.speed = baseSpeed;
        if (targetWall != null)
        {
            WallManager.instance.ReleaseWall(targetWall);
            targetWall = null;
        }
        StopAllCoroutines();
        ChooseRandomWall();
    }

    protected override void HandleMovement()
    {
        if (CanSeePlayer() || isEnraged)
        {
            if (targetWall != null)
            {
                WallManager.instance.ReleaseWall(targetWall);
                targetWall = null;
            }
            agent.SetDestination(target.position);
            return;
        }

        if (!agent.pathPending && (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance))
        {
            if (targetWall != null)
            {
                WallManager.instance.ActivateWall(targetWall, this);
                WallManager.instance.StartWallRemoveTimer(targetWall, wallLifetime);
                targetWall = null;
            }
            ChooseRandomWall();
        }
    }

    void ChooseRandomWall()
    {
        LockableWall wall = WallManager.instance.GetAvailableWall(this);
        if (wall == null) return;
        targetWall = wall;
        currentWallTarget = wall.transform.parent;
        agent.SetDestination(currentWallTarget.position);
    }
}