using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public AudioSource backgroundSource;
    public AudioClip[] songs; // assign all your songs here
    void Start()
    {
        if (songs.Length > 0)
            PlaySong(0); // start with the first song
    }

    public void PlaySong(int index)
    {
        if (index < 0 || index >= songs.Length) return;

        backgroundSource.Stop();
        backgroundSource.clip = songs[index];
        backgroundSource.Play();
    }

    public IEnumerator PuaseBGMForCollectionText()
    {
        backgroundSource.Pause();

        yield return new WaitForSeconds(3f);
        backgroundSource.UnPause(); 
    }

    public void StartPuaseBGMForCollectionText()
    {
        StartCoroutine(PuaseBGMForCollectionText());
    }
}
