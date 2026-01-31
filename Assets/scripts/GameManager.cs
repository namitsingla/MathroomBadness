using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public enum CatchType
    {
        baldi,
        uiiacat,
        oggy
    }
public class GameManager : MonoBehaviour
{
    public int lives = 3;

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
    public void KhelKhatam(Transform lookTarget, CatchType type)
    {
        if (isDead) return;

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
        float x = Random.Range(-150f, 250f);
        float y = Random.Range(-500f, -600f);

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
            baldi.SetActive(true);
            BGM.UnPause();

            //baldi.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(baldispawn);
            //baldi.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(baldispawn);

            //uiiacat.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(uiiacatspawn);
            //uiiacat.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(uiiacatspawn);

            //oggy.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(oggyspawn);
            //oggy.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(oggyspawn);
            
            spawnManager.SpawnAllEnemies();
            
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = playerspawn;
            player.GetComponent<CharacterController>().enabled = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None; // Unlocks the cursor for menus
            SceneManager.LoadScene(0);
        }
    }
}
