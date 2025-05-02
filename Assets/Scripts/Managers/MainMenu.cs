using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// given to main menu canvas object of main menu scene
public class MainMenu : MonoBehaviour
{
    [SerializeField] private Image imageToFade;
    [SerializeField] private string newGameScene;
    [SerializeField] private GameObject continueButton;

    [SerializeField] private int musicToPlay;
    private bool musicAlreadyPlaying;

    private void Start()
    {
        // check any key in player prefs to see if continue button should be displayed
        if (PlayerPrefs.HasKey("Player_Pos_X"))
        {
            //continueButton.SetActive(true);
        }
        else
        {
            continueButton.SetActive(false);
        }
    }

    private void Update()
    {
        // play background music
        if (!musicAlreadyPlaying)
        {
            musicAlreadyPlaying = true;
            AudioManager.instance.PlayBackgroundMusic(musicToPlay);
            AudioManager.instance.prevMusic = musicToPlay; // set prevMusic
        }
    }

    // functions for the menu buttons
    public void NewGameButton()
    {
        _ = StartCoroutine(StartNewGame());
    }

    public IEnumerator StartNewGame()
    {
        AudioManager.instance.PlaySFX(8);
        imageToFade.GetComponent<Animator>().SetTrigger("Start Fading"); // --> trigger in animator for image
        yield return new WaitForSeconds(0.5f);

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene(newGameScene);
    }

    public void ExitButton()
    {
        // turn off app
        Application.Quit();
    }

    public void ContinueButton()
    {
        AudioManager.instance.PlaySFX(8);
        SceneManager.LoadScene("LoadingScene"); // LoadingScene then loads saved data
    }
}