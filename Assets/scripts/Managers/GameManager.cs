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

    [Header("Spawn Coordinates")]
    [SerializeField] private Vector3 playerSpawn = new Vector3(40f, 3.1f, 100f);

    [Header("Death Sequence & Jumpscares")]
    [SerializeField] private float deathScreenTime = 2.5f;
    public RawImage baldiJumpscare;
    public RectTransform baldiJumpscarePosiiton;

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
    public AudioSource gameOverSound;

    public void KhelKhatam(BaseEnemy enemy, CatchType type)
    {
        if (powerSystem.isPowerDotOn) return;
        if (isDead) return;

        if (powerSystem.isPrisonRealmActive)
        {
            powerSystem.EnemyCaughtInPrison(enemy);
            return;
        }

        if (boostsHandler.isShukakuActive)
        {
            StartCoroutine(boostsHandler.ShukakuProtection(type));
            return;
        }

        lives -= 1;
        isDead = true;

        movement.enabled = false;
        player.GetComponent<CharacterController>().enabled = false;

        if (type == CatchType.baldi)
            StartCoroutine(BaldiDeathSequence(enemy.transform));
        else if (type == CatchType.uiiacat)
            StartCoroutine(UiiaCatDeathSequence(enemy.transform));
        else if (type == CatchType.oggy)
            StartCoroutine(OggyDeathSequence(enemy.transform));
    }

    IEnumerator BaldiDeathSequence(Transform lookTarget)
    {
        yield return StartCoroutine(RotateTowards(lookTarget.position, 0.1f));

        // move baldi out of view instead of deactivating
        BaldiEnemy baldi = EnemyManager.instance.GetEnemy<BaldiEnemy>();
        if (baldi != null) baldi.agent.Warp(new Vector3(0f, -100f, 0f));

        BGM.Pause();
        dialogueSoundManager.PlayBaldiDeathScreenSound();

        float x = Random.Range(-350f, 600f);
        float y = Random.Range(-1000f, -1300f);
        baldiJumpscarePosiiton.anchoredPosition = new Vector2(x, y);
        baldiJumpscare.enabled = true;
        deathScreenTime = 2.5f;

        StartCoroutine(FinishDeathSequence());
    }

    IEnumerator UiiaCatDeathSequence(Transform lookTarget)
    {
        yield return StartCoroutine(RotateTowards(lookTarget.position, 0.1f));
        BGM.Pause();
        deathScreenTime = 1f;
        StartCoroutine(FinishDeathSequence());
    }

    IEnumerator OggyDeathSequence(Transform lookTarget)
    {
        yield return StartCoroutine(RotateTowards(lookTarget.position, 0.1f));
        BGM.Pause();
        deathScreenTime = 1f;
        StartCoroutine(FinishDeathSequence());
    }

    IEnumerator RotateTowards(Vector3 target, float duration)
    {
        Vector3 flatTarget = new Vector3(target.x, player.transform.position.y, target.z);
        Vector3 dir = (flatTarget - player.transform.position).normalized;
        if (dir == Vector3.zero) yield break;

        Quaternion start = player.transform.rotation;
        Quaternion end = Quaternion.LookRotation(dir);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            player.transform.rotation = Quaternion.Slerp(start, end, t);

            Vector3 currentEuler = player.transform.eulerAngles;
            currentEuler.x = 0;
            currentEuler.z = 0;
            player.transform.eulerAngles = currentEuler;

            yield return null;
        }

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

            BGM.UnPause();
            baldiWarningcanvas.SetActive(false);

            // respawn all enemies via EnemyManager/SpawnManager
            spawnManager.RespawnEnemiesInPlace();

            // reset player position
            spawnManager.TeleportPlayerAway();

            StartCoroutine(powerSystem.StunAllEnemies(5f));
        }
        else
        {
            hasGameEnded = true;
            Time.timeScale = 0f;
        
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
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(0);
        }
    }
}