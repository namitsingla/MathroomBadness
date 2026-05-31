using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using  UnityEngine.Audio;

public class Collectible : MonoBehaviour
{
   public float rotationSpeed = 50f; // degrees per second
   public string poolTag;
   collectedisplay collecteddisplay;  
   BaldiWarningHide baldiWarning;
   MusicManager musicManager;
   DialogueSoundManager dialogueSoundManager;
   SpawnManager spawnManager;
   BoostsHandler boostsHandler;
   PowerSystem powerSystem;
   private bool hasBeenCollected = false;
    void Awake()
    {
        collecteddisplay = ReferencesManager.instance.collecteddisplay;
        baldiWarning = ReferencesManager.instance.baldiWarning;
        musicManager = ReferencesManager.instance.musicManager;
        dialogueSoundManager = ReferencesManager.instance.dialogueSoundManager;
        spawnManager = ReferencesManager.instance.spawnManager;
        boostsHandler = ReferencesManager.instance.boostsHandler;
        powerSystem = ReferencesManager.instance.powerSystem;

    }

    void Update()
    {
        // Smooth, frame-rate independent rotation
        transform.rotation = Quaternion.Euler(0, Time.time * rotationSpeed * 1.5f, 0);
    }

    void OnTriggerEnter(Collider other )
    {
        //Debug.Log("Item was touched by: " + other.gameObject.name + " | Tag: " + other.tag);

        if (!other.CompareTag("Player")) return;

        // If it's already been collected this frame, do nothing.
        if (hasBeenCollected) return;
        
        // Lock the gate immediately
        hasBeenCollected = true;


        //spawn next item
        if (spawnManager.spawnedCount < 1000)
        spawnManager.SpawnItem(); 

        int increaseAmount = 1;

        if (boostsHandler.isMitosisOn)
        {
            while (true)
            {
                int roll = Random.Range(0,100);
                if (roll<15) 
                    increaseAmount *= boostsHandler.mitosisMultiplier;
                else 
                    break;
            }
        }

        //increase item type count
            if (gameObject.CompareTag("Homework"))
            {
                collecteddisplay.homework += increaseAmount;
            }
            else if (gameObject.CompareTag("Chalk"))
            {
                collecteddisplay.chalk += increaseAmount;
            }



            //update the collected display
            collecteddisplay.collected = collecteddisplay.homework + collecteddisplay.chalk;
            collecteddisplay.UpdateDisplay();

            //activate exit door if collected = 3
            if (collecteddisplay.collected >= ExitDoor.requiredItems)
                ExitDoorManager.instance.ActivateAllExitDoors();

            dialogueSoundManager.PlayCollectSound();
            musicManager.StartPuaseBGMForCollectionText();
            ObjectPooler.instance.ReturnToPool(poolTag, gameObject);

            // display collected warning
            baldiWarning.ShowWarning();
            baldiWarning.WarningNumber(gameObject);
            

            // increase baldi's speed
            if (!powerSystem.isStunnerActive)
            EnemyManager.instance.GetEnemy<BaldiEnemy>().UpdateSpeed(collecteddisplay.collected);

            musicManager.UpdateBackgroundMusic();  
    }

    void OnEnable()
    {
        hasBeenCollected = false;
    }

}
