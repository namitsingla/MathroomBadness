using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public VideoPlayer BaldiStrike;
    public AudioSource PauseSound;

    public AudioSource BGM;
    public AudioSource uiiacatmusic;
    
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
        Time.timeScale = 1f;

        // Resuming sounds
        BaldiStrike.Play();
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
        BaldiStrike.Pause();                    // Pauses Quad 
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
        BaldiStrike.Play();

        SceneManager.LoadScene(0);
    }
}

