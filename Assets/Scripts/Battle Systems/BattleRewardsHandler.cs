using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BattleRewardsHandler : MonoBehaviour
{
    public static BattleRewardsHandler instance;

    [SerializeField] TextMeshProUGUI XPText, itemsText;
    [SerializeField] GameObject rewardScreen;

    [SerializeField] ItemsManager[] rewardItems;
    [SerializeField] int xpReward;

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

    public void OpenRewardScreen(int xpEarned, ItemsManager[] itemsEarned)
    {
        xpReward = xpEarned;
        rewardItems = itemsEarned;

        XPText.text = xpEarned + " XP";
        itemsText.text = "";

        foreach(ItemsManager rewardItemText in rewardItems)
        {
            itemsText.text += rewardItemText.itemName + " ";
        }

        rewardScreen.SetActive(true);
    }

    public void CloseRewardScreen()
    {
        foreach(PlayerStats activePlayer in GameManager.instance.GetPlayerStats())
        {
            if(activePlayer.gameObject.activeInHierarchy)
            {
                activePlayer.AddXP(xpReward);
            }
        }

        foreach(ItemsManager itemrewarded in rewardItems)
        {
            Inventory.instance.AddItems(itemrewarded);
        }

        rewardScreen.SetActive(false);
        GameManager.instance.battleIsActive = false; // keep player still while showing rewards

        if(markQuestComplete)
        {
            QuestManager.instance.MarkQuestComplete(questToComplete);
        }
    }
}
