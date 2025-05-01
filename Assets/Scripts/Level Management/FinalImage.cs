using UnityEngine;

public class FinalImage : MonoBehaviour
{
    [SerializeField] private int musicToPlay;


    private void Start()
    {
        AudioManager.instance.PlayBackgroundMusic(musicToPlay);
        AudioManager.instance.prevMusic = musicToPlay; // set prevMusic
    }

    public void QuitGame()
    {
        // turn off app
        Debug.Log("We have quit the game.");
        Application.Quit();
    }

}