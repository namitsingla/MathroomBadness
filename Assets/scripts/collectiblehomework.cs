using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using  UnityEngine.Audio;

public class collectiblehomework : MonoBehaviour
{
   public float rotationSpeed = 50f; // degrees per second

   public collectedisplay collecteddisplay;  
   public AudioClip collectSound;
   public BaldiWarningHide baldiWarning;
    public AudioMixerGroup MasterMixer; //assign in inspector
    public MusicManager musicManager;

    public GameObject baldi;
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
            baldiWarning.ShowWarning();
            baldiWarning.WarningNumber(gameObject);
            

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

}
