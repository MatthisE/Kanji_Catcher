using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string newGameScene;
    [SerializeField] GameObject continueButton;

    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        
    }

    // give these functions to the corresponding buttons
    public void NewGameButton()
    {
        SceneManager.LoadScene(newGameScene);
    }

    public void ExitButton()
    {
        Debug.Log("We just quit the game.");
    }

    public void ContinueButton()
    {
        SceneManager.LoadScene("LoadingScene"); // LoadingScene then loads saved data
    }
}
