using UnityEngine;

public class CheatManager : MonoBehaviour
{
    private string inputBuffer = "";
    private float lastKeyTime;
    private float resetDelay = 1.5f;
    public MusicManager musicManager;
    public UIIAController uIIAController;
    public GameManager gameManager;
    public player_controller player_Controller;
    public ExitDoor exitDoor;
    public RarityManager rarityManager;
    public PowerSystem powerSystem;

    void Update()
    {
        //Debug.Log("Cheat manager running");

        foreach (char c in Input.inputString)
        {
            inputBuffer += char.ToUpper(c);
            lastKeyTime = Time.time;

            if (inputBuffer.Contains("GANGNAMSTYLE"))
            {
                GangnamStyle();
                inputBuffer = "";
            }

            if (inputBuffer.Contains("WALLS"))
            {
                Walls();
                inputBuffer = "";
                gameManager.hasCheated = true;
            }

            if (inputBuffer.Contains("VIVIIKUN"))
            {
                ViviiKun();
                inputBuffer = "";
                gameManager.hasCheated = true;
            }

            if (inputBuffer.Contains("PKPKPK"))
            {
                PkPkPk();
                inputBuffer = "";
                gameManager.hasCheated = true;
            }

            if (inputBuffer.Contains("GOPALA"))
            {
                ReRoll();
                inputBuffer = "";
                gameManager.hasCheated = true;
            }

            if (inputBuffer.Contains("KENJAKU"))
            {
                PrisonRealm();
                inputBuffer = "";
                gameManager.hasCheated = true;
            }
        }

        if (Time.time - lastKeyTime > resetDelay)
            inputBuffer = "";
    }

    void GangnamStyle()
    {
        Debug.Log("ITS GANGNAM TIME");
        
        musicManager.PlaySong(9);
        //musicManager.backgroundSource.volume = 1.0f;
    }

    void Walls()
    {
        uIIAController.DeactivateAllWalls();
    }

    void ViviiKun()
    {
        if (!gameManager.isDead) 
        {
            gameManager.isDead = true;
            player_Controller.moveSpeed *= 5f;
        }
        else 
        {
            gameManager.isDead = false;
            player_Controller.moveSpeed *= 0.2f;
        }
        
        Debug.Log("mwuah mwuah mwuah");
    }

    void PkPkPk()
    {
        exitDoor.RoundEndSequence();
    }

    void ReRoll()
    {
        rarityManager.StartCoroutine(rarityManager.GenerateRewards());
    }

    void PrisonRealm()
    {
        powerSystem.currentPower.AssignPower(powerSystem.prisonRealm);
    }
}
