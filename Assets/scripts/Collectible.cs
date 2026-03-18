using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using  UnityEngine.Audio;

public class Collectible : MonoBehaviour
{
   public float rotationSpeed = 50f; // degrees per second
   collectedisplay collecteddisplay;  
   BaldiWarningHide baldiWarning;
   MusicManager musicManager;
   DialogueSoundManager dialogueSoundManager;
   SpawnManager spawnManager;
   GameObject baldi;
   BaldiEnemy baldiEnemy;
   ExitDoor exitDoor;
   private bool hasBeenCollected = false;
    void Awake()
    {
        collecteddisplay = ReferencesManager.instance.collecteddisplay;
        baldiWarning = ReferencesManager.instance.baldiWarning;
        musicManager = ReferencesManager.instance.musicManager;
        dialogueSoundManager = ReferencesManager.instance.dialogueSoundManager;
        baldi = ReferencesManager.instance.baldi;
        spawnManager = ReferencesManager.instance.spawnManager;
        baldiEnemy = ReferencesManager.instance.baldiEnemy;
        exitDoor = ReferencesManager.instance.exitDoor;

    }

    void Update()
    {
        // Smooth, frame-rate independent rotation
        transform.rotation = Quaternion.Euler(0, Time.time * rotationSpeed * 1.5f, 0);
    }

    void OnTriggerEnter(Collider other )
    {
        if (!other.CompareTag("Player")) return;

        // If it's already been collected this frame, do nothing.
        if (hasBeenCollected) return;
        
        // Lock the gate immediately
        hasBeenCollected = true;


        //spawn next item
        if (spawnManager.spawnedCount < 1000)
        spawnManager.SpawnItem(); 

        //increase item type count
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
            collecteddisplay.UpdateDisplay();

            //activate exit door if collected = 3
            if (collecteddisplay.collected == 3) exitDoor.ActivateExitDoor();

            dialogueSoundManager.PlayCollectSound();
            musicManager.StartPuaseBGMForCollectionText();
            Destroy(gameObject);

            // display collected warning
            baldiWarning.ShowWarning();
            baldiWarning.WarningNumber(gameObject);
            

            // sincrease baldi's speed
            baldi.GetComponent<NavMeshAgent>().speed = baldiEnemy.baldiBaseSpeed + baldiEnemy.speedIncrease*collecteddisplay.collected;

            musicManager.UpdateBackgroundMusic();  
    }

}
