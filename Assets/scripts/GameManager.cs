using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int lives = 3;

    public GameObject player;
    public GameObject baldi;
    public GameObject uiiacat;
    public GameObject oggy;

    Vector3 playerspawn = new Vector3(40f, 2.1f, 100f);
    Vector3 baldispawn = new Vector3(135f, 2.5f, -22.5f);
    Vector3 uiiacatspawn = new Vector3(-110f, 2f, 100f);
    Vector3 oggyspawn = new Vector3(-10f, 2f, -70f);

    private bool hitCooldown = false; // prevent double hits
    public float cooldownTime = 1f; //buffer

    public void KhelKhatam()
    {
        if (lives > 1 && !hitCooldown)
        {
            hitCooldown = true;
            Invoke(nameof(ResetHitCooldown), cooldownTime);

            lives -= 1;

            baldi.transform.position = baldispawn;
            uiiacat.transform.position = uiiacatspawn;
            oggy.transform.position = oggyspawn;

            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = playerspawn;
            player.GetComponent<CharacterController>().enabled = true;

            Debug.Log("ouch");
        }
        else if (!hitCooldown)
        {
            Cursor.lockState = CursorLockMode.None; // Unlocks the cursor
            SceneManager.LoadScene(0);

        }
    }
    
    private void ResetHitCooldown()
    {
        hitCooldown = false;
    }
}
