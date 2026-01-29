using UnityEngine;

public class CheatManager : MonoBehaviour
{
    private string inputBuffer = "";
    private float lastKeyTime;
    private float resetDelay = 1.5f;
    public MusicManager musicManager;
    public UIIAController uIIAController;
    public GameManager gameManager;

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
            }

            if (inputBuffer.Contains("MWUAH"))
            {
                Mwuah();
                inputBuffer = "";
            }
        }

        if (Time.time - lastKeyTime > resetDelay)
            inputBuffer = "";
    }

    void GangnamStyle()
    {
        Debug.Log("ITS GANGNAM TIME");
        
        musicManager.PlaySong(3);
        musicManager.backgroundSource.volume = 1.0f;
    }

    void Walls()
    {
        uIIAController.ActivateAllWalls();
    }

    void Mwuah()
    {
        gameManager.isDead = true;
        Debug.Log("mwuah mwuah mwuah");
    }
}
