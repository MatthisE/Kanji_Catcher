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
    [SerializeField] int xpRewards;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            OpenRewardScreen(15000, rewardItems);
        }
    }

    public void OpenRewardScreen(int xpEarned, ItemsManager[] itemsEarned)
    {
        xpRewards = xpEarned;
        rewardItems = itemsEarned;

        XPText.text = xpEarned + " XP";
        itemsText.text = "";

        foreach(ItemsManager rewardItemText in rewardItems)
        {
            itemsText.text += rewardItemText.itemName + " ";
        }

        rewardScreen.SetActive(true);
    }

    public void CloseButton()
    {
        rewardScreen.SetActive(false);
    }
}
