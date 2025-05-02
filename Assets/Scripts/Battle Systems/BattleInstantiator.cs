using System.Collections;
using UnityEngine;

// given to a battle zone object
public class BattleInstantiator : MonoBehaviour
{
    [SerializeField] private BattleTypeManager[] availableBattles; // the kind of battles that can happen in that zone
    [SerializeField] private bool activateOnEnter; // a battle starts immediately when entering the zone, you can only have one battle in it (useful for bosses)

    [SerializeField] private float timeBetweenBattles;
    private float battleCounter; // to count the time between battles
    private bool inArea; // make Update() able to reduce battleCounter

    [SerializeField] private bool deactivateAfterStarting; // after 1 battle starts, zone disappears (should be used along activateOnEnter)

    [SerializeField] private bool canRunAway;

    [SerializeField] private bool shouldCompleteQuest; // winning a battle in this zone completes a quest (useful for bosses)
    public string questToComplete;
    private readonly float epsilon = 0.0001f;

    private void Start()
    {
        battleCounter = Random.Range(timeBetweenBattles * 0.5f, timeBetweenBattles * 1.5f); // define a random time around timeBetweenBattles
        inArea = false; // when entering a new scene, you are not automatically in a battle zone
    }

    private void Update()
    {
        // count down time between battles
        if (inArea && !Player.instance.deactivateMovement && (Joystick.instance.Horizontal < 0 - epsilon || Joystick.instance.Horizontal > 0 + epsilon || Joystick.instance.Vertical < 0 - epsilon || Joystick.instance.Vertical > 0 + epsilon)) // player is in this battle zone an able to move (menu not open) & when player moves, reduce battle counter
        {
            battleCounter -= Time.deltaTime;
        }

        // start a new battle when counter reaches 0
        if (battleCounter <= 0)
        {
            battleCounter = Random.Range(timeBetweenBattles * 0.5f, timeBetweenBattles * 1.5f); // set new counter
            _ = StartCoroutine(StartBattleCoroutine()); // start battle coroutine
        }
    }

    private IEnumerator StartBattleCoroutine()
    {
        MenuManager.instance.FadeImage(); // fade to black
        GameManager.instance.battleIsActive = true; // make player unable to move

        // set up a random possible battle scenario
        int selectBattle = Random.Range(0, availableBattles.Length);

        BattleManager.Instance.itemsReward = availableBattles[selectBattle].rewardItems;
        BattleManager.Instance.XPRewardAmount = availableBattles[selectBattle].rewardXP;

        // tell RewardsHandler to mark a quest as complete if the battle is won (optional)
        BattleRewardsHandler.Instance.markQuestComplete = shouldCompleteQuest;
        BattleRewardsHandler.Instance.questToComplete = questToComplete;

        yield return new WaitForSeconds(1.5f); // wait 1.5sec
        BattleManager.Instance.StartBattle(availableBattles[selectBattle].enemies, canRunAway); // activate battle scene
        MenuManager.instance.FadeOut(); // fade out to reveal battle scene

        if (deactivateAfterStarting)
        {
            Destroy(gameObject); // destroy zone
        }
    }

    // when player enters battle zone, either start battle or start counter
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (activateOnEnter)
            {
                _ = StartCoroutine(StartBattleCoroutine());
            }
            else
            {
                inArea = true; // starts counter in Update()
                battleCounter = Random.Range(timeBetweenBattles * 0.5f, timeBetweenBattles * 1.5f); // define a random time around timeBetweenBattles
            }
        }
    }

    // when player exits battle zone, stop counter
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inArea = false; // Update() can no longer reduce counter
        }
    }
}