using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Image imageToFade;
    [SerializeField] GameObject menu;

    [SerializeField] GameObject[] statsButtons;

    public static MenuManager instance;

    private PlayerStats[] playerStats;
    [SerializeField] TextMeshProUGUI[] nameText, hpText, manaText, currentXPText, xpText;
    [SerializeField] Slider[] xpSlider;
    [SerializeField] Image[] characterImage;
    [SerializeField] GameObject[] characterPanel;

    [SerializeField] TextMeshProUGUI statName, statHP, statMana;
    [SerializeField] Image characterSatImage;


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
                UpdateStats(); // display all current characters with stats
                menu.SetActive(true);
                GameManager.instance.gameMenuOpened = true; //set condition for player to stop moving
            }
        }
    }

    public void UpdateStats()
    {
        playerStats = GameManager.instance.GetPlayerStats();

        for(int i = 0; i < playerStats.Length; i++)
        {
            // make player sets of this object visible in menu
            characterPanel[i].SetActive(true);

            // set the player sets for every object that has them
            characterImage[i].sprite = playerStats[i].characterImage;

            nameText[i].text = playerStats[i].playerName;
            hpText[i].text = "HP: " + playerStats[i].currentHP + "/" + playerStats[i].maxHP;
            manaText[i].text = "Mana: " + playerStats[i].currentMana + "/" + playerStats[i].maxMana;
            currentXPText[i].text = "Current XP: " + playerStats[i].currentXP;

            xpText[i].text = playerStats[i].currentXP.ToString() + "/" + playerStats[i].xpForNextLevel[playerStats[i].playerLevel];
            xpSlider[i].maxValue = playerStats[i].xpForNextLevel[playerStats[i].playerLevel]; // max value of XP slider --> needed XP for next level
            xpSlider[i].value = playerStats[i].currentXP;
        }
    }

    // when opening stats menu
    public void StatsMenu()
    {
        for(int i = 0; i < playerStats.Length; i++)
        {
            statsButtons[i].SetActive(true);

            statsButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = playerStats[i].playerName; // set text in buttons to player names
        }

        StatsMenuUpdate(0);
    }

    // set values of stats menu to those of selected player (referenced by index (playerSelectedNumber))
    public void StatsMenuUpdate(int playerSelectedNumber)
    {
        PlayerStats playerSelected = playerStats[playerSelectedNumber];

        statName.text = playerSelected.playerName;
        statHP.text = playerSelected.currentHP.ToString() + "/" + playerSelected.maxHP.ToString();
        statMana.text =  playerSelected.currentMana.ToString() + "/" + playerSelected.maxMana.ToString();

        characterSatImage.sprite = playerSelected.characterImage;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("We have quit the game.");
    }

    // activate trigger for animator to fade
    public void FadeImage()
    {
        imageToFade.GetComponent<Animator>().SetTrigger("Start Fading");
    }
}
