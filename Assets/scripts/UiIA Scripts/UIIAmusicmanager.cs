using UnityEngine;

public class UIIAmusicmanager : MonoBehaviour
{
    public AudioSource audioSource;      // drag your AudioSource here
    public AudioClip[] songs;            // assign your songs in the Inspector
    public PauseMenu pauseMenu;

    public int lastSongIndex = -1;      // to avoid repeats

    public void Start()
    {
        PlayRandomSong();
    }
    public void Update()
    {
        if (!PauseMenu.GameIsPaused)
        {
        // when current song finishes, play another random one
        if (!audioSource.isPlaying)
        {
            PlayRandomSong();
        }
    }
    }
    
    public void PlayRandomSong()
    {
        if (songs.Length == 0) return;

        int newIndex;
        do
        {
            newIndex = Random.Range(0, songs.Length);
        }
        while (newIndex == lastSongIndex && songs.Length > 1); // avoid repeat

        lastSongIndex = newIndex;
        audioSource.clip = songs[newIndex];
        audioSource.Play();
    }
}
