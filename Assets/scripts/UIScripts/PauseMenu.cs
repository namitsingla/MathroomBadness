using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public AudioSource PauseSound;

    public AudioSource BGM;
    public AudioSource uiiacatmusic;
    public GameManager gameManager;
    
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
        uiiacatmusic.UnPause();

        Cursor.lockState = CursorLockMode.Locked;
        PauseSound.Play ();  

        GameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;

        // Pausing sounds
        BGM.Pause();
        uiiacatmusic.Pause();

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
}

