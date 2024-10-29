using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // background music of a scene is set by its virtual camera
    [SerializeField] AudioSource[] SFX, backgroundMusic;

    public static AudioManager instance;

    // Start is called before the first frame update
    void Start()
    {
        //singelton pattern --> avoid duplicate Players in new scenes
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

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            PlayBackgroundMusic(5);
        }
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
