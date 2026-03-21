using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public AudioSource backgroundSource;
    public BaldiEnemy baldiEnemy;
    public collectedisplay collectedisplay;
    public AudioClip[] songs; // assign all your songs here
    public bool isEnragedMusicOn = false;
    void Start()
    {
        UpdateBackgroundMusic();
    }

    public void UpdateBackgroundMusic()
    {
        int dif = 0;
        if (SettingsData.Difficulty == 3)
            dif = 4;

        // for rage mode music 
        if (baldiEnemy.isEnraged)
        {
            if (isEnragedMusicOn) return;

            PlaySong(3 +dif);
            isEnragedMusicOn = true;
            return;
        }

        // for round start music 
        if (collectedisplay.collected == 0)
        {
            PlaySong(dif);
        }

        // for 3 items
        if (collectedisplay.collected == 3)
        {
            PlaySong(1 + dif);
        }

        // for 5 items
        if (collectedisplay.collected == 5)
        {
            PlaySong(2 + dif);
        }

        return;
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

    public void PlaySong( int index)
    {
        backgroundSource.Stop();
        backgroundSource.clip = songs[index];
        backgroundSource.Play();
    }
}
