using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] songs; // assign all your songs here

    void Start()
    {
        if (songs.Length > 0)
            PlaySong(0); // start with the first song
    }

    public void PlaySong(int index)
    {
        if (index < 0 || index >= songs.Length) return;

        audioSource.Stop();
        audioSource.clip = songs[index];
        audioSource.Play();
    }
}
