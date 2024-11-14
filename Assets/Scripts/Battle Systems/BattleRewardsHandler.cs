using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

// displays rewards after a won battle and can complete a quest
public class BattleRewardsHandler : MonoBehaviour
{
    public static BattleRewardsHandler instance;

    // display elements
    [SerializeField] TextMeshProUGUI XPText, itemsText;
    [SerializeField] GameObject rewardScreen;

    [SerializeField] ItemsManager[] rewardItems;
    [SerializeField] int xpReward;

    // optional quest completion
    public bool markQuestComplete;
    public string questToComplete;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            OpenRewardScreen(15000, rewardItems);
        }
    }

    // open reward screen with given elements
    public void OpenRewardScreen(int xpEarned, ItemsManager[] itemsEarned)
    {
        xpReward = xpEarned;
        rewardItems = itemsEarned;

        XPText.text = xpEarned + " XP";
        itemsText.text = "";

        itemsText.text += " * ";
        foreach(ItemsManager rewardItemText in rewardItems)
        {
            itemsText.text += rewardItemText.itemName + " * ";
        }

        rewardScreen.SetActive(true);
    }

    public void CloseRewardScreen()
    {
        // give player his won XP
        foreach(PlayerStats activePlayer in GameManager.instance.GetPlayerStats())
        {
            if(activePlayer.gameObject.activeInHierarchy)
            {
                activePlayer.AddXP(xpReward);
            }
        }

        // add won items to inventory
        foreach(ItemsManager itemrewarded in rewardItems)
        {
            Inventory.instance.AddItems(itemrewarded);
        }

        // colse reward display
        rewardScreen.SetActive(false);
        GameManager.instance.battleIsActive = false; // keep player still while showing rewards

        // mark given quest as complete (optional)
        if(markQuestComplete)
        {
            QuestManager.instance.MarkQuestComplete(questToComplete);
        }
    }
}
