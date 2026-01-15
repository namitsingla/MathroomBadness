using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.AI;
using  UnityEngine.Audio;

public class collectiblehomework : MonoBehaviour
{
   public float rotationSpeed = 50f; // degrees per second

   public collectedisplay collecteddisplay;  
   public AudioClip collectSound;
   public BaldiWarningHide baldiWarning;
   public TextMeshProUGUI messageText;
    public AudioMixerGroup MasterMixer; //assign in inspector
    public MusicManager musicManager;

    public GameObject baldi;
    public GameObject uiiacat;
    public GameObject oggy;


     void Start()
    {

    }

    void Update()
    {
        // Smooth, frame-rate independent rotation
        transform.rotation = Quaternion.Euler(0, Time.time * rotationSpeed * 1.5f, 0);
    }

    void OnTriggerEnter(Collider other )
    {
        //increase item type count
        if (other.GetComponent<Collider>().CompareTag("Player"))
        {
            if (gameObject.CompareTag("Homework"))
            {
                collecteddisplay.homework += 1;
            }
            else if (gameObject.CompareTag("Chalk"))
            {
                collecteddisplay.chalk += 1;
            }



            //update the collected display
            collecteddisplay.collected = collecteddisplay.homework + collecteddisplay.chalk;

            // Create a temporary AudioSource for 2D playback
            GameObject tempGO = new GameObject("TempAudio");
            AudioSource aSource = tempGO.AddComponent<AudioSource>();
            aSource.clip = collectSound;
            aSource.spatialBlend = 0f; // 0 = 2D, 1 = 3D
            aSource.outputAudioMixerGroup = MasterMixer; //to use audio settings
            aSource.Play();

            Destroy(tempGO, collectSound.length);

            Destroy(gameObject);

            // display collected warning
            WarningNumber();
            baldiWarning.ShowWarning();

            // sincrease baldi's speed
            baldi.GetComponent<NavMeshAgent>().speed = 70 + collecteddisplay.collected*10;



            // Change background music
            if (collecteddisplay.collected == 3)
            {
                musicManager.PlaySong(1); // play song 2 after 3 items
            }
            else if (collecteddisplay.collected == 6)
            {
                musicManager.PlaySong(2); // play song 3 after 6 items
            }
        }        
    }

    public void WarningNumber()
    {
        if (gameObject.CompareTag("Homework"))
        {
            if (collecteddisplay.homework == 1)
            {
                messageText.text = "Baldi is fuming - that was his best doodle";
                messageText.fontSize = 60;
            }

            if (collecteddisplay.homework == 2)
            {
                messageText.text = "“Another unfinished assignment.... you'll never finish them all!”";
                messageText.fontSize = 50;
            }

            if (collecteddisplay.homework == 3)
            {
                messageText.text = "*strikes*";
                messageText.fontSize = 130;

                baldi.GetComponent<BaldiEnemy>().lookRadius = 500f;
                baldi.GetComponent<BaldiEnemy>().isEnraged = true;
            }
        }
        else if (gameObject.CompareTag("Chalk"))
        {
            if (collecteddisplay.chalk == 1)
            {
                 messageText.text = "One less chalk stick for Baldi! His slaps grow louder…";
                 messageText.fontSize = 55;
            }

            if (collecteddisplay.chalk == 2)
            {
                messageText.text = "Snap! Baldi can’t write now, but he sure can chase.";
                messageText.fontSize = 60;           
            }

            if (collecteddisplay.chalk == 3)
            {
                messageText.text = "*meow*";
                messageText.fontSize = 130;

                uiiacat.GetComponent<UIIAController>().lookRadius = 500f;
                oggy.GetComponent<EnemyController>().lookRadius = 500f;          
                uiiacat.GetComponent<UIIAController>().isEnraged = true;
                oggy.GetComponent<EnemyController>().isEnraged = true; 
                uiiacat.GetComponent<UIIAController>().ActivateAllWalls();
            }
        }

    }

}
