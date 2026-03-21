using UnityEngine;
using System.Collections;


public class DialogueSoundManager : MonoBehaviour
{
   public AudioSource audioSource;
   public AudioClip[] dialogues;
   public AudioSource BGM;

   public IEnumerator PlayBaldiFirstDetection()
    {

        audioSource.Stop();
        audioSource.clip = dialogues[0];
        audioSource.Play();

        //since audioclip has some empty parts in beginning
        yield return new WaitForSeconds(0.5f); 
        BGM.Pause();
        Debug.Log("BGM Puased");

        yield return new WaitForSeconds(4f); 
        BGM.UnPause();
        Debug.Log("BGM Unpaused.");
    }

    public void PlayBaldiDeathScreenSound()
    {
        audioSource.Stop();
        audioSource.clip = dialogues[1];
        audioSource.Play();
    }

    public void PlayCollectSound()
    {
        audioSource.Stop();
        audioSource.clip = dialogues[2];
        audioSource.Play();
    }
}
