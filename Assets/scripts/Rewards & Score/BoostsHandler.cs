using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;
using Unity.VisualScripting;

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
    public RarityManager rarityManager;
    int playerLayer;
    int uiiaWallsLayer;
    public bool isMitosisOn = false;
    public int mitosisMultiplier = 2;
    public ExitDoor exitDoor;
    public bool isPassiveMagnetOn = false;
    public ParticleSystem sandBurst;
    public AudioSource sandHitSoundSource;
    public AudioClip sandHitSound;
    public bool isShukakuActive = false;
    public Image shukakuSprite;
    public Image shukakuRechargeCirle;
    public bool isShukakuRecharging = false;
    private float shukakuCooldown = 0f;


    List<Action> powerUps = new List<Action>();

    void Start()
    {
        powerUps.Add(InvincibilityShield);
        powerUps.Add(Teleporter);
        powerUps.Add(PowerDot);

        playerLayer = LayerMask.NameToLayer("Player");
        uiiaWallsLayer = LayerMask.NameToLayer("UIIA Walls");

        //to turn off schrodingerscat when new game starts
        PassThroughWalls(false);

        //to turn off aura farming when new game starts
        Shader.SetGlobalFloat("_GlobalXrayToggle", 0f); 
    }
    public void SpeedIncrease()
    {
        player_Controller.moveSpeed *= 1.20f;
        mainCamera.fieldOfView += 10f;
    }

    public void MultiplierIncrease()
    {
        collectedisplay.mult += 0.25f;
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
        baldiEnemy.baldiBaseSpeed *= 0.70f;
        baldiNavMesh.speed = baldiEnemy.baldiBaseSpeed;
        uiiaNavMesh.speed *= 0.70f;
        enemyController.oggyBAseSpeed *= 0.70f;
        oggyNavMesh.speed = enemyController.oggyBAseSpeed;
    }

    public void InvincibilityShield()
    {
        powerSystem.EquipPower(powerSystem.invincibilityShield);
    }

    public void Teleporter()
    {
        powerSystem.EquipPower(powerSystem.teleporter);
    }

    public void Stunner()
    {
        powerSystem.EquipPower(powerSystem.stunner);
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
        powerSystem.EquipPower(powerSystem.powerDot);
    }

    public void WallBreaker()
    {
        powerSystem.EquipPower(powerSystem.wallBreaker);
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
        if (powerSystem.currentPower == null || powerSystem.currentPower.Effect == powerSystem.deEquipPower.Effect || powerSystem.isRecharging || powerSystem.isPowerActive)
        {
            Action chosen = powerUps[UnityEngine.Random.Range(0, powerUps.Count)];
            chosen.Invoke(); 
            //Debug.Log("signal was recieved");   
        }
    }

    public void TimeSpeedUp()
    {
        gameManager.gameSpeed *= 1.4f;
        collectedisplay.mult += 1.4f;
    }

    public void IncreaseItemSpawnRate()
    {
        spawnManager.spawnCount += 2;
    }

    public void PrisonRealm()
    {
        powerSystem.EquipPower(powerSystem.prisonRealm);
    }

    public void TheFourthWall()
    {
        rarityManager.rewardCount += 1;

        for (int i = 0; i < 3; i++)
        {
            rarityManager.rewardButtons[i].GetComponent<RectTransform>().anchoredPosition += new Vector2( -80f ,0f);
        }

        rarityManager.rewardButtons[3].gameObject.SetActive(true);
    }

    public void CornerCutter()
    {
        player_Controller.diagonalBoostMultiplier *= 1.5f;
    }

    public void SchrodingersCat()
    {
        PassThroughWalls(true);
    }

    public void PassThroughWalls(bool state)
    {
        Physics.IgnoreLayerCollision(playerLayer, uiiaWallsLayer, state);
    }

    public void Mitosis()
    {
        isMitosisOn = true;
    }

    public void Meiosis()
    {
        mitosisMultiplier = 4;
    }

    public void HighStakes()
    {
        exitDoor.requiredItems *= 2;;
        collectedisplay.mult += 2f;
    }

    public void AuraFarming()
    {
        Shader.SetGlobalFloat("_GlobalXrayToggle", 1f);
    }

    public void TunnelVision()
    {
        rarityManager.rewardCount = 1;

        for (int i = 1; i < 4; i++)
        {
            rarityManager.rewardButtons[i].gameObject.SetActive(false);
        }

        rarityManager.rewardButtons[0].GetComponent<RectTransform>().anchoredPosition = new Vector2( 0f ,0f);

        // removing common and rare items from selection
        rarityManager.initialWeights[0] = 0f;
        rarityManager.initialWeights[1] = 0f;
        rarityManager.perItemIncre[1] = 0f;
        rarityManager.perRoundIncre[1] = 0f;

        // increasing luck
        rarityManager.IncreaseLuckBy(5);
    }

    public void LoadedDice()
    {
        rarityManager.IncreaseLuckBy(2);
    }

    public void PassiveMagnet()
    {
        isPassiveMagnetOn = true;
    }

    public void ShukakuTurnOn()
    {
        isShukakuActive = true;
        shukakuSprite.enabled = true;

        player_Controller.moveSpeed *= 0.8f;
        mainCamera.fieldOfView -= 10f;

    }

    public IEnumerator ShukakuProtection(CatchType type)
    {
        gameManager.isDead = true;
        powerSystem.StunHitEnemy(type, 2.5f);
        PlaySandEffect();
        isShukakuActive = false;
        isShukakuRecharging = true;
        shukakuSprite.enabled = false;

        yield return new WaitForSeconds(1f);

        gameManager.isDead = false;
    }
    public void PlaySandEffect()
    {
        sandBurst.Play();

        // This tricks the brain into thinking it's a new sound every time!
        sandHitSoundSource.pitch = 1f + UnityEngine.Random.Range(-0.15f, 0.15f);
        
        // PlayOneShot lets multiple hits overlap without cutting off
        sandHitSoundSource.PlayOneShot(sandHitSound);
    }

    void Update()
    {
        if (isShukakuRecharging)
        {
            shukakuCooldown += Time.deltaTime;
            
            // Fills the circle back up over time (0.0 to 1.0)
            shukakuRechargeCirle.fillAmount = 1 - shukakuCooldown / 5f;

            if (shukakuCooldown >= 5f)
            {
                shukakuCooldown = 0f;
                isShukakuRecharging = false;
                isShukakuActive = true;
                shukakuSprite.enabled = true;
                Debug.Log("Recharge Complete!");
            }
        }
    }
}
