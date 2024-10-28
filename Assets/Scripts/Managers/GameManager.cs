using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] PlayerStats[] playerStats;

    public bool gameMenuOpened, dialogBoxOpened; // conditions for player to stop moving

    // Start is called before the first frame update
    void Start()
    {
        //singelton pattern --> avoid duplicate GameManagers in new scenes
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);

        playerStats = FindObjectsOfType<PlayerStats>(); // add all objects that have a PlayerStats script to playerStats Array
        Array.Reverse(playerStats); // revert array because it adds object in the wrong order
    }

    // Update is called once per frame
    void Update()
    {
        // save quest data
        if(Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Data has been saved.");
            SaveData();
        }

        // load quest data
        if(Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("Data has been loaded.");
            LoadData();
        }

        // check if Player should stay still
        if(gameMenuOpened || dialogBoxOpened)
        {
            Player.instance.deactivateMovement = true;
        }else
        {
            Player.instance.deactivateMovement = false;
        }
    }

    public PlayerStats[] GetPlayerStats()
    {
        return playerStats;
    }

    public void SaveData()
    {
        SavingPlayerPosition();
        SavingPlayerStats();

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
            if(playerStats[i].gameObject.activeInHierarchy)
            {
                PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_active", 1);
            }
            else
            {
                PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_active", 0);
            }

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

    public void LoadData()
    {
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
