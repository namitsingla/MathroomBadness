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
    [Header("Game State")]
    public int lives = 3;
    public int round = 1;
    public float gameSpeed = 1f;
    public bool isDead = false;
    public bool hasGameEnded = false;
    public bool hasCheated = false;

    [Header("Player Settings")]
    public GameObject player;
    public player_controller movement;

    [Header("Enemy References")]
    public GameObject baldi;
    public GameObject uiiacat;
    public GameObject oggy;
    public UIIAController uIIAController;

    [Header("Spawn Coordinates")]
    [SerializeField] private Vector3 playerspawn = new Vector3(40f, 3.1f, 100f);
    [SerializeField] private Vector3 baldispawn = new Vector3(135f, 2.5f, -22.5f);
    [SerializeField] private Vector3 uiiacatspawn = new Vector3(-110f, 2f, 100f);
    [SerializeField] private Vector3 oggyspawn = new Vector3(-10f, 2f, -70f);

    [Header("Death Sequence & Jumpscares")]
    [SerializeField] private float deathScreenTime = 2.5f;
    public RawImage baldiJumpscare;
    public RectTransform baldiJumpscarePosiiton; // (Typo here: Posiiton)

    [Header("Managers")]
    public SpawnManager spawnManager;
    public PowerSystem powerSystem;
    public MusicManager musicManager;
    public DialogueSoundManager dialogueSoundManager;
    public BoostsHandler boostsHandler;

    [Header("UI & HUD")]
    public GameObject baldiWarningcanvas;
    public GameObject endScreeen;
    public TextMeshProUGUI endScore;
    public collectedisplay collectedisplay;

    [Header("Audio")]
    public AudioSource BGM;
    public AudioSource uiiacatmusic;
    public AudioSource gameOverSound;
    public void KhelKhatam(Transform lookTarget, CatchType type)
    {
        if (powerSystem.isPowerDotOn) return;

        if (isDead) return;

        if (boostsHandler.isShukakuActive)
        {
            StartCoroutine(boostsHandler.ShukakuProtection(type));
            return;
        }

        if (powerSystem.isPrisonRealmActive) 
            powerSystem.EnemyCaughtInPrison(type); 

            lives -= 1;
            isDead = true;

            movement.enabled = false;
            player.GetComponent<CharacterController>().enabled = false;

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
        // WE MUST SPECIFY player.transform EVERYWHERE HERE
        Vector3 flatTarget = new Vector3(target.x, player.transform.position.y, target.z);
        Vector3 dir = (flatTarget - player.transform.position).normalized;
        if (dir == Vector3.zero) yield break;

        Quaternion start = player.transform.rotation;
        Quaternion end = Quaternion.LookRotation(dir);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            
            // 1. Apply the rotation to the PLAYER
            player.transform.rotation = Quaternion.Slerp(start, end, t);

            // 2. Lock X and Z on the PLAYER
            Vector3 currentEuler = player.transform.eulerAngles;
            currentEuler.x = 0;
            currentEuler.z = 0;
            player.transform.eulerAngles = currentEuler;

            yield return null;
        }
        
        // 3. Lock final rotation on the PLAYER
        Vector3 finalEuler = end.eulerAngles;
        finalEuler.x = 0;
        finalEuler.z = 0;
        player.transform.eulerAngles = finalEuler;
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
            if (!powerSystem.isBaldiImprisoned)
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

            StartCoroutine(powerSystem.StunAllEnemies(5f));
        }
        else
        {
            hasGameEnded = true;
            Time.timeScale = 0f;
            uiiacatmusic.Stop();
            endScreeen.SetActive(true);

            if (!hasCheated)
            {
                endScore.text = "CLASS OVER\nSCORE: " + collectedisplay.score.ToString();
                UploadScore();
            }
            else 
                endScore.text = "CHEATS HAVE\nBEEN USED";

            gameOverSound.Play();
            musicManager.PlaySong(8);

        }
    }

    private async void UploadScore()
    {
        await LeaderboardManager.Instance.SubmitScoreForDifficulty(collectedisplay.score, SettingsData.Difficulty);
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
