using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// given to canvas object of game over scene
public class GameOverManager : MonoBehaviour
{
    [SerializeField] GameObject continueButton;
    
    void Start()
    {
        // play death music and turn off other objects
        AudioManager.instance.PlayBackgroundMusic(5);
        Player.instance.gameObject.SetActive(false);
        MenuManager.instance.gameObject.SetActive(false);
        BattleManager.instance.gameObject.SetActive(false);

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

    // functions for the menu buttons
    public void QuitToMainMenu()
    {
        DestroyGameSession();
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLastSave()
    {
        DestroyGameSession();
        SceneManager.LoadScene("LoadingScene");
    }

    private static void DestroyGameSession()
    {
        // destroy managers
        Destroy(GameManager.instance.gameObject);
        Destroy(Player.instance.gameObject);
        Destroy(MenuManager.instance.gameObject);
        Destroy(BattleManager.instance.gameObject);
    }

    public void QuitGame()
    {
        // turn off app
        Debug.Log("We have quit the game.");
        Application.Quit();
    }
}
