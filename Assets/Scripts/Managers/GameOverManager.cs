using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// given to canvas object of game over scene
public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject continueButton;

    [SerializeField] private int musicToPlay;

    private void Start()
    {
        // play death music and turn off other objects
        AudioManager.instance.PlayBackgroundMusic(musicToPlay);
        AudioManager.instance.prevMusic = musicToPlay; // set prevMusic
        Player.instance.gameObject.SetActive(false);
        MenuManager.instance.gameObject.SetActive(false);
        BattleManager.Instance.gameObject.SetActive(false);

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

    // functions for the menu buttons
    public void QuitToMainMenu()
    {
        _ = StartCoroutine(Transition());
    }

    public IEnumerator Transition()
    {
        AudioManager.instance.PlaySFX(8);
        MenuManager.instance.FadeImage();
        yield return new WaitForSeconds(0.4f);

        DestroyGameSession();
        SceneManager.LoadScene("MainMenu");

    }

    public void LoadLastSave()
    {
        AudioManager.instance.PlaySFX(8);
        DestroyGameSession();
        SceneManager.LoadScene("LoadingScene");
    }

    private static void DestroyGameSession()
    {
        // destroy managers
        Destroy(GameManager.instance.gameObject);
        Destroy(Player.instance.gameObject);
        Destroy(MenuManager.instance.gameObject);
        Destroy(BattleManager.Instance.gameObject);
    }

    public void QuitGame()
    {
        // turn off app
        Application.Quit();
    }
}