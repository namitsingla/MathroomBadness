using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public enum Rarity
{
    Common,
    Rare,
    Epic,
    Cursed,
    Legendary
}

[System.Serializable]
public class Reward
{
    public string name;
    public Rarity rarity;
    public Sprite icon;

    public int maxPicks = -1;   // -1 = infinite
    [HideInInspector] public int timesPicked = 0;

    [TextArea(3, 5)] // This makes the text box bigger in the Unity Inspector
    public string description;

    public List<string> removesOnPickNames;
    public List<string> prerequisites; 
}

public class RarityManager : MonoBehaviour
{

    [Header("Rewards")]
    public Button[] rewardButtons;
    public TMP_Text[] rewardTexts; 
    public Image[] rewardIcons;
    public RewardTooltipTrigger[] infoButtonTriggers; 
    public GameObject tooltipPanel;    // The background panel for the tooltip
    public TMP_Text tooltipText;       // The text component inside the panel
    public RectTransform tooltipRect;
    public Image tooltipBorder; 
    public Vector2 tooltipOffset = new Vector2(15f, -15f);
    public List<Reward> rewards;


    [Header("Rarity Drops")]
    public int rewardCount = 3;
    public float[] initialWeights =  {50f, 30f, 20f, 5f, 1f};
    public float[] perRoundIncre = {0f, 3.5f, 3f, 1f, 0.25f};
    public float[] perItemIncre = {0f, 3f, 2.5f, 1f, 0.2f};
    public float[] cumulativeFinalWeights = {50f, 80f, 100f, 105f, 106f};
    public float[] luckIncrement = {1f, 1f, 1f, 1f, 1f};

    

    [Header("Variables")]
    public bool isSpinning = false;
    private int activeSpins = 0;
    public bool isRewardScreenUp = false;


    [Header("Reward History")]
    public List<string> claimedRewardNames = new List<string>();
    private bool IsRewardUnlocked(Reward reward)
    {
        if (reward.prerequisites == null || reward.prerequisites.Count == 0)
            return true;

        foreach (string req in reward.prerequisites)
        {
            if (!claimedRewardNames.Contains(req))
            {
                return false;
            }
        }

        return true; 
    }


    [Header("Sound Effects")]
    public AudioSource BGM;
    public AudioSource casinoSound;
    public AudioSource rewardSelectSound;
    public AudioSource cursedRewardSelectSound;
    public AudioSource rewardScreenMusic;


    [Header("References")]
    public GameObject rewardsUI;
    public BoostsHandler boostsHandler;
    public GameManager gameManager;
    public collectedisplay collectedisplay;
    public MusicManager musicManager;
    public GameObject rewardButtonsUI;
    public PowerSystem powerSystem;


    public void ShowTooltip(Reward reward)
    {
        tooltipPanel.SetActive(true);
        tooltipText.text = reward.description;
        
        if (tooltipBorder != null)
        {
            tooltipBorder.color = GetRarityColor(reward.rarity);
        }
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }

    void Update()
    {
        // If the tooltip is visible, lock its position to the mouse + offset
        if (tooltipPanel != null && tooltipPanel.activeSelf)
        {
            tooltipRect.position = Input.mousePosition + (Vector3)tooltipOffset;
        }
    }

    public void UpdateRarityDrops(int collected)
    {

        for (int i = 0; i < 5; i++)
        {
            cumulativeFinalWeights[i] = 0f;
            for (int j = 0; j <= i; j++)
            {
                cumulativeFinalWeights[i] += (initialWeights[j] + perRoundIncre[j]*(gameManager.round - 1) + perItemIncre[j]*(collected - 3))*luckIncrement[j];
            }
        }

        for (int i = 0; i < 5; i++)
            Debug.Log(cumulativeFinalWeights[i]/cumulativeFinalWeights[4]);
    }

    public void IncreaseLuckBy(int increaseLuckBy)
    {
        luckIncrement[2] += 1f/6f *increaseLuckBy;
        luckIncrement[3] += 0.5f *increaseLuckBy;
        luckIncrement[4] += 4f/3f *increaseLuckBy;
    }

    Reward GetRandomRewardFromPool(List<Reward> pool)
    {
        float roll = Random.Range(0,cumulativeFinalWeights[4]);

        Rarity selectedRarity;

        if (roll < cumulativeFinalWeights[0])
            selectedRarity = Rarity.Common;
        else if (roll < cumulativeFinalWeights[1])
            selectedRarity = Rarity.Rare;
        else if (roll < cumulativeFinalWeights[2])
            selectedRarity = Rarity.Epic;
        else if (roll < cumulativeFinalWeights[3])
            selectedRarity = Rarity.Cursed;
        else 
            selectedRarity = Rarity.Legendary;


        List<Reward> matches = pool.FindAll(r => r.rarity == selectedRarity);

        if (matches.Count == 0)
            matches = pool; // fallback if no reward of that rarity exists

        if (matches.Count == 0)
            return null;

        return matches[Random.Range(0, matches.Count)];
    }

    Reward GetRandomFromRarity(Rarity rarity)
    {
        List<Reward> matches = rewards.FindAll(r => r.rarity == rarity);

        if(matches.Count == 0)
            return null;

        return matches[Random.Range(0, matches.Count)];
    }

    public IEnumerator GenerateRewards()
    {
        rewardScreenMusic.Play();
        casinoSound.Play();
        rewardButtonsUI.SetActive(true);

        isSpinning = true;

        List<Reward> tempPool = new List<Reward>();
        foreach (Reward r in rewards)
        {
            if (IsRewardUnlocked(r))
            {
                tempPool.Add(r);
            }
        }

        Reward[] finalRewards = new Reward[rewardCount];

        // First decide all rewards
        for (int i = 0; i < rewardCount; i++)
        {
            if (tempPool.Count == 0)
                continue;

            Reward r = GetRandomRewardFromPool(tempPool);
            tempPool.Remove(r);
            finalRewards[i] = r;

            rewardButtons[i].onClick.RemoveAllListeners();
            rewardButtons[i].onClick.AddListener(() => PickReward(r));
        }

        // Then spin them all at once
        activeSpins = 0;

        for (int i = 0; i < rewardCount; i++)
        {
            if (finalRewards[i] != null)
            {
                activeSpins++;
                StartCoroutine(SpinSlot(i, finalRewards[i], tempPool));
            }
        }

        // Wait until all spins finish
        while (activeSpins > 0)
        {
            yield return null;
        }

        isSpinning = false;
        casinoSound.Stop();
    }

    void ButtonColor(Rarity rarity, int i)
    {
        rewardButtons[i].image.color = GetRarityColor(rarity);
    }

    public Color GetRarityColor(Rarity rarity)
    {
        switch(rarity)
        {
            case Rarity.Common: return new Color32(80, 200, 120, 255);
            case Rarity.Rare: return new Color32(65, 140, 255, 255);
            case Rarity.Epic: return new Color32(200, 70, 255, 255);
            case Rarity.Cursed: return new Color32(140, 20, 30, 255);
            case Rarity.Legendary: return new Color32(217, 217, 49, 255);
            default: return Color.white;
        }
    }

    void PickReward(Reward reward)
    {
        if (isSpinning) return;

        Debug.Log("Picked: " + reward.name);

        reward.timesPicked += 1;

        // add name to history 
        if (!claimedRewardNames.Contains(reward.name))
        {
            claimedRewardNames.Add(reward.name);
        }

        // Remove if max reached
        if (reward.maxPicks > 0 && reward.timesPicked >= reward.maxPicks)
        {
            rewards.Remove(reward);
        }

        // Remove linked rewards by name
        if (reward.removesOnPickNames != null)
        {
            foreach (string rewardName in reward.removesOnPickNames)
            {
                Reward toRemove = rewards.Find(x => x.name == rewardName);

                if (toRemove != null)
                    rewards.Remove(toRemove);
            }
        }

        if (reward.rarity == Rarity.Cursed)
            cursedRewardSelectSound.Play();
        else 
            rewardSelectSound.Play();

        // Apply effect later
        switch(reward.name)
        {
            case "Running Shoes":
                boostsHandler.SpeedIncrease();
                break;

            case "Coefficient Boost":
                boostsHandler.MultiplierIncrease();
                break;

            case "Binoculars":
                boostsHandler.VisionIncrease();
                break;

            case "Minimap Increase":
                boostsHandler.MinimapIncrease();
                break;

            case "Paint Spill":
                boostsHandler.SlowDownEnemies();
                break;

            case "A.T. Barrier":
                boostsHandler.InvincibilityShield();
                break;

            case "Teleporter":
                boostsHandler.Teleporter();
                break;

            case "Stunner":
                boostsHandler.Stunner();
                break;

            case "Ragebait Baldi":
                boostsHandler.RagebaitBaldi();
                break;

            case "Radar":
                boostsHandler.Radar();
                break;

            case "Power Dot":
                boostsHandler.PowerDot();
                break;  

            case "Wall Breaker":
                boostsHandler.WallBreaker();
                break;

            case "An Extra Life":
                boostsHandler.AnExtraLife();
                break;

            case "Wall Runner":
                boostsHandler.WallRunner();
                break;   

            case "Entropy Cache":
                boostsHandler.RandomPowerUp();
                break;  

            case "Minimap Eater":
                boostsHandler.MiniMapEater();
                break; 

            case "Time SpeedUp":
                boostsHandler.TimeSpeedUp();
                break;

            case "Hallway Litter":
                boostsHandler.IncreaseItemSpawnRate();
                break;

            case "Prison Realm":
                boostsHandler.PrisonRealm();
                break;

            case "The Fourth Wall":
                boostsHandler.TheFourthWall();
                break;

            case "Diagonalizer":
                boostsHandler.CornerCutter();
                break;
            
            case "Schrodinger's Cat":
                boostsHandler.SchrodingersCat();
                break;

            case "Mitosis":
                boostsHandler.Mitosis();
                break;

            case "Meiosis":
                boostsHandler.Meiosis();
                break;

            case "High Stakes":
                boostsHandler.HighStakes();
                break;

            case "Aura Farming":
                boostsHandler.AuraFarming();
                break;

            case "Choice Specs":
                boostsHandler.TunnelVision();
                break;

            case "Loaded Dice":
                boostsHandler.LoadedDice();
                break;

            case "Ferromagnetism":
                boostsHandler.PassiveMagnet();
                break;

            case "Shukaku":
                boostsHandler.ShukakuTurnOn();
                break;
        }

        // Hide UI
        rewardButtonsUI.SetActive(false);
        rewardsUI.SetActive(false);
        HideTooltip();
        Time.timeScale = gameManager.gameSpeed;
        BGM.UnPause();
        Cursor.lockState = CursorLockMode.Locked;
        collectedisplay.mult += 0.1f;
        
        StartCoroutine(powerSystem.StunAllEnemies(5f));

        if (boostsHandler.ifRandomPowerUpEachRound)
        {
            boostsHandler.GetRandomPowerUp();
            //Debug.Log("Signal sent");
        }

        rewardScreenMusic.Stop();
        musicManager.UpdateBackgroundMusic();

        gameManager.round += 1;
        collectedisplay.UpdateDisplay();

        isRewardScreenUp = false;

    }

    IEnumerator SpinSlot(int slotIndex, Reward finalReward, List<Reward> pool)
    {
        float baseSpinTime = 2.0f + slotIndex * 0.4f;

        // Add rarity suspense
        switch (finalReward.rarity)
        {
            case Rarity.Rare:
                baseSpinTime += 0.3f;
                break;

            case Rarity.Epic:
                baseSpinTime += 0.8f;
                break;

            case Rarity.Cursed:
                baseSpinTime += 1.2f;
                break;
        }

        float totalSpinTime = baseSpinTime;
        float endTime = Time.unscaledTime + totalSpinTime;

        float minDelay = 0.03f;
        float maxDelay = 0.25f;

        float nextSwapTime = 0f;

        rewardButtons[slotIndex].interactable = false;

        while (Time.unscaledTime < endTime)
        {
            float remaining = endTime - Time.unscaledTime;
            float t = 1f - (remaining / totalSpinTime); // 0 → 1 progress

            casinoSound.pitch = Mathf.Lerp(1.8f, 0f, t);

            float delay = Mathf.Lerp(minDelay, maxDelay, t * t);

            if (Time.unscaledTime >= nextSwapTime)
            {
                if (pool.Count > 0)
                {
                    Reward random = pool[Random.Range(0, pool.Count)];
                    rewardTexts[slotIndex].text = random.name;
                    rewardIcons[slotIndex].sprite = random.icon;
                    ButtonColor(random.rarity, slotIndex);
                }

                nextSwapTime = Time.unscaledTime + delay;
            }

            yield return null; // wait one frame

            // remaining = endTime - Time.unscaledTime;

            // // If it's Epic/Cursed and near the end, exaggerate slowdown
            // if ((finalReward.rarity == Rarity.Epic || finalReward.rarity == Rarity.Cursed) 
            //     && remaining < 0.5f)
            // {
            //     delay *= 1.5f;
            // }
        }

        // Final snap
        rewardTexts[slotIndex].text = finalReward.name;
        rewardIcons[slotIndex].sprite = finalReward.icon;
        ButtonColor(finalReward.rarity, slotIndex);
        infoButtonTriggers[slotIndex].currentReward = finalReward;

        rewardButtons[slotIndex].interactable = true;

        activeSpins--;

        StartCoroutine(PunchScale(rewardButtons[slotIndex].transform));

        if (finalReward.rarity == Rarity.Epic)
            StartCoroutine(FlashColor(slotIndex, Color.magenta));

        if (finalReward.rarity == Rarity.Cursed)
            StartCoroutine(FlashColor(slotIndex, Color.red));

        if (slotIndex == 2) // last slot
        {
            StartCoroutine(ScreenShake(0.1f, 1f));
        }
    }

    IEnumerator PunchScale(Transform target)
    {
        Vector3 original = target.localScale;
        Vector3 enlarged = original * 1.2f;

        float t = 0f;
        float duration = 0.15f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            target.localScale = Vector3.Lerp(original, enlarged, t / duration);
            yield return null;
        }

        t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            target.localScale = Vector3.Lerp(enlarged, original, t / duration);
            yield return null;
        }

        target.localScale = original;
    }

    IEnumerator FlashColor(int i, Color flash)
    {
        Color original = rewardButtons[i].image.color;
        rewardButtons[i].image.color = flash;
        yield return new WaitForSecondsRealtime(0.15f);
        rewardButtons[i].image.color = original;
    }

    IEnumerator ScreenShake(float duration, float magnitude)
    {
        Vector3 originalPos = Camera.main.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            Camera.main.transform.localPosition = originalPos + new Vector3(x, y, 0);

            yield return null;
        }

        Camera.main.transform.localPosition = originalPos;
    }
}