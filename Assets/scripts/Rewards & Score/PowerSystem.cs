using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

[System.Serializable]
public class Power
{
    public Func<IEnumerator> Effect;
    public Action onEquip;
    public float duration = 0f;
    public float rechargeTime = 0f;
    public bool isReusable = false;

    public void AssignPower(Power other)
    {
        Effect = other.Effect;
        other.onEquip();
        duration = other.duration;
        rechargeTime = other.rechargeTime;
        isReusable = other.isReusable;
    }
}

public class PrisonData
{
    public GameObject Enemy;
    public int ReleaseRound;

    public PrisonData(GameObject enemy, int round)
    {
        Enemy = enemy;
        ReleaseRound = round;
    }
}

public class PowerSystem : MonoBehaviour
{
    [Header("Powers")]
    public Power currentPower;
    public Power deEquipPower;
    public Power invincibilityShield;
    public Power teleporter;
    public Power stunner;
    public Power powerDot;
    public Power wallBreaker;
    public Power prisonRealm;


    [Header("Sprites")]
    public Image currentPowerIcon;
    public Sprite invincibilityShieldIcon;
    public Sprite teleporterIcon;
    public Sprite stunnerIcon;
    public Sprite powerDotSprite;
    public Sprite wallBreakerSprite;
    public Sprite prisonRealmSprite;


    [Header("Sound Effects")]
    public AudioSource powerDotAudio;
    public AudioSource invincibilityShieldAudio;
    public AudioSource teleporterAudio;
    public AudioSource stunnerAudio;
    public AudioSource prisonRealmHeartbeat;
    public AudioSource prisonRealmCapturedSound;


    [Header("Power Specific Refrences")]
    public GameObject invincibilityShieldOverlay;
    public Shader pacManModeShader;
    public Shader prisonRealmShader;
    public GameObject priosnRealmCaughtImage;

    [Header("Variables")]
    public bool isPowerDotOn = false;
    private float currentCooldown = 0f;
    public bool isRecharging = false;
    public bool isPowerActive = false;
    private float currentActiveTime = 0f;
    public bool isPrisonRealmActive = false;
    public bool isStunnerActive = false;
    public List<PrisonData> imprisonedEnemies = new List<PrisonData>();
    public bool isBaldiImprisoned = false;
    private bool powerUpInput = false;


    [Header("References")]
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
    public Material[] mapMaterials;
    Shader urpLit;
    public RenderPipelineAsset URP_Low;
    public Image rechargeCircle;
    public GameObject baldi;
    public GameObject oggy;
    public GameObject uiiaCat;
    public player_controller player_Controller;


    void Awake()
    {
        currentPower = new Power();

        deEquipPower = new Power();
        deEquipPower.Effect = DeEquipPowerEffect;
        deEquipPower.onEquip = DeEquipPowerOnEquip;

        invincibilityShield = new Power();
        invincibilityShield.Effect = InvincibilityShieldEffect;
        invincibilityShield.onEquip = InvincibilityShieldOnEquip;
        invincibilityShield.duration = 14f;

        teleporter = new Power();
        teleporter.Effect = TeleporterEffect;
        teleporter.onEquip = TeleporterOnEquip;

        stunner = new Power();
        stunner.Effect = StunnerEffect;
        stunner.onEquip = StunnerOnEquip;
        stunner.isReusable = true;
        stunner.duration = 5f;
        // stunner.rechargeTime = 20f;

        powerDot = new Power();
        powerDot.Effect =  PowerDotEffect;
        powerDot.onEquip = PowerDotOnEquip;
        Shader.SetGlobalFloat("_FrightenedAmount", 0f);
        Shader.SetGlobalFloat("_FrightenedFlash", 0f);
        powerDot.duration = 7f;
        urpLit = Shader.Find("Universal Render Pipeline/Lit");

        wallBreaker = new Power();
        wallBreaker.Effect = WallBreakerEffect;
        wallBreaker.onEquip = WallBreakerOnEQuip;

        prisonRealm = new Power();
        prisonRealm.Effect = PrisonRealmEffect;
        prisonRealm.onEquip = PrisonRealmOnEquip;
        //prisonRealm.isReusable = true;
        

        EquipPower(powerDot);
        ChangeMapShader(urpLit);
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

    public void EquipPower(Power newPower)
    {
        // 1. Assign the power (this updates the duration, effect, and calls onEquip)
        currentPower.AssignPower(newPower);

        // 2. Reset the lock-out flags so the new power can be used immediately
        isPowerActive = false;
        isRecharging = false;
        
        // 3. Reset the timers
        currentActiveTime = 0f;
        currentCooldown = 0f;

        // 4. Reset the UI circle to full
        rechargeCircle.fillAmount = 1f; 

        rechargeCircle.enabled = false;
    }


    IEnumerator InvincibilityShieldEffect()
    {
        gameManager.isDead = true;
        invincibilityShieldAudio.Play();

        SetFullScreenPass(true);
        invincibilityShieldOverlay.SetActive(true);

        Debug.Log("Invincibility activated");

        yield return new WaitForSeconds(invincibilityShield.duration);
        gameManager.isDead = false;
        invincibilityShieldAudio.Stop();

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
        teleporterAudio.Play();
        yield return null;
    }

    public void TeleporterOnEquip()
    {
        currentPowerIcon.enabled = true;
        currentPowerIcon.sprite = teleporterIcon;
    }

    IEnumerator StunnerEffect()
    {
        StartCoroutine(StunAllEnemies(stunner.duration));
        stunnerAudio.Play();
        StartCoroutine(StunTheMap());

        isRecharging = true;
        currentPowerIcon.enabled = false;

        yield return null;
    }
    public IEnumerator StunAllEnemies(float duration)
    {
        StartCoroutine(StunBaldi(duration));
        StartCoroutine(StunOggy(duration));
        StartCoroutine(StunUiia(duration));

        yield return null;
    }

    public IEnumerator StunBaldi( float duration)
    {
        float baldiSpeed = baldiNavMesh.speed;
        baldiNavMesh.speed = 0f;
        isStunnerActive = true;

        yield return new WaitForSeconds(duration);

        baldiNavMesh.speed = baldiSpeed;
        isStunnerActive = false;
    }

    public IEnumerator StunOggy(float duration)
    {
        enemyController.isStunned = true;

        yield return new WaitForSeconds(duration);
        
        enemyController.isStunned = false;
    }

    public IEnumerator StunUiia(float duration)
    {
        float uiiaSpeed = uiiaNavMesh.speed;
        uiiaNavMesh.speed = 0f;

        yield return new WaitForSeconds(duration);
        
        uiiaNavMesh.speed = uiiaSpeed;
    }

    public void StunHitEnemy(CatchType type, float duration)
    {
        if (type == CatchType.baldi) 
            StartCoroutine(StunBaldi(duration));
        else if (type == CatchType.uiiacat) 
            StartCoroutine(StunUiia(duration));
        else if (type == CatchType.oggy) 
            StartCoroutine(StunOggy(duration));
    }

    public void StunnerOnEquip()
    {
        currentPowerIcon.enabled = true;
        currentPowerIcon.sprite = stunnerIcon;
    }

    private IEnumerator StunTheMap()
    {
        float duration = 5f;
        float startTime = Time.time;
        
        // Adjust these to control the pacing
        float startDelay = 0.30f; // Starts fast (5 times a second)
        float endDelay = 0.1f;   // Ends incredibly fast (100 times a second)
        
        bool toggleSwitch = true;

        // Keep looping as long as 4 seconds haven't passed
        while (Time.time - startTime < duration)
        {
            // 1. Fire the alternating functions
            if (toggleSwitch)
            {
                ChangeMapShader(pacManModeShader);
            }
            else
            {
                ChangeMapShader(urpLit);
            }

            // 2. Flip the switch for the next loop
            toggleSwitch = !toggleSwitch;

            // 3. Calculate how far along we are (0.0 to 1.0)
            float progress = (Time.time - startTime) / duration;

            // 4. Calculate the current wait time. 
            // Using Mathf.Pow(progress, 3) makes the delay drop off sharply at the very end.
            float currentDelay = Mathf.Lerp(startDelay, endDelay, Mathf.Pow(progress, 3));

            // 5. Wait for the calculated delay before looping again
            yield return new WaitForSeconds(currentDelay);
        }

        ChangeMapShader(urpLit);

        Debug.Log("4-second sequence complete!");
        // Optional: Trigger a final explosion or event here!
    }

    IEnumerator PowerDotEffect()
    {
        isPowerDotOn = true;
        Shader.SetGlobalFloat("_FrightenedAmount", 1f);
        Shader.SetGlobalFloat("_FrightenedFlash", 0f);
        powerDotAudio.Play();
        uiiaCatAudio.Pause();
        BGM.Pause();
        ChangeMapShader(pacManModeShader);


        yield return new WaitForSeconds(powerDot.duration -2f);

         Shader.SetGlobalFloat("_FrightenedFlash", 1f);

        yield return new WaitForSeconds(2f);

         isPowerDotOn = false;
         Shader.SetGlobalFloat("_FrightenedAmount", 0f);
         Shader.SetGlobalFloat("_FrightenedFlash", 0f);
         powerDotAudio.Stop();
         uiiaCatAudio.UnPause();
         BGM.UnPause();
         ChangeMapShader(urpLit);

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

        yield return new WaitForSecondsRealtime(0.5f);

        spawnManager.SpawnEnemy(enemy);
        Time.timeScale = gameManager.gameSpeed;
        powerDotAudio.UnPause();
    }

    public void ChangeMapShader(Shader shader)
    {
        foreach (var mat in mapMaterials)
            mat.shader = shader;
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

    IEnumerator PrisonRealmEffect()
    {
        isPrisonRealmActive = true;
        ChangeMapShader(prisonRealmShader);
        BGM.Pause();
        prisonRealmHeartbeat.Play();
        Debug.Log("Prison Realm Activated");

        yield return null;
    }

    void EndPrisonRealmEffect()
    {
        isPrisonRealmActive = false;
        ChangeMapShader(urpLit);
        BGM.UnPause();
        prisonRealmHeartbeat.Stop();
        Debug.Log("Prison Realm Deactivated");
    }

    public void PrisonRealmOnEquip()
    {
        currentPowerIcon.enabled = true;
        currentPowerIcon.sprite = prisonRealmSprite;
    }

    public void EnemyCaughtInPrison(CatchType type) 
    {
        GameObject capturedEnemy = null;
        Debug.Log("Enemy Caught");

        if (type == CatchType.baldi)
            {
                capturedEnemy = baldi;
                baldiEnemy.baldiBaseSpeed *= 1.2f;
                isBaldiImprisoned = true;
                Debug.Log("Baldi");
            }
        else if (type == CatchType.uiiacat)
            {
                capturedEnemy = uiiaCat;
                uiiaNavMesh.speed *= 1.2f;
                Debug.Log("UIIA");
            }
        else if (type == CatchType.oggy)
            {
                capturedEnemy = oggy;
                enemyController.oggyBAseSpeed *= 1.2f;
                Debug.Log("Oggy");
            }

        if (capturedEnemy != null) 
        {
            capturedEnemy.SetActive(false);

            int releasingRound = gameManager.round + 3;
            Debug.Log(capturedEnemy.name + " imprisoned. Releasing Round is " + releasingRound);
            imprisonedEnemies.Add(new PrisonData(capturedEnemy, releasingRound));

            StartCoroutine(ShowPrisonRealmCaughtImage());
        }
    }

    IEnumerator ShowPrisonRealmCaughtImage()
    {
        prisonRealmCapturedSound.Play();
        AnimationCurve popCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        priosnRealmCaughtImage.SetActive(true);
        Vector3 originalScale = priosnRealmCaughtImage.transform.localScale;

        priosnRealmCaughtImage.transform.localScale = Vector3.zero;

        float timeElapsed = 0f;

        while (timeElapsed < 0.7f)
        {
            timeElapsed += Time.deltaTime;
            
            // Evaluate the curve and apply the multiplier to the scale
            float curveValue = popCurve.Evaluate(timeElapsed / 0.25f);
            priosnRealmCaughtImage.transform.localScale = originalScale * curveValue;
            
            yield return null; 
        }

        // Snap to the exact original scale when finished
        priosnRealmCaughtImage.transform.localScale = originalScale;

        yield return new WaitForSeconds(2.0f);
        priosnRealmCaughtImage.SetActive(false);
        EndPrisonRealmEffect();
    }

    public void UsePowerUp()
    {
        if (currentPower == null || currentPower.Effect == null) return;
        if (gameManager.isDead) return;
        
        // Prevent using if already active or recharging
        if (isRecharging || isPowerActive) return; 

        StartCoroutine(currentPower.Effect());

        // If it has a duration, start the active timer
        if (currentPower.duration > 0f)
        {
            isPowerActive = true;
            currentActiveTime = 0f;
            currentPowerIcon.enabled = false; 

            rechargeCircle.enabled = true;
        }
        // If no duration, but it's reusable, jump straight to cooldown
        else if (currentPower.isReusable && currentPower.rechargeTime > 0f)
        {
            StartCooldown(); 
            // Note: StartCooldown() already has currentPowerIcon.enabled = false;
        }
        // If no duration and not reusable, de-equip immediately
        else if (!currentPower.isReusable)
        {
            
            EquipPower(deEquipPower);
        }
    }

    // Helper function to keep things clean
    private void StartCooldown()
    {
        isRecharging = true;
        currentCooldown = 0f;
        currentPowerIcon.enabled = false; 

        rechargeCircle.enabled = true;
    }

    void Update()
    {
        // --- UI TIMER LOGIC ---
        if (isPowerActive)
        {
            currentActiveTime += Time.deltaTime;
            
            // Empties the circle over time (1.0 to 0.0)
            rechargeCircle.fillAmount = 1f - (currentActiveTime / currentPower.duration);

            if (currentActiveTime >= currentPower.duration)
            {
                isPowerActive = false;
                
                // Check what to do after the power ends
                if (currentPower.isReusable && currentPower.rechargeTime > 0f)
                {
                    StartCooldown();
                }
                else if (!currentPower.isReusable)
                {
                    EquipPower(deEquipPower);
                }
            }
        }
        else if (isRecharging)
        {
            currentCooldown += Time.deltaTime;
            
            // Fills the circle back up over time (0.0 to 1.0)
            rechargeCircle.fillAmount = 1 - currentCooldown / currentPower.rechargeTime;

            if (currentCooldown >= currentPower.rechargeTime)
            {
                currentCooldown = 0f;
                isRecharging = false;
                currentPowerIcon.enabled = true;

                rechargeCircle.enabled = false;

                Debug.Log("Recharge Complete!");
            }
        }


        if (player_Controller.currentDevice == player_controller.TargetDevice.PC)
        {
            powerUpInput = (Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(1));
        } 

       if (powerUpInput)
        {
           UsePowerUp();
        } 

        // Loop BACKWARDS so we can safely remove enemies as they are released
        for (int i = imprisonedEnemies.Count - 1; i >= 0; i--)
        {
            PrisonData prisoner = imprisonedEnemies[i];

            // Check if the current round has reached or passed their release round
            if (gameManager.round >= prisoner.ReleaseRound)
            {
                // Set them free!
                prisoner.Enemy.SetActive(true);

                if (prisoner.Enemy == baldi) 
                    isBaldiImprisoned = false;

                Debug.Log(prisoner.Enemy.name + " Released Successfully");

                // Remove them from the prison list so we don't keep trying to release them
                imprisonedEnemies.RemoveAt(i);
            }
        }
    }
}
