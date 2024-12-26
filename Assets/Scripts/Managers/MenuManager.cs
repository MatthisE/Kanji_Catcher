using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

// given to canvas object
public class MenuManager : MonoBehaviour
{
    [SerializeField] Image imageToFade;
    [SerializeField] GameObject menu;

    [SerializeField] GameObject[] statsButtons;

    public static MenuManager instance;

    private PlayerStats[] playerStats;
    private KanjiManager[] collectedKanji;

    [SerializeField] TextMeshProUGUI[] xpText;
    [SerializeField] Slider[] xpSlider;
    [SerializeField] Sprite[] characterImage;
    [SerializeField] GameObject[] characterPanel;

    [SerializeField] TextMeshProUGUI statName, statHP, statMana;
    [SerializeField] Image characterSatImage;

    [SerializeField] GameObject itemSlotContainer; // items button prefab
    [SerializeField] Transform itemSlotContainerParent; // items display panel

    [SerializeField] GameObject itemsPanel;
    [SerializeField] GameObject statusPanel;

    public TextMeshProUGUI itemName, itemDescription;
    public ItemsManager activeItem;


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
                // close menu
                menu.SetActive(false);
                GameManager.instance.gameMenuOpened = false;
            }
            else
            {
                if(GameManager.instance.dialogBoxOpened != true && GameManager.instance.battleIsActive != true && GameManager.instance.goThroughExit != true){ // don't open if one of these are true
                    // open menu
                    itemsPanel.SetActive(false);
                    statusPanel.SetActive(false);

                    UpdateStats(); // display all current characters with stats
                    menu.SetActive(true);
                    GameManager.instance.gameMenuOpened = true; //set condition for player to stop moving
                }
            }
        }
    }

    // when opening menu, overview of stats should be displayed (different from stats menu)
    public void UpdateStats()
    {
        playerStats = GameManager.instance.GetPlayerStats();

        collectedKanji = playerStats[0].collectedKanji;

        // go through all collected kanji
        for(int i = 0; i < collectedKanji.Length; i++)
        {
            // make panel for this character visible in menu
            characterPanel[i].SetActive(true);

            // fill panel with info of this character from their kanjiManager object
            characterImage[i] = collectedKanji[i].kanjiImage;

            xpText[i].text = collectedKanji[i].currentXP.ToString() + "/" + 100;
            xpSlider[i].maxValue = 100; // max value of XP slider --> needed XP
            xpSlider[i].value = collectedKanji[i].currentXP;
        }

    }

    // when opening stats menu
    public void StatsMenu()
    {
        // go through all current players (aka their stats)
        for(int i = 0; i < playerStats.Length; i++)
        {
            // make a button for each of them
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

    // on click on items button:
    public void UpdateItemsInventory()
    {
        //destroy all previous item slots to not have doubles
        foreach(Transform itemSlot in itemSlotContainerParent)
        {
            Destroy(itemSlot.gameObject);
        }

        foreach(ItemsManager item in Inventory.instance.GetItemsList())
        {
            // make each item in inventory a slot in items menu
            RectTransform itemSlot = Instantiate(itemSlotContainer, itemSlotContainerParent).GetComponent<RectTransform>(); // Instantiate --> makes first value a child of the second value, then get that slot as a value

            Image itemImage = itemSlot.Find("Items Image").GetComponent<Image>(); // get the image of the slot
            itemImage.sprite = item.itemsImage; // set the image of the slot to the image of item in inventory

            // set the amount text to a number if the amount is bigger than 1
            TextMeshProUGUI itemsAmountText = itemSlot.Find("Amount Text").GetComponent<TextMeshProUGUI>();
            if(item.amount > 1)
            {
                itemsAmountText.text = item.amount.ToString();
            }
            else
            {
                itemsAmountText.text = "";
            }

            // set item of item button (script given to botton component of item slot object)
            itemSlot.GetComponent<ItemButton>().itemOnButton = item;
        }
    }

    // remove item from inventory and reload item menu
    public void DiscardItem()
    {
        Inventory.instance.RemoveItem(activeItem);
        UpdateItemsInventory();
        AudioManager.instance.PlaySFX(3);
    }

    // use item, remove item from inventory and reload item menu
    public void UseItem()
    {
        activeItem.UseItem();
        
        Inventory.instance.RemoveItem(activeItem);
        UpdateItemsInventory();
        AudioManager.instance.PlaySFX(8);
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
        GameManager.instance.gameMenuOpened = false; // make player movable
    }

    public void QuitGame()
    {
        // turn off app
        Debug.Log("We have quit the game.");
        Application.Quit();
    }

    // activate trigger for animator to fade
    public void FadeImage()
    {
        imageToFade.GetComponent<Animator>().SetTrigger("Start Fading"); // --> trigger in animator for image
    }

    public void FadeOut()
    {
        imageToFade.GetComponent<Animator>().SetTrigger("End Fading"); // --> trigger in animator for image
    }
}
