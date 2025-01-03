using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

// displays rewards after a won battle and can complete a quest, called in BattleManager and BattleInstantiator
public class BattleRewardsHandler : MonoBehaviour
{
    public static BattleRewardsHandler instance;

    // display elements
    [SerializeField] GameObject rewardScreen;
    [SerializeField] ItemsManager[] rewardItems;
    [SerializeField] int xpReward;
    public KanjiXPSliderManager[] xpSliders;

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
        rewardItems = itemsEarned;
        KanjiManager[] collectedKanji = GameManager.instance.GetCollectedKanji();
        

        for(int i = 0; i < collectedKanji.Length; i++)
        {
            xpReward = collectedKanji[i].xpReward;

            // add XP to kanji of player
            KanjiManager kanji = collectedKanji[i];

            if(kanji.currentXP < 100)
            {
                if(kanji.currentXP + xpReward > 100)
                {
                    xpReward = 100 - kanji.currentXP;
                    kanji.currentXP = 100;
                }
                else
                {
                    kanji.currentXP += xpReward;
                }
            }
            else
            {
                xpReward = -10; // to show kanji does not need xp anymore
            }

            // set slider of kanji on reward screen
            xpSliders[i].gameObject.SetActive(true);
            xpSliders[i].SetSlider(kanji, xpReward);
        }

        rewardScreen.SetActive(true);
    }

    public void CloseRewardScreen()
    {
        // deactivate all xp sliders of reward screen
        foreach(KanjiXPSliderManager xpSlider in xpSliders)
        {
            xpSlider.gameObject.SetActive(false);
        }

        // add won items to inventory
        foreach(ItemsManager itemrewarded in rewardItems)
        {
            Inventory.instance.AddItems(itemrewarded);
        }

        // colse reward display
        rewardScreen.SetActive(false);
        GameManager.instance.battleIsActive = false; // keep player still while showing rewards

        MenuButton.instance.SetActiveState(true);

        // mark given quest as complete (optional)
        if(markQuestComplete)
        {
            QuestManager.instance.MarkQuestComplete(questToComplete);
        }
    }
}
