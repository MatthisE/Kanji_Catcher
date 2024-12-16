using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// given to main menu canvas object of main menu scene
public class MainMenu : MonoBehaviour
{
    [SerializeField] string newGameScene;
    [SerializeField] GameObject continueButton;

    [SerializeField] int musicToPlay;
    private bool musicAlreadyPlaying;

    void Start()
    {
        // check any key in player prefs to see if continue button should be displayed
        if(PlayerPrefs.HasKey("Player_Pos_X"))
        {
            continueButton.SetActive(true);
        }
        else
        {
            continueButton.SetActive(false);
        }
    }

    void Update()
    {
        // play background music
        if(!musicAlreadyPlaying)
            {
                musicAlreadyPlaying = true;
                AudioManager.instance.PlayBackgroundMusic(musicToPlay);
            }   
    }

    // functions for the menu buttons
    public void NewGameButton()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene(newGameScene);
    }

    public void ExitButton()
    {
        // turn off app
        Debug.Log("We have quit the game.");
        Application.Quit();
    }

    public void ContinueButton()
    {
        SceneManager.LoadScene("LoadingScene"); // LoadingScene then loads saved data
    }
}
