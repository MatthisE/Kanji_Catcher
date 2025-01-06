using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

// given to game manager object
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] PlayerStats[] playerStats;

    public bool gameMenuOpened, dialogBoxOpened, battleIsActive, goThroughExit; // conditions for player to stop moving

    void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        playerStats = FindObjectsOfType<PlayerStats>(); // Initialize player stats
        Array.Reverse(playerStats); // Revert array
    }

    void Start()
    {
        ItemsManager itemToAdd = ItemsAssets.instance.GetItemAsset("Cursed Kanji Book");
        Inventory.instance.AddItems(itemToAdd); // add item to loaded inventory

        StartCoroutine(StartText());
    }

    IEnumerator StartText()
    {
        yield return new WaitForSeconds(1.0f); // wait before next letter

        ActionButton.instance.SetActiveState(false);
        string[] sentences = {"All but four kanji flew out of the kanji book.", "Be sure to study the ones you have, it might come in handy."};
        DialogController.instance.ActivateDialog(sentences); // open box with first sentence
        DialogController.instance.ActivateQuestAtEnd("Start Game", true); // activate quest after dialog
    }

    void Update()
    {
        // save data (quest data is saved in quest manager)
        if(Input.GetKeyDown(KeyCode.I))
        {
            SaveData();
        }

        // load data (quest data is loaded in quest manager)
        if(Input.GetKeyDown(KeyCode.O))
        {
            LoadData();
        }

        // check if Player should stay still
        if(gameMenuOpened || dialogBoxOpened || battleIsActive || goThroughExit)
        {
            Player.instance.deactivateMovement = true;
            Joystick.ActivateJoystick(false);
            Joystick.instance.ResetJoystick();
        }else
        {
            Player.instance.deactivateMovement = false;
            Joystick.ActivateJoystick(true);
        }
    }

    public PlayerStats[] GetPlayerStats()
    {
        return playerStats;
    }

    public KanjiManager[] GetCollectedKanji()
    {
        PlayerStats[] playerStats = GetPlayerStats();
        return playerStats[0].collectedKanji;
    }

    // save data (scene, items, player position, player stats)
    public void SaveData()
    {
        AudioManager.instance.PlaySFX(5);

        // fill player stats again because this function is given to save button and it does not wait for Start()
        playerStats = FindObjectsOfType<PlayerStats>(); // add all objects that have a PlayerStats script to playerStats Array
        Array.Reverse(playerStats); // revert array because it adds object in the wrong order

        SavingPlayerPosition();
        SavingPlayerStats();

        // save current scene
        PlayerPrefs.SetString("Current_Scene", SceneManager.GetActiveScene().name);

        PlayerPrefs.SetInt("Number_Of_Items", Inventory.instance.GetItemsList().Count); // save inventory length
        for(int i = 0; i < Inventory.instance.GetItemsList().Count; i++)
        {
            ItemsManager itemInInventory = Inventory.instance.GetItemsList()[i];
            PlayerPrefs.SetString("Item_" + i + "_Name", itemInInventory.itemName); // save item name

            if(itemInInventory.isStackable)
            {
                PlayerPrefs.SetInt("Items_" + i + "_Name", itemInInventory.amount); // save item amount
            }
        }
    }

    void PrintPlayerPrefs()
    {
        // Get the total number of PlayerPrefs entries
        int count = PlayerPrefs.GetInt("PlayerPrefsCount", 0);

        // Print each PlayerPref key and its value
        for (int i = 0; i < count; i++)
        {
            string key = PlayerPrefs.GetString("PlayerPrefsKey_" + i, null);
            if (key != null)
            {
                if (PlayerPrefs.HasKey(key))
                {
                    string value = PlayerPrefs.GetString(key);  // Change this if you're saving other types
                    Debug.Log(key + ": " + value);
                }
            }
        }
    }

    private void SavingPlayerPosition()
    {
        PlayerPrefs.SetFloat("Player_Pos_X", Player.instance.transform.position.x);
        PlayerPrefs.SetFloat("Player_Pos_Y", Player.instance.transform.position.y);
        PlayerPrefs.SetFloat("Player_Pos_Z", Player.instance.transform.position.z);
    }

    private void SavingPlayerStats()
    {
        for(int i = 0; i < playerStats.Length; i++)
        {
            // players' active statuses
            if(playerStats[i].gameObject.activeInHierarchy)
            {
                PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_active", 1);
            }
            else
            {
                PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_active", 0);
            }

            Debug.Log(PlayerPrefs.GetString("Player_Jimmy_active"));

            // players' stats
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_Level", playerStats[i].playerLevel);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_CurrentXP", playerStats[i].currentXP);

            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_MaxHP", playerStats[i].maxHP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_CurrentHP", playerStats[i].currentHP);

            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_MaxMana", playerStats[i].maxMana);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_CurrentMana", playerStats[i].currentMana);

            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_Dexterity", playerStats[i].dexterity);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_Defence", playerStats[i].defence);
        }
    }

    // load data (items, player position, player stats), scene is loaded by LoadingScene.cs
    public void LoadData()
    {
        Debug.Log("Data has been loaded.");

        LoadingPlayerPosition();
        LoadingPlayerStats();
        
        for(int i = 0; i < PlayerPrefs.GetInt("Number_Of_Items"); i++) // go through inventory positions
        {
            string itemName = PlayerPrefs.GetString("Item_" + i + "_Name"); // get item for each position
            ItemsManager itemToAdd = ItemsAssets.instance.GetItemAsset(itemName);

            int itemAmount = 0;
            if(PlayerPrefs.HasKey("Items_" + i + "_Amount")) // if item has amount
            {
                itemAmount = PlayerPrefs.GetInt("Items_" + i + "_Amount"); // set item amount
            }

            Inventory.instance.AddItems(itemToAdd); // add item to loaded inventory
            if(itemToAdd.isStackable && itemAmount > 1)
            {
                itemToAdd.amount = itemAmount; // if it has amount bigger than 1, add it
            }
        }
    }

    private void LoadingPlayerPosition()
    {
        Player.instance.transform.position = new Vector3(
            PlayerPrefs.GetFloat("Player_Pos_X"),
            PlayerPrefs.GetFloat("Player_Pos_Y"),
            PlayerPrefs.GetFloat("Player_Pos_Z")
        );
    }

    private void LoadingPlayerStats()
    {
        for(int i = 0; i < playerStats.Length; i++)
        {
            if(PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_active") == 0)
            {
                playerStats[i].gameObject.SetActive(false);
            }
            else
            {
                playerStats[i].gameObject.SetActive(true);
            }

            playerStats[i].playerLevel = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_Level");
            playerStats[i].currentXP = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_CurrentXP");

            playerStats[i].maxHP = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_MaxHP");
            playerStats[i].currentHP = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_CurrentHP");

            playerStats[i].maxMana = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_MaxMana");
            playerStats[i].currentMana = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_CurrentMana");

            playerStats[i].dexterity = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_Dexterity");
            playerStats[i].defence = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_Defence");
        }
    }
}
