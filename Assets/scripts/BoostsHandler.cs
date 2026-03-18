using UnityEngine;
using System;
using System.Collections.Generic;

public class BoostsHandler : MonoBehaviour
{
    public player_controller player_Controller;
    public collectedisplay collectedisplay;
    public Camera mainCamera;
    public GameObject Minimap;
    public BaldiEnemy baldiEnemy;
    public UnityEngine.AI.NavMeshAgent baldiNavMesh;
    public UnityEngine.AI.NavMeshAgent uiiaNavMesh;
    public EnemyController enemyController;
    public UnityEngine.AI.NavMeshAgent oggyNavMesh;
    public PowerSystem powerSystem;
    public bool isRadarOn = false;
    public GameManager gameManager;
    public bool isWallRunner = false;
    public bool ifRandomPowerUpEachRound = false;
    public GameObject MiniMapUI;
    public SpawnManager spawnManager;


    List<Action> powerUps = new List<Action>();

    void Start()
    {
        powerUps.Add(InvincibilityShield);
        powerUps.Add(Teleporter);
        powerUps.Add(PowerDot);
    }
    public void SpeedIncrease()
    {
        player_Controller.moveSpeed *= 1.20f;
        mainCamera.fieldOfView += 10f;
    }

    public void MultiplierIncrease()
    {
        collectedisplay.mult += 0.15f;
    }

    public void VisionIncrease()
    {
        if (RenderSettings.fogDensity <= 0.015f) RenderSettings.fogDensity = 0f;
         else RenderSettings.fogDensity -= 0.015f;
    }

    public void MinimapIncrease()
    {
        Minimap.transform.position += new Vector3(0f, 150f, 0f);
    }

    public void SlowDownEnemies()
    {
        baldiEnemy.baldiBaseSpeed *= 0.75f;
        baldiNavMesh.speed = baldiEnemy.baldiBaseSpeed;
        uiiaNavMesh.speed *= 0.75f;
        enemyController.oggyBAseSpeed *= 0.75f;
        oggyNavMesh.speed = enemyController.oggyBAseSpeed;
    }

    public void InvincibilityShield()
    {
        powerSystem.currentPower.AssignPower(powerSystem.invincibilityShield);
    }

    public void Teleporter()
    {
        powerSystem.currentPower.AssignPower(powerSystem.teleporter);
    }

    public void Stunner()
    {
        powerSystem.currentPower.AssignPower(powerSystem.stunner);
    }

    public void RagebaitBaldi()
    {
        if (baldiEnemy.isEnraged)
        {
            baldiEnemy.baldiBaseSpeed *= 1.2f;
        }

        baldiEnemy.isEnraged = true;
        baldiEnemy.lookRadius = 1000f;
        baldiEnemy.baldiBaseSpeed *= 1.1f;
        collectedisplay.mult += 1f;
    }

    public void Radar()
    {
        isRadarOn = true;
    }

    public void PowerDot()
    {
        powerSystem.currentPower.AssignPower(powerSystem.powerDot);
    }

    public void WallBreaker()
    {
        powerSystem.currentPower.AssignPower(powerSystem.wallBreaker);
    }

    public void AnExtraLife()
    {
        gameManager.lives += 1;
    }

    public void WallRunner()
    {
        isWallRunner = true;
    }

    public void RandomPowerUp()
    {
        ifRandomPowerUpEachRound = true;
    }

    public void MiniMapEater()
    {
        Minimap.SetActive(false);
        MiniMapUI.SetActive(false);
        collectedisplay.mult += 0.65f;
        player_Controller.moveSpeed *= 1.3f;
    }

    public void GetRandomPowerUp()
    {
        if (powerSystem.currentPower == null || powerSystem.currentPower.Effect == powerSystem.deEquipPower.Effect)
        {
            Action chosen = powerUps[UnityEngine.Random.Range(0, powerUps.Count)];
            chosen.Invoke(); 
            //Debug.Log("signal was recieved");   
        }
    }

    public void TimeSpeedUp()
    {
        gameManager.gameSpeed *= 1.25f;
        collectedisplay.mult += 1.4f;
    }

    public void IncreaseItemSpawnRate()
    {
        spawnManager.spawnCount += 1;
    }

    public void PrisonRealm()
    {
        powerSystem.currentPower.AssignPower(powerSystem.prisonRealm);
    }
}
