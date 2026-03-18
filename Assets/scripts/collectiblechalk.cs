using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.AI;
using  UnityEngine.Audio;

public class collectiblechalk : MonoBehaviour
{
   public float rotationSpeed = 50f; // degrees per second

   public collectedisplay collecteddisplay;  
   public AudioClip collectSound;
   public BaldiWarningHide baldiWarning;
   public TextMeshProUGUI messageText;

    public GameObject targetObject; // assign in Inspector
     public AudioMixerGroup MasterMixer; //assign in inspector
    private NavMeshAgent targetAgent;
    public MusicManager musicManager;

     void Start()
    {
        if (targetObject != null)
            targetAgent = targetObject.GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Smooth, frame-rate independent rotation
        transform.rotation = Quaternion.Euler(0, Time.time * rotationSpeed * 1.5f, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        collecteddisplay.chalk += 1;

        collecteddisplay.collected = collecteddisplay.homework + collecteddisplay.chalk;

        // Create a temporary AudioSource for 2D playback
        GameObject tempGO = new GameObject("TempAudio");
        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = collectSound;
        aSource.spatialBlend = 0f; // 0 = 2D, 1 = 3D
        aSource.outputAudioMixerGroup = MasterMixer; //to use audio settings
        aSource.Play();

        Destroy(tempGO, collectSound.length);

        WarningNumber();
        baldiWarning.ShowWarning();

        targetAgent.speed = 70 + collecteddisplay.collected;

        Destroy(gameObject);
                
    }

    public void WarningNumber ()
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
                messageText.fontSize = 150; 

                GameObject.Find("uiia cat").GetComponent<EnemyController>().lookRadius = 500f;
                GameObject.Find("oggy").GetComponent<EnemyController>().lookRadius = 500f;          
            }

    }

}
