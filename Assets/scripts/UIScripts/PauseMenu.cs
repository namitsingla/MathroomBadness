using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public AudioSource PauseSound;

    public AudioSource BGM;
    public GameManager gameManager;
    public RarityManager rarityManager;
    
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = gameManager.gameSpeed;

        // Resuming sounds
        BGM.UnPause();
        EnemyManager.instance.PauseUIIAMusic();

        Cursor.lockState = CursorLockMode.Locked;
        PauseSound.Play ();  

        GameIsPaused = false;
    }

    public void Pause()
    {
        if (rarityManager.isRewardScreenUp) return;
        
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;

        // Pausing sounds
        BGM.Pause();
        EnemyManager.instance.UnpauseUIIAMusic();

        Cursor.lockState = CursorLockMode.None; // Unlocks the cursor
        PauseSound.Play ();

        GameIsPaused = true;
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;

        SceneManager.LoadScene(0);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Pause();
        }
    }

    // This is called when the user clicks away or a popup covers the game
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Pause();
        }
    }
}

