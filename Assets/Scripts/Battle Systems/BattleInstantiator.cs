using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInstantiator : MonoBehaviour
{
    [SerializeField] BattleTypeManager[] availableBattles;
    [SerializeField] bool activateOnEnter;
    private bool inArea;

    [SerializeField] float timeBetweenBattles;
    private float battleCounter;

    [SerializeField] bool deactivateAfterStarting; // true --> you can only have 1 battle in that zone

    [SerializeField] bool canRunAway;

    [SerializeField] bool shouldCompleteQuest;
    public string questToComplete;

    private void Start()
    {
        battleCounter = Random.Range(timeBetweenBattles * 0.5f, timeBetweenBattles * 1.5f);
        inArea = false;
    }

    private void Update()
    {
        if(inArea && !Player.instance.deactivateMovement) // player is in area an able to move (menu not open)
        {
            if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) // when player moves, reduce battle counter
            {
                battleCounter -= Time.deltaTime; // this could go below 0
            }
        }

        if(battleCounter <= 0) 
        {
            battleCounter = Random.Range(timeBetweenBattles * 0.5f, timeBetweenBattles * 1.5f);
            StartCoroutine(StartBattleCoroutine());
        }
    }

    private IEnumerator StartBattleCoroutine()
    {
        MenuManager.instance.FadeImage();
        GameManager.instance.battleIsActive = true;

        int selectBattle = Random.Range(0, availableBattles.Length);

        BattleManager.instance.itemsReward = availableBattles[selectBattle].rewardItems;
        BattleManager.instance.XPRewardAmount = availableBattles[selectBattle].rewardXP;

        BattleRewardsHandler.instance.markQuestComplete = shouldCompleteQuest;
        BattleRewardsHandler.instance.questToComplete = questToComplete;

        yield return new WaitForSeconds(1.5f);

        MenuManager.instance.FadeOut();

        BattleManager.instance.StartBattle(availableBattles[selectBattle].enemies, canRunAway);

        if(deactivateAfterStarting)
        {
            Destroy(gameObject);
        }
    }

    // when entering battle zone, start battle
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            print("Enter");
            if(activateOnEnter)
            {
                StartCoroutine(StartBattleCoroutine());
            }
            else
            {
                inArea = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            print("Exit");
            inArea = false;
        }
    }
}
