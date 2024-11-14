using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// given to a battle zone object
public class BattleInstantiator : MonoBehaviour
{
    [SerializeField] BattleTypeManager[] availableBattles; // the kind of battles that can happen in that zone
    [SerializeField] bool activateOnEnter; // a battle starts immediately when entering the zone, you can only have one battle in it (useful for bosses)

    [SerializeField] float timeBetweenBattles;
    private float battleCounter; // to count the time between battles
    private bool inArea; // make Update() able to reduce battleCounter

    [SerializeField] bool deactivateAfterStarting; // after 1 battle starts, zone disappears (should be used along activateOnEnter)

    [SerializeField] bool canRunAway;

    [SerializeField] bool shouldCompleteQuest; // winning a battle in this zone completes a quest (useful for bosses)
    public string questToComplete;

    private void Start()
    {
        battleCounter = Random.Range(timeBetweenBattles * 0.5f, timeBetweenBattles * 1.5f); // define a random time around timeBetweenBattles
    
        inArea = false; // when entering a new scene, you are not automatically in a battle zone
    }

    private void Update()
    {
        // count down time between battles
        if(inArea && !Player.instance.deactivateMovement) // player is in this battle zone an able to move (menu not open)
        {
            if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) // when player moves, reduce battle counter
            {
                battleCounter -= Time.deltaTime; // this could go below 0
            }
        }

        // start a new battle when counter reaches 0
        if(battleCounter <= 0) 
        {
            battleCounter = Random.Range(timeBetweenBattles * 0.5f, timeBetweenBattles * 1.5f); // set new counter
            StartCoroutine(StartBattleCoroutine()); // start battle coroutine
        }
    }

    private IEnumerator StartBattleCoroutine()
    {
        MenuManager.instance.FadeImage(); // fade to black
        GameManager.instance.battleIsActive = true; // make player unable to move

        // set up a random possible battle scenario
        int selectBattle = Random.Range(0, availableBattles.Length);

        BattleManager.instance.itemsReward = availableBattles[selectBattle].rewardItems;
        BattleManager.instance.XPRewardAmount = availableBattles[selectBattle].rewardXP;

        // tell RewardsHandler to mark a quest as complete if the battle is won (optional)
        BattleRewardsHandler.instance.markQuestComplete = shouldCompleteQuest;
        BattleRewardsHandler.instance.questToComplete = questToComplete;

        yield return new WaitForSeconds(1.5f); // wait 1.5sec
        BattleManager.instance.StartBattle(availableBattles[selectBattle].enemies, canRunAway); // activate battle scene
        MenuManager.instance.FadeOut(); // fade out to reveal battle scene

        if(deactivateAfterStarting)
        {
            Destroy(gameObject); // destroy zone
        }
    }

    // when player enters battle zone, either start battle or start counter
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(activateOnEnter)
            {
                StartCoroutine(StartBattleCoroutine());
            }
            else
            {
                inArea = true; // starts counter in Update()
            }
        }
    }

    // when player exits battle zone, stop counter
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            inArea = false; // Update() can no longer reduce counter
        }
    }
}
