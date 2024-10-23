using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Image imageToFade;
    [SerializeField] GameObject menu;

    public static MenuManager instance;

    private void Start()
    {
        instance = this; // no singelton pattern needed
    }

    private void Update()
    {
        // open and close menu with M key
        if(Input.GetKeyDown(KeyCode.M))
        {
            if(menu.activeInHierarchy)
            {
                menu.SetActive(false);
                GameManager.instance.gameMenuOpened = false;
            }
            else
            {
                menu.SetActive(true);
                GameManager.instance.gameMenuOpened = true; //set condition for player to stop moving
            }
        }
    }

    // activate trigger for animator to fade
    public void FadeImage()
    {
        imageToFade.GetComponent<Animator>().SetTrigger("Start Fading");
    }
}
