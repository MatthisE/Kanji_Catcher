using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEngine.SceneManagement;
//using UnityEngine.UIElements;

// given to canvas object
public class MenuManager : MonoBehaviour
{
    [SerializeField] Image imageToFade;
    [SerializeField] GameObject menu;

    [SerializeField] GameObject[] statsButtons;

    public static MenuManager instance;

    [SerializeField] TextMeshProUGUI[] xpText;
    [SerializeField] Slider[] xpSlider;
    [SerializeField] Image[] characterImage;
    [SerializeField] Button[] detailsButtons;
    [SerializeField] GameObject[] characterPanel;

    [SerializeField] TextMeshProUGUI statName, statHP, statMana;
    [SerializeField] Image characterSatImage;

    [SerializeField] GameObject itemSlotContainer; // items button prefab
    [SerializeField] Transform itemSlotContainerParent; // items display panel

    [SerializeField] GameObject itemsPanel;
    [SerializeField] GameObject itemsDescription;
    [SerializeField] GameObject kanjiPanel;
    [SerializeField] Scrollbar scrollbar;
    [SerializeField] GameObject kanjiInfoPage;

    public TextMeshProUGUI itemName, itemDescription;
    public ItemsManager activeItem;
    private bool actionButtonActive = false;


    private void Start()
    {
        instance = this; // no singelton pattern needed
    }

    public void OpenMenu()
    {
        if(menu.activeInHierarchy)
        {
            // close menu
            CloseMenu();
        }
        else
        {
            if(GameManager.instance.dialogBoxOpened != true && GameManager.instance.battleIsActive != true && GameManager.instance.goThroughExit != true) // don't open if one of these are texture
            {
                AudioManager.instance.PlaySFX(2);
                MenuButton.instance.SetActiveState(false);

                if(ActionButton.instance.gameObject.activeInHierarchy)
                {
                    actionButtonActive = true;
                }
                ActionButton.instance.SetActiveState(false);

                // open menu
                UpdateStats(); // display all current characters with stats
                menu.SetActive(true);
                KanjiInfoPageManager.instance.SetActiveState(false);
                GameManager.instance.gameMenuOpened = true; //set condition for player to stop moving

                itemsPanel.SetActive(false);
                kanjiInfoPage.SetActive(false);
                kanjiPanel.SetActive(true);

                // make sure scrollbar starts at top
                Canvas.ForceUpdateCanvases();
                scrollbar.value = 1f;
            }
        }
    }

    public void CloseKanjiPanel()
    {
        kanjiPanel.SetActive(false);
    }

    // when opening menu, overview of stats should be displayed (different from stats menu)
    public void UpdateStats()
    {
        KanjiManager[] collectedKanji = GameManager.instance.GetCollectedKanji();

        // go through all collected kanji
        for(int i = 0; i < collectedKanji.Length; i++)
        {
            // make panel for this character visible in menu
            characterPanel[i].SetActive(true);

            // fill panel with info of this character from their kanjiManager object
            characterImage[i].sprite = collectedKanji[i].kanjiImage;

            int index = i;
            detailsButtons[index].onClick.AddListener(() => kanjiInfoPage.GetComponent<KanjiInfoPageManager>().SetPage(collectedKanji[index].kanjiSymbol));

            xpText[i].text = collectedKanji[i].currentXP.ToString() + "/" + 100;
            xpSlider[i].maxValue = 100; // max value of XP slider --> needed XP
            xpSlider[i].value = collectedKanji[i].currentXP;
        }

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

            Vector3 newScale = new Vector3(0.8f, 0.8f, 0.8f);
            itemImage.transform.localScale = newScale;

            // set the amount text to a number if the amount is bigger than 1
            TextMeshProUGUI itemsAmountText = itemSlot.Find("Amount Text").GetComponent<TextMeshProUGUI>();
            if(item.amount > 1)
            {
                itemsAmountText.text = item.amount.ToString();
                itemsAmountText.color = Color.blue;
            }
            else
            {
                itemsAmountText.text = "";
            }

            // set item of item button (script given to botton component of item slot object)
            itemSlot.GetComponent<ItemButton>().itemOnButton = item;
            itemSlot.GetComponent<ItemButton>().itemsDescription = itemsDescription;
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
        AudioManager.instance.PlaySFX(3);
        MenuButton.instance.SetActiveState(true);
        menu.SetActive(false);

        if(actionButtonActive)
        {
            ActionButton.instance.SetActiveState(true);
            actionButtonActive = false;
        }

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

    public void QuitToMainMenu()
    {
        StartCoroutine(Transition());
    }

    public IEnumerator Transition()
    {
        AudioManager.instance.PlaySFX(8);
        FadeImage();
        yield return new WaitForSeconds(0.5f);

        DestroyGameSession();
        SceneManager.LoadScene("MainMenu");

    }

    private static void DestroyGameSession()
    {
        // destroy managers
        Destroy(GameManager.instance.gameObject);
        Destroy(Player.instance.gameObject);
        Destroy(MenuManager.instance.gameObject);
        Destroy(BattleManager.instance.gameObject);
    }
}
