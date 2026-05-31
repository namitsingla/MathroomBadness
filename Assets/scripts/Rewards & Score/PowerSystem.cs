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
    public AudioSource wallBreakSound;


    [Header("Power Specific Refrences")]
    public GameObject invincibilityShieldOverlay;
    public Shader pacManModeShader;
    public GameObject priosnRealmCaughtImage;
    public PrisonRealmSealer prisonRealmSealer;

    [Header("Variables")]
    public bool isPowerDotOn = false;
    private float currentCooldown = 0f;
    public bool isRecharging = false;
    public bool isPowerActive = false;
    private float currentActiveTime = 0f;
    public bool isPrisonRealmActive = false;
    public bool isStunnerActive = false;
    private bool powerUpInput = false;
    public bool isSealingInProgress = false;

    [Header("Global Domain Settings")]
    public Transform playerTransform;
    public float maxRadius = 150f;
    public float expansionTime = 2.5f;
    public float collapseSpeed = 250f; 

    [Header("Domain Materials")]
    public Material pacmanMaterial;
    public Material prisonRealmMaterial;
    public Material wireframeMaterial;
    private float currentRadius = 0f;
    private float activeTimer = 0f; 
    private float targetSustainDuration = 0f; // Stores how long to hold it AFTER it expands
    private Material activeMaterial; 

    private enum DomainState { Inactive, Expanding, Sustaining, Collapsing }
    private DomainState currentState = DomainState.Inactive;

    private int playerPosID;
    private int effectRadiusID;


    [Header("References")]
    public GameManager gameManager;
    public UniversalRendererData rendererData;
    public SpawnManager spawnManager; 
    public collectedisplay collectedisplay;
    public AudioSource BGM;
    public Material[] mapMaterials;
    Shader urpLit;
    public RenderPipelineAsset URP_Low;
    public Image rechargeCircle;
    public player_controller player_Controller;
    public EnemyManager enemyManager;


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
        stunner.rechargeTime = 20f;

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
        prisonRealm.duration = 15f;
        //prisonRealm.isReusable = true;
        

        playerPosID = Shader.PropertyToID("_PlayerPosition");
        effectRadiusID = Shader.PropertyToID("_EffectRadius");

        // 1. Safety first: Force all domains to 0 radius at the start
        ResetAllDomains();

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

    public void StartDomain(Material domainMat, float duration)
    {
        if (activeMaterial != null && activeMaterial != domainMat)
        {
            activeMaterial.SetFloat(effectRadiusID, 0f);
        }

        activeMaterial = domainMat;
        currentRadius = 0f;
        
        targetSustainDuration = duration; // Save the sustain duration for later
        activeTimer = 0f; // Reset the timer to 0 to track the expansion phase
        
        currentState = DomainState.Expanding; 
    }

    private void ResetAllDomains()
    {
        if (pacmanMaterial != null) pacmanMaterial.SetFloat(effectRadiusID, 0f);
        if (prisonRealmMaterial != null) prisonRealmMaterial.SetFloat(effectRadiusID, 0f);
        if (wireframeMaterial != null) wireframeMaterial.SetFloat(effectRadiusID, 0f);
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
        isStunnerActive = true;
        StartCoroutine(StunAllEnemies(stunner.duration));
        stunnerAudio.Play();
        StartCoroutine(StunTheMap());

        yield return new WaitForSeconds(stunner.duration);
        isStunnerActive = false;

        isRecharging = true;
        currentPowerIcon.enabled = false;
    }
    public IEnumerator StunAllEnemies(float duration)
    {
        enemyManager.StunAllEnemies(duration);

        yield return null;
    }

    public void StunHitEnemy(CatchType type, float duration)
    {
        foreach (BaseEnemy enemy in EnemyManager.instance.GetAllEnemies())
        {
            if (enemy.catchType == type)
                enemy.Stun(duration);
        }
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
        enemyManager.PauseUIIAMusic();
        BGM.Pause();
        //ChangeMapShader(pacManModeShader);
        StartDomain(pacmanMaterial, powerDot.duration -3.5f);


        yield return new WaitForSeconds(powerDot.duration -2f);

         Shader.SetGlobalFloat("_FrightenedFlash", 1f);

        yield return new WaitForSeconds(2f);

         isPowerDotOn = false;
         Shader.SetGlobalFloat("_FrightenedAmount", 0f);
         Shader.SetGlobalFloat("_FrightenedFlash", 0f);
         powerDotAudio.Stop();
         enemyManager.UnpauseUIIAMusic();
         BGM.UnPause();
         //ChangeMapShader(urpLit);

    }

    public void PowerDotOnEquip()
    {
        currentPowerIcon.enabled = true;
        currentPowerIcon.sprite = powerDotSprite;
    }

    public IEnumerator PowerDotTeleport(BaseEnemy enemy)
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
        WallManager.instance.DeactivateAllWalls();
        
        wallBreakSound.Play();

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
        //ChangeMapShader(prisonRealmShader);
        StartDomain(prisonRealmMaterial, prisonRealm.duration - 3.5f);
        BGM.Pause();
        prisonRealmHeartbeat.Play();
        Debug.Log("Prison Realm Activated");

        yield return new WaitForSeconds(prisonRealm.duration);

        EndPrisonRealmEffect();
    }

    void EndPrisonRealmEffect()
    {
        isPrisonRealmActive = false;
        //ChangeMapShader(urpLit);
        currentState = DomainState.Collapsing;

        BGM.UnPause();
        prisonRealmHeartbeat.Stop();

        // FIX: Only de-equip if the current active power is STILL the Prison Realm
        if (currentPower != null && currentPower.Effect == prisonRealm.Effect)
        {
            EquipPower(deEquipPower);
        }

        Debug.Log("Prison Realm Deactivated");
    }

    public void PrisonRealmOnEquip()
    {
        currentPowerIcon.enabled = true;
        currentPowerIcon.sprite = prisonRealmSprite;
    }

    public void EnemyCaughtInPrison(BaseEnemy captured)
    {
        if (isSealingInProgress) return;

        isSealingInProgress = true;
        StartCoroutine(SealEnemy(captured));
    }

    IEnumerator SealEnemy(BaseEnemy enemy)
    {
        isPrisonRealmActive = true;

        // use isStunned since Update already respects it
        enemy.isStunned = true;

        yield return StartCoroutine(prisonRealmSealer.ExecuteSealAndWait(enemy.transform));
        
        EnemyManager.instance.ImprisonEnemy(enemy, gameManager.round);
        isSealingInProgress = false;
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

       if (playerTransform != null)
        {
            Shader.SetGlobalVector(playerPosID, playerTransform.position);
        }

        if (activeMaterial != null)
        {
            switch (currentState)
            {
                case DomainState.Expanding:
                    // 1. Count up the timer
                    activeTimer += Time.deltaTime;
                    
                    // 2. Get a percentage of how far along the expansion time we are (0.0 to 1.0)
                    float t = activeTimer / expansionTime;
                    
                    // 3. THE MATH MAGIC (Ease-In Cubic Curve)
                    // Multiplying t by itself makes the curve start extremely flat, then curve violently upwards.
                    float easeInT = t * t; 
                    
                    // 4. Apply the curve to the radius
                    currentRadius = Mathf.Lerp(0f, maxRadius, easeInT);
                    
                    if (activeTimer >= expansionTime)
                    {
                        currentRadius = maxRadius;
                        currentState = DomainState.Sustaining;
                        
                        // Load the actual domain duration timer now that expansion is done!
                        activeTimer = targetSustainDuration; 
                    }
                    
                    activeMaterial.SetFloat(effectRadiusID, currentRadius);
                    break;

                case DomainState.Sustaining:
                    activeTimer -= Time.deltaTime;
                    
                    if (activeTimer <= 0f)
                    {
                        currentState = DomainState.Collapsing;
                    }
                    break;

                case DomainState.Collapsing: // Left exactly as you requested
                    currentRadius -= collapseSpeed * Time.deltaTime;
                    
                    if (currentRadius <= 0f)
                    {
                        currentRadius = 0f;
                        currentState = DomainState.Inactive;
                        activeMaterial.SetFloat(effectRadiusID, 0f);
                        activeMaterial = null; 
                    }
                    else
                    {
                        activeMaterial.SetFloat(effectRadiusID, currentRadius);
                    }
                    break;
            }
        }
    }
}
