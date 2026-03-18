using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public enum CatchType
    {
        baldi,
        uiiacat,
        oggy
    }
public class GameManager : MonoBehaviour
{
    public int lives = 3;
    public int round = 1;

    public GameObject player;
    public player_controller movement;
    public GameObject baldi;
    public GameObject uiiacat;
    public GameObject oggy;

    Vector3 playerspawn = new Vector3(40f, 2.1f, 100f);
    Vector3 baldispawn = new Vector3(135f, 2.5f, -22.5f);
    Vector3 uiiacatspawn = new Vector3(-110f, 2f, 100f);
    Vector3 oggyspawn = new Vector3(-10f, 2f, -70f);

    public bool isDead = false;
    private float deathScreenTime = 2.5f;
    public DialogueSoundManager dialogueSoundManager;
    public SpawnManager spawnManager;

    //for baldi death screen
    public RawImage baldiJumpscare;
    public RectTransform baldiJumpscarePosiiton;
    public AudioSource BGM;
    public PowerSystem powerSystem;
    public float gameSpeed = 1f;
    public UIIAController uIIAController;

    public bool hasGameEnded = false;
    public GameObject endScreeen;
    public TextMeshProUGUI endScore;
    public collectedisplay collectedisplay;
    public AudioSource uiiacatmusic;
    public AudioSource gameOverSound;
    public bool hasCheated = false;
    public MusicManager musicManager;
    public GameObject baldiWarningcanvas;
    public void KhelKhatam(Transform lookTarget, CatchType type)
    {
        if (powerSystem.isPowerDotOn) return;

        if (isDead) return;

        if (powerSystem.isPrisonRealmActive) 
            powerSystem.EnemyCaughtInPrison(type); 

            lives -= 1;
            isDead = true;

            movement.enabled = false;

            if (type == CatchType.baldi)
            {
                StartCoroutine(BaldiDeathSequence(lookTarget));
            }
            else if (type == CatchType.uiiacat)
            {
                StartCoroutine(UiiaCatDeathSequence(lookTarget));
            }
            else if (type == CatchType.oggy)
            {
                StartCoroutine(OggyDeathSequence(lookTarget));
            }

            Debug.Log("ouch");
    }
    
    IEnumerator BaldiDeathSequence(Transform lookTarget)
    {
        // 1️⃣ Rotate player to enemy
        yield return StartCoroutine(RotateTowards(lookTarget.position, 0.1f));

        //to remove baldi from scene
        baldi.SetActive(false);

        //play sound and pause background music
        BGM.Pause();
        dialogueSoundManager.PlayBaldiDeathScreenSound();

        //to randomize position of image
        float x = Random.Range(-350f, 600f);
        float y = Random.Range(-1000f, -1300f);

        baldiJumpscarePosiiton.anchoredPosition = new Vector2(x, y);
        baldiJumpscare.enabled = true;
        deathScreenTime = 2.5f;

        StartCoroutine(FinishDeathSequence());
    }

    IEnumerator UiiaCatDeathSequence(Transform lookTarget)
    {
        // 1️⃣ Rotate player to enemy
        yield return StartCoroutine(RotateTowards(lookTarget.position, 0.1f));

        //play sound and pause background music
        BGM.Pause();

        deathScreenTime = 1f;
        StartCoroutine(FinishDeathSequence());
    }

    IEnumerator OggyDeathSequence(Transform lookTarget)
    {
        // 1️⃣ Rotate player to enemy
        yield return StartCoroutine(RotateTowards(lookTarget.position, 0.1f));

        //play sound and pause background music
        BGM.Pause();

        deathScreenTime = 1f;
        StartCoroutine(FinishDeathSequence());
    }

    IEnumerator RotateTowards(Vector3 target, float duration)
    {
        Quaternion start = transform.rotation;
        Vector3 dir = (target - transform.position).normalized;
        Quaternion end = Quaternion.LookRotation(dir);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            transform.rotation = Quaternion.Slerp(start, end, t);
            yield return null;
        }
    }

    public IEnumerator FinishDeathSequence()
    {
        yield return new WaitForSeconds(deathScreenTime);

        if (lives > 0) 
        {
            baldiJumpscare.enabled = false;
            movement.enabled = true;
            isDead = false;

            //to only activate it if not in prison
            bool releaseBaldi = true;
            foreach (PrisonData list in powerSystem.imprisonedEnemies)
            {
                if (list.Enemy == baldi) 
                    releaseBaldi = false;
            }
            if (releaseBaldi)
                baldi.SetActive(true);
            else 
                Debug.Log("Baldi is stil in priosn");
            
            BGM.UnPause();

            //baldi.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(baldispawn);
            //baldi.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(baldispawn);

            //uiiacat.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(uiiacatspawn);
            //uiiacat.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(uiiacatspawn);

            //oggy.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(oggyspawn);
            //oggy.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(oggyspawn);
            
            spawnManager.SpawnAllEnemies();
            uIIAController.DeactivateAllWalls();
            
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = playerspawn;
            player.GetComponent<CharacterController>().enabled = true;

            baldiWarningcanvas.SetActive(false);

            StartCoroutine(powerSystem.StunAllEnemies());
        }
        else
        {
            hasGameEnded = true;
            Time.timeScale = 0f;
            uiiacatmusic.Stop();
            endScreeen.SetActive(true);

            if (!hasCheated)
                endScore.text = "CLASS OVER\nSCORE: " + collectedisplay.score.ToString();
            else 
                endScore.text = "CHEATS HAVE\nBEEN USED";

            gameOverSound.Play();
            musicManager.PlaySong(8);

        }
    }

    void Update()
    {
        if (!hasGameEnded) return;
        if (Input.anyKeyDown)
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None; // Unlocks the cursor for menus
            SceneManager.LoadScene(0);
        }
    }
}
