using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// given to audio manager object
public class AudioManager : MonoBehaviour
{
    // background music of a scene is set by its virtual camera
    [SerializeField] AudioSource[] SFX, backgroundMusic;

    public int prevMusic; // previous music needs to be safed in global object, current music in local object (camera)

    public static AudioManager instance;

    void Start()
    {
        //singelton pattern --> avoid duplicate Audio Managers in new scenes
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    // reference sound to play by its index in the AudioSorce array
    public void PlaySFX(int soundToPlay)
    {
        if(soundToPlay < SFX.Length)
        {
            SFX[soundToPlay].Play();
        }
    }

    public void PlayBackgroundMusic(int musicToPlay)
    {
        StopMusic();

        if(musicToPlay < backgroundMusic.Length)
        {
            backgroundMusic[musicToPlay].Play();
        }
    }

    // stop every background music (before activating a new one)
    private void StopMusic()
    {
        foreach(AudioSource song in backgroundMusic)
        {
            song.Stop();
        }
    }
}
