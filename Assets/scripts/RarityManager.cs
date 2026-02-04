using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public enum Rarity
{
    Common,
    Rare,
    Epic,
    Cursed
}

[System.Serializable]
public class Reward
{
    public string name;
    public Rarity rarity;
    public Sprite icon;
}

public class RarityManager : MonoBehaviour
{
    public Button[] rewardButtons;
    public TMP_Text[] rewardTexts; 
    public List<Reward> rewards;
    public GameObject rewardsUI;
    public Image[] rewardIcons;
    public BoostsHandler boostsHandler;
    public GameManager gameManager;
    //public collectedisplay collectedisplay;
    public AudioSource BGM;
    public collectedisplay collectedisplay;
    public MusicManager musicManager;

    public Reward GetRandomReward()
    {
        int roll = Random.Range(0,100);

        if(roll < 45)
            return GetRandomFromRarity(Rarity.Common);
        else if(roll < 70)
            return GetRandomFromRarity(Rarity.Rare);
        else if(roll < 95)
            return GetRandomFromRarity(Rarity.Epic);
        else 
            return GetRandomFromRarity(Rarity.Cursed);
    }

    Reward GetRandomFromRarity(Rarity rarity)
    {
        var matches = rewards.FindAll(r => r.rarity == rarity);

        if(matches.Count == 0)
            return null;

        return matches[Random.Range(0, matches.Count)];
    }

    public void GenerateRewards()
    {
        for(int i = 0; i < 3; i++)
        {
            Reward r = GetRandomReward();

            rewardTexts[i].text = r.name;

            ButtonColor(r.rarity, i);
            rewardIcons[i].sprite = r.icon;

            int index = i; // needed for button callback
            rewardButtons[i].onClick.RemoveAllListeners();
            rewardButtons[i].onClick.AddListener(() => PickReward(r));
        }
    }

    void ButtonColor(Rarity rarity, int i)
    {
        switch(rarity)
        {
            case Rarity.Common:
                rewardButtons[i].image.color = new Color32(80, 200, 120, 255);;
                break;

            case Rarity.Rare:
                rewardButtons[i].image.color = new Color32(65, 140, 255, 255);
                break;

            case Rarity.Epic:
                rewardButtons[i].image.color = new Color32(200, 70, 255, 255);
                break;

            case Rarity.Cursed:
                rewardButtons[i].image.color = new Color32(140, 20, 30, 255);
                break;
        }
    }

    void PickReward(Reward reward)
    {
        Debug.Log("Picked: " + reward.name);

        // Apply effect later
        switch(reward.name)
        {
            case "Speed Increase":
                boostsHandler.SpeedIncrease();
                break;

            case "Multiplier Increase":
                boostsHandler.MultiplierIncrease();
                break;

            case "Vision Increase":
                boostsHandler.VisionIncrease();
                break;

            case "Minimap Increase":
                boostsHandler.MinimapIncrease();
                break;

            case "Slow Down Enemies":
                boostsHandler.SlowDownEnemies();
                break;

            case "Invincibility Shield":
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

            case "A Random PowerUp Each Round":
                boostsHandler.RandomPowerUp();
                break;  

            case "Minimap Eater":
                boostsHandler.MiniMapEater();
                break; 

            case "Time SpeedUp":
                boostsHandler.TimeSpeedUp();
                break;

            case "Increase Item Spawn Rate":
                boostsHandler.IncreaseItemSpawnRate();
                break;
        }

        // Hide UI
        rewardsUI.SetActive(false);
        Time.timeScale = gameManager.gameSpeed;
        BGM.UnPause();
        Cursor.lockState = CursorLockMode.Locked;
        collectedisplay.mult += 0.1f;

        if (boostsHandler.ifRandomPowerUpEachRound)
        {
            boostsHandler.GetRandomPowerUp();
            //Debug.Log("Signal sent");
        }

        musicManager.PlaySong(0);

        collectedisplay.UpdateDisplay();
    }
}