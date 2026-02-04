using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class Power
{
    public Func<IEnumerator> Effect;
    public Action onEquip;

    public void AssignPower(Power other)
    {
        Effect = other.Effect;
        other.onEquip();
    }
}

public class PowerSystem : MonoBehaviour
{
    public Power currentPower;
    public Image currentPowerIcon;
    public GameManager gameManager;
    public UniversalRendererData rendererData;
    public SpawnManager spawnManager; 
    public UnityEngine.AI.NavMeshAgent baldiNavMesh;
    public UnityEngine.AI.NavMeshAgent uiiaNavMesh;
    public EnemyController enemyController;
    public BaldiEnemy baldiEnemy;
    public collectedisplay collectedisplay;
    public AudioSource uiiaCatAudio;
    public AudioSource BGM;
    public UIIAController uIIAController;


    public Power deEquipPower;

    public Power invincibilityShield;
    public Sprite invincibilityShieldIcon;
    public GameObject invincibilityShieldOverlay;
    public Power teleporter;
    public Sprite teleporterIcon;
    public Power stunner;
    public Sprite stunnerIcon;
    public Power powerDot;
    public Sprite powerDotSprite;
    public AudioSource powerDotAudio;
    public bool isPowerDotOn = false;
    public Power wallBreaker;
    public Sprite wallBreakerSprite;

    public Material[] mapMaterials;
    public Shader pacManModeShader;
    Shader urpLit;
    void Awake()
    {
        currentPower = new Power();

        deEquipPower = new Power();
        deEquipPower.Effect = DeEquipPowerEffect;
        deEquipPower.onEquip = DeEquipPowerOnEquip;

        invincibilityShield = new Power();
        invincibilityShield.Effect = InvincibilityShieldEffect;
        invincibilityShield.onEquip = InvincibilityShieldOnEquip;

        teleporter = new Power();
        teleporter.Effect = TeleporterEffect;
        teleporter.onEquip = TeleporterOnEquip;

        stunner = new Power();
        stunner.Effect = StunnerEffect;
        stunner.onEquip = StunnerOnEquip;

        powerDot = new Power();
        powerDot.Effect =  PowerDotEffect;
        powerDot.onEquip = PowerDotOnEquip;
        Shader.SetGlobalFloat("_FrightenedAmount", 0f);
        Shader.SetGlobalFloat("_FrightenedFlash", 0f);
        urpLit = Shader.Find("Universal Render Pipeline/Lit");

        wallBreaker = new Power();
        wallBreaker.Effect = WallBreakerEffect;
        wallBreaker.onEquip = WallBreakerOnEQuip;

        currentPower.AssignPower(powerDot);
    }

    IEnumerator DeEquipPowerEffect()
    {
        yield return null;
    }
    public void DeEquipPowerOnEquip()
    {
        currentPower.Effect = DeEquipPowerEffect;
        currentPowerIcon.enabled = false;
    }


    IEnumerator InvincibilityShieldEffect()
    {
        gameManager.isDead = true;
        SetFullScreenPass(true);
        invincibilityShieldOverlay.SetActive(true);
        Debug.Log("Invincibility activated");

        yield return new WaitForSeconds(7f);
        gameManager.isDead = false;
        SetFullScreenPass(false);
        invincibilityShieldOverlay.SetActive(false);
        Debug.Log("Invincibility ended");
    }

    public void InvincibilityShieldOnEquip()
    {
        currentPowerIcon.enabled = true;
        currentPowerIcon.sprite = invincibilityShieldIcon;
    }

    public void SetFullScreenPass(bool enabled)
    {
        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature is FullScreenPassRendererFeature pass)
            {
                pass.SetActive(enabled);
                return;
            }
        }
    }

    IEnumerator TeleporterEffect()
    {
        spawnManager.TeleportPlayerAway();
        yield return null;
    }

    public void TeleporterOnEquip()
    {
        currentPowerIcon.enabled = true;
        currentPowerIcon.sprite = teleporterIcon;
    }

    IEnumerator StunnerEffect()
    {
        float baldiSpeed = baldiNavMesh.speed;
        baldiNavMesh.speed = 0f;
        float uiiaSpeed = uiiaNavMesh.speed;
        uiiaNavMesh.speed = 0f;
        enemyController.isStunned = true;

        yield return new WaitForSeconds(5f);
        baldiNavMesh.speed = baldiSpeed;
        enemyController.isStunned = false;
        uiiaNavMesh.speed = uiiaSpeed;

    }

    public void StunnerOnEquip()
    {
        currentPowerIcon.enabled = true;
        currentPowerIcon.sprite = stunnerIcon;
    }

    IEnumerator PowerDotEffect()
    {
        isPowerDotOn = true;
        Shader.SetGlobalFloat("_FrightenedAmount", 1f);
        Shader.SetGlobalFloat("_FrightenedFlash", 0f);
        powerDotAudio.Play();
        uiiaCatAudio.Pause();
        BGM.Pause();
        foreach (var mat in mapMaterials)
            mat.shader = pacManModeShader;


        yield return new WaitForSeconds(7f);

         Shader.SetGlobalFloat("_FrightenedFlash", 1f);

        yield return new WaitForSeconds(3f);

         isPowerDotOn = false;
         Shader.SetGlobalFloat("_FrightenedAmount", 0f);
         Shader.SetGlobalFloat("_FrightenedFlash", 0f);
         powerDotAudio.Stop();
         uiiaCatAudio.UnPause();
         BGM.UnPause();
         for (int i = 0; i < mapMaterials.Length; i++)
            mapMaterials[i].shader = urpLit;

    }

    public void PowerDotOnEquip()
    {
        currentPowerIcon.enabled = true;
        currentPowerIcon.sprite = powerDotSprite;
    }

    public IEnumerator PowerDotTeleport(UnityEngine.AI.NavMeshAgent enemy)
    {
        Time.timeScale = 0f;
        powerDotAudio.Pause();

        yield return new WaitForSecondsRealtime(1f);

        spawnManager.SpawnEnemy(enemy);
        Time.timeScale = gameManager.gameSpeed;
        powerDotAudio.UnPause();
    }

    IEnumerator WallBreakerEffect()
    {
        uIIAController.DeactivateAllWalls();
        yield return null;
    }

    public void WallBreakerOnEQuip()
    {
        currentPowerIcon.enabled = true;
        currentPowerIcon.sprite = wallBreakerSprite;
    }

    void Update()
    {
       if ((Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(1)) && currentPower != null)
        {
            // so cant be activated when already dead
            if (gameManager.isDead) return;

            StartCoroutine(currentPower.Effect());
            currentPower.AssignPower(deEquipPower);
        } 
    }
}
