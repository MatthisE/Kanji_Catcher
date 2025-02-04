//using System;  --> comment System out so you can use Random
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

// given to battle manager object
public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    private bool isBattleActive;

    // scene with characters and their positions
    [SerializeField] GameObject battleScene;
    [SerializeField] List<BattleCharacters> activeCharacters = new List<BattleCharacters>();
    [SerializeField] Transform playerPosition;
    [SerializeField] BattleCharacters player;
    [SerializeField] Transform[] enemiesPositions;
    [SerializeField] BattleCharacters[] enemiesPrefabs;

    // info of player
    [SerializeField] GameObject[] playersBattleStats;
    [SerializeField] TextMeshProUGUI[] playersNameText; // just has 1 entry
    [SerializeField] Slider[] playerHealthSlider, playerManaSlider; // just has 1 entry
    [SerializeField] TextMeshProUGUI hpText;

    // attacks of battle characters
    [SerializeField] BattleMoves[] battleMovesList;
    [SerializeField] ParticleSystem characterAttackEffect;
    [SerializeField] CharacterDamageGUI damageText;
    private bool isCritical;

    // turn based system
    [SerializeField] int currentTurn; // does not need to be serialized field
    [SerializeField] bool waitingForTurn; // does not need to be serialized field

    // buttons
    [SerializeField] GameObject UIButtonHolder;
    [SerializeField] GameObject actionsMenu;

    [SerializeField] GameObject runningButton;

    // which enemy to target
    [SerializeField] GameObject enemyTargetPanel;
    [SerializeField] BattleTargetButtons[] targetButtons;
    private int selectEnemyTarget;

    // what magic attack
    public GameObject magicChoicePanel;
    [SerializeField] BattleMagicButtons[] magicButtons;

    // running away
    [SerializeField] float chanceToRunAway = 0.5f;
    private bool canRun;
    private bool runningAway;
    [SerializeField] string gameOverScene;

    // items
    public GameObject itemsToUseMenu;
    [SerializeField] ItemsManager selectedItem;
    [SerializeField] GameObject itemSlotContainer;
    [SerializeField] Transform itemSlotContainerParent;
    [SerializeField] TextMeshProUGUI itemName, itemDescription;

    // battle notifications
    public BattleNotifications battleNotice;

    // rewards
    public int XPRewardAmount;
    public ItemsManager[] itemsReward;
    [SerializeField] KanjiXPSliderManager[] xpSliders;

    // attack menus
    [SerializeField] GameObject enemyAttackMenu;
    [SerializeField] GameObject playerAttackMenu;

    void Start()
    {
        //singelton pattern --> avoid duplicates in new scenes
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);

        SetAttacks();
    }

    // when battle is active, always check if it is player's turn (show UI) or enemies' turn (make enemy move)
    void Update()
    {
        CheckPlayerButtonHolder();
    }

    private void CheckPlayerButtonHolder()
    {
        if(isBattleActive)
        {
            // de-/activate UI button holder depending on whos turn it is
            if(waitingForTurn)
            {
                if(activeCharacters[currentTurn].IsPlayer())
                {
                    UIButtonHolder.SetActive(true); // activate

                    if(enemyTargetPanel.activeInHierarchy != true){
                        actionsMenu.SetActive(true);
                    }
                }
                else
                {
                    UIButtonHolder.SetActive(false);
                    StartCoroutine(EnemyMoveCoroutine()); // dactivate and make enemy move
                }
            }
            else
            {
                UIButtonHolder.SetActive(false);
                enemyTargetPanel.SetActive(false);
            }
        }
    }

    // battle started by battle zone, display scene with player, his stats, enemies and UI
    public void StartBattle(string[] enemiesToSpawn, bool canRunAway)
    {
        SetAttacks(); 

        if(!isBattleActive)
        {
            MenuButton.instance.SetActiveState(false);
            ActionButton.instance.SetActiveState(false);

            canRun = canRunAway; // define if you can or cannot run away

            if(canRun)
            {
                runningButton.SetActive(true);
            }
            else
            {
                runningButton.SetActive(false);
            }
            
            SettingUpBattle(); 

            BattleCharacters newPlayer = Instantiate(
                player, // original object
                playerPosition.position, // object position
                playerPosition.rotation, // object rotation
                playerPosition // parent of object
            );

            activeCharacters.Add(newPlayer); // add player as a battle character
            ImportPlayerStats(0);

            AddingEnemies(enemiesToSpawn);

            UpdatePlayerStats(); // the UI

            waitingForTurn = true;
            currentTurn = 0; // or: Random.Range(0, activeCharacters.Count);
        }
    }

    private void SettingUpBattle()
    {
        isBattleActive = true;
        GameManager.instance.battleIsActive = true; // make player stop moving in overworld

        // make camera point to battle scene
        transform.position = new Vector3(
            Camera.main.transform.position.x,
            Camera.main.transform.position.y,
            transform.position.z
        );
        battleScene.SetActive(true); // diaply battle scene
    }

    private void ImportPlayerStats(int i)
    {
        PlayerStats playerStats = GameManager.instance.GetPlayerStats()[0]; // get stats of player (first (AND ONLY) in array)

        // set stats in player's activeCharacters slot (0)
        activeCharacters[i].currentHP = playerStats.currentHP;
        activeCharacters[i].maxHP = playerStats.maxHP;

        activeCharacters[i].currentMana = playerStats.currentMana;
        activeCharacters[i].maxMana = playerStats.maxMana;

        activeCharacters[i].dexterity = playerStats.dexterity;
        activeCharacters[i].defence = playerStats.defence;
    }

    private void AddingEnemies(string[] enemiesToSpawn)
    {
        for(int i = 0; i < enemiesToSpawn.Length; i++) // go through all enemies to spawn
        {
            if(enemiesToSpawn[i] != "") // if there is an enemy
            {
                for(int j = 0; j < enemiesPrefabs.Length; j++) // go through all enemy prefabs given to battle manager
                {
                    if(enemiesPrefabs[j].characterName == enemiesToSpawn[i]) // check if the names are the same
                    {
                        BattleCharacters newEnemy = null;

                        if(enemiesToSpawn.Length == 1)
                        {
                            newEnemy = Instantiate( // make enemy a battle character
                                enemiesPrefabs[j],
                                enemiesPositions[1].position, // at position 1 (middle)
                                enemiesPositions[1].rotation,
                                enemiesPositions[1]
                            );
                        }
                        else
                        {
                            newEnemy = Instantiate( // make enemy a battle character
                                enemiesPrefabs[j],
                                enemiesPositions[i].position,
                                enemiesPositions[i].rotation,
                                enemiesPositions[i]
                            );
                        }
                        activeCharacters.Add(newEnemy);
                    }
                }
            }
        }
    }

    public void UpdatePlayerStats()
    {
        for(int i = 0; i < playersNameText.Length; i++) // we just have 1 player
        {
            if(activeCharacters.Count > i)
            {
                if(activeCharacters[i].IsPlayer())
                {
                    BattleCharacters playerData = activeCharacters[i]; // get player chara (for his stats that were set by ImportPlayerStats())

                    // put player info in stats UI
                    playersNameText[i].text = "HP";

                    playerHealthSlider[i].maxValue = playerData.maxHP;
                    playerHealthSlider[i].value = playerData.currentHP;

                    hpText.text = playerData.currentHP + "/" + playerData.maxHP;

                    playerManaSlider[i].maxValue = playerData.maxMana;
                    playerManaSlider[i].value = playerData.currentMana;
                }
                else
                {
                    // turn off player
                    playersBattleStats[i].gameObject.SetActive(false);
                }
            }
            else
            {
                // turn off player if you have less active charas than texts
                playersBattleStats[i].gameObject.SetActive(false);
            }
        }
    }

    // enemy attacks player, then a new turn begins (or battle ends)
    public IEnumerator EnemyMoveCoroutine()
    {
        waitingForTurn = false;

        yield return new WaitForSeconds(1f);
        StartCoroutine(EnemyAttack());
    }

    private IEnumerator EnemyAttack()
    {
        yield return StartCoroutine(MoveCharacter(activeCharacters[currentTurn].GetComponent<SpriteRenderer>().transform, -0.1f, 0.2f));

        enemyAttackMenu.SetActive(true);
        enemyAttackMenu.GetComponent<EnemyAttack>().SetWords();
    }

    public void StartEnemyAttackImpact(float defence){
        StartCoroutine(EnemyAttackImpact(defence));
    }

    public IEnumerator EnemyAttackImpact(float defence)
    {
        enemyAttackMenu.SetActive(false);

        int selectedAttack = Random.Range(0, activeCharacters[currentTurn].AttackMovesAvailable().Length); // select random move of enemy

        int movePower = (int)(activeCharacters[currentTurn].dexterity * defence);
        DealDamageToCharacters(0, movePower); // calculate damage to player and attack him (at position 0 of activeCharacters), show damage number

        for(int i = 0; i < battleMovesList.Length; i++)
        {
            if(battleMovesList[i].moveName == activeCharacters[currentTurn].AttackMovesAvailable()[selectedAttack]) // if battle manager has move of active enemy
            {
                GettingMovePowerAndEffectInstantiation(0, i); // put damage effect on player (0 --> player as target)
            }
        }

        //InstantiateEffectOnAttackingCharacter(); // put attack effect on enemy

        UpdatePlayerStats(); // update player stats UI (might not need this here since it is also in NextTurn())

        yield return new WaitForSeconds(1f);
        NextTurn();
    }

    private IEnumerator MoveCharacter(Transform charTransform, float moveDistance, float duration)
    {
        Vector3 originalPosition = charTransform.position;
        Vector3 targetPosition = originalPosition + new Vector3(0, moveDistance, 0);

        // Move Down
        for (float t = 0; t < duration / 2f; t += Time.deltaTime)
        {
            charTransform.position = Vector3.Lerp(originalPosition, targetPosition, t / (duration / 2f));
            yield return null;
        }
        charTransform.position = targetPosition;

        // Move Up
        for (float t = 0; t < duration / 2f; t += Time.deltaTime)
        {
            charTransform.position = Vector3.Lerp(targetPosition, originalPosition, t / (duration / 2f));
            yield return null;
        }
        charTransform.position = originalPosition;
    }

    private void GettingMovePowerAndEffectInstantiation(int selectedCharacterTarget, int battleMove)
    {
        if(isCritical)
        {
            battleMove = 2;
        }

        // put damage effect of attack move on player
        Instantiate(
            battleMovesList[battleMove].effectToUse,
            activeCharacters[selectedCharacterTarget].transform.position, // position of selected target
            activeCharacters[selectedCharacterTarget].transform.rotation
        );
    }

    private void InstantiateEffectOnAttackingCharacter()
    {
        // instantiating the particle effect on the attacking character
        Instantiate(
            characterAttackEffect,
            activeCharacters[currentTurn].transform.position,
            activeCharacters[currentTurn].transform.rotation
        );
    }

    private void DealDamageToCharacters(int selectedCharacterToAttack, int movePower)
    {
        //float attackPower = activeCharacters[currentTurn].dexterity; // power of current chara
        //float defenceAmount = activeCharacters[selectedCharacterToAttack].dexterity; // defence of chara to be attacked

        //float damageAmount = (attackPower / defenceAmount) * movePower * Random.Range(0.9f, 1.1f);
        //int damageToGive = (int)damageAmount;

        int damageToGive = CalculateCritical(movePower);

        //Debug.Log(activeCharacters[currentTurn].characterName + " just dealt " + damageAmount + "(" + damageToGive + ") to " + activeCharacters[selectedCharacterToAttack]);

        activeCharacters[selectedCharacterToAttack].TakeHPDamage(damageToGive); // give chara damage

        // put damage number on chara
        CharacterDamageGUI characterDamageText = Instantiate(
            damageText,
            activeCharacters[selectedCharacterToAttack].transform.position,
            activeCharacters[selectedCharacterToAttack].transform.rotation
        );
        characterDamageText.SetColor(Color.red);
        characterDamageText.SetDamage(damageToGive);
    }

    private int CalculateCritical(int damageToGive)
    {
        // double damage on rare occasion
        if(Random.value <= 0.1f)
        {
            isCritical = true;
            return (damageToGive * 2);
        }

        return damageToGive;
    }

    private void NextTurn()
    {
        // loop through turn counter
        currentTurn++;
        if(currentTurn >= activeCharacters.Count)
        {
            currentTurn = 0;
        }

        isCritical = false;
        waitingForTurn = true;
        UpdateBattle(); // check who died and if battle is over

        UpdatePlayerStats(); // update player stats UI
    }

    private void UpdateBattle()
    {
        bool allEnemiesAreDead = true;
        bool playerIsDead = true;

        // check HP of all active characters
        for(int i = 0; i < activeCharacters.Count; i++)
        {
            if(activeCharacters[i].currentHP < 0)
            {
                activeCharacters[i].currentHP = 0; // min HP
            }

            if(activeCharacters[i].currentHP == 0)
            {
                // kill player
                if(activeCharacters[i].IsPlayer() && !activeCharacters[i].isDead)
                {
                    activeCharacters[i].KillPlayer();
                }

                // kill enemy
                if(!activeCharacters[i].IsPlayer() && !activeCharacters[i].isDead)
                {
                    activeCharacters[i].KillEnemy(); // so far same as kill player because I don't have death particles
                }
            }
            else
            {
                if(activeCharacters[i].IsPlayer())
                {
                    playerIsDead = false;
                }
                else
                {
                    allEnemiesAreDead = false;
                }
            }
        }

        // end battle
        if(allEnemiesAreDead || playerIsDead)
        {
            if(allEnemiesAreDead)
            {
                if(QuestManager.instance.questMarkersCompleted[3] == true)
                {
                    StartCoroutine(EndGame());
                }
                else
                {
                    StartCoroutine(EndBattleCoroutine());
                }
            }
            else if(playerIsDead)
            {
                StartCoroutine(GameOverCoroutine());
            }
        }
        else
        {
            // if a character is dead, skip his turn
            while(activeCharacters[currentTurn].currentHP == 0)
            {
                currentTurn++;
                if(currentTurn >= activeCharacters.Count)
                {
                    currentTurn = 0;
                }
            }
        }
    }
    private IEnumerator EndGame()
    {
        isBattleActive = false;
        UIButtonHolder.SetActive(false);
        enemyTargetPanel.SetActive(false);
        magicChoicePanel.SetActive(false);

        yield return new WaitForSeconds(1.5f); // wait 1.5sec
        MenuManager.instance.FadeImage();
        yield return new WaitForSeconds(1.0f); // wait 1.5sec
        string[] sentences = {"You have rid the library of its cursed energy.", "All monsters have turned back to normal and peoples' minds are clear again.", "The library is free but the rest of the word is still filled with cursed kanji.", "Can you catch them all?"};
        DialogController.instance.ActivateDialog(sentences, "end"); // open box with first sentence

    }

    // pick an attack, pick an enemy, make player attack
    public void OpenMagicPanel() // on click on magic panel
    {
        magicChoicePanel.SetActive(true);

        for(int i = 0; i < magicButtons.Length; i++)
        {
            if(activeCharacters[currentTurn].AttackMovesAvailable().Length > i)
            {
                // set spell names on buttons
                magicButtons[i].gameObject.SetActive(true);
                magicButtons[i].spellName = GetCurrentActiveCharacter().AttackMovesAvailable()[i];
                magicButtons[i].spellNameText.text = magicButtons[i].spellName;

                for(int j = 0; j < battleMovesList.Length; j++)
                {
                    if(battleMovesList[j].moveName == magicButtons[i].spellName)
                    {
                        // set stats of moves
                        magicButtons[i].spellCost = battleMovesList[j].manaCost;
                        magicButtons[i].spellCostText.text = magicButtons[i].spellCost.ToString();
                    }
                }
            }
            else
            {
                // no moves available
                magicButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetAttacks(){
        int wordThatNeedsTraining = Mathf.FloorToInt(Random.value * magicButtons.Length); // set random index of button with word that definitely needs training (so you never just have options with fully trained kanji)

        for(int i = 0; i < magicButtons.Length; i++)
        {
            bool needsTraining = false;
            if(i == wordThatNeedsTraining)
            {
                needsTraining = true;
            }
            TrainingWord randomWord = GetRandomWord(needsTraining);

            // set attack words on buttons
            magicButtons[i].spellName = randomWord.inKana;
            magicButtons[i].spellNameText.text = magicButtons[i].spellName;
            magicButtons[i].trainingWord = randomWord;
        }
    }

    private TrainingWord GetRandomWord(bool needsTraining){
        KanjiManager[] collectedKanji = GameManager.instance.GetCollectedKanji();

        int randomIndex = 0;
        if(needsTraining)
        {
            // get random kanji until you have one that definitely needs training (aka has less than 100xp)

            if(GameManager.instance.AllKanjiAreTrained())
            {
                randomIndex = Mathf.FloorToInt(Random.value * collectedKanji.Length);
            }
            else
            {
                bool isFullyTrained = true;
                while(isFullyTrained == true)
                {
                    randomIndex = Mathf.FloorToInt(Random.value * collectedKanji.Length);
                    if(collectedKanji[randomIndex].currentXP != 100)
                    {
                        isFullyTrained = false;
                    }
                }
            }
        }
        else
        {
            // get random kanji
            randomIndex = Mathf.FloorToInt(Random.value * collectedKanji.Length);
        }

        TrainingWord[] randomTrainingWords = collectedKanji[randomIndex].trainingWords;
        // get random training word for that kanji
        int randomIndex2 = Mathf.FloorToInt(Random.value * randomTrainingWords.Length);
        return randomTrainingWords[randomIndex2];
    }

    public BattleCharacters GetCurrentActiveCharacter()
    {
        return activeCharacters[currentTurn];
    }

    public void OpenTargetMenu(TrainingWord trainingWord) // on click on an attack
    {
        enemyTargetPanel.SetActive(true); // activate enemy target select panel
        actionsMenu.SetActive(false); // activate enemy target select panel

        // create list of all the enemies 
        List<int> Enemies = new List<int>();

        for(int i = 0; i < activeCharacters.Count; i++)
        {
            if(!activeCharacters[i].IsPlayer())
            {
                Enemies.Add(i);
            }
        }

        for(int i = 0; i < targetButtons.Length; i++)
        {
            if(Enemies.Count > i && activeCharacters[Enemies[i]].currentHP > 0) // make sure you have (alive) enemies for the target buttons
            {
                targetButtons[i].gameObject.SetActive(true);
                targetButtons[i].trainingWord = trainingWord; // set trainingWord for the player attack
                targetButtons[i].activeBattleTarget = Enemies[i];
                targetButtons[i].targetName.text = activeCharacters[Enemies[i]].characterName;
            }
            else
            {
                targetButtons[i].gameObject.SetActive(false); // turn off enemy target button
            }
        }
    }

    public void PlayerAttack(TrainingWord trainingWord, int thisSelectEnemyTarget)
    {
        waitingForTurn = false; // false so Update() does sets UIButtonHolder to inactive
        selectEnemyTarget = thisSelectEnemyTarget;
        playerAttackMenu.SetActive(true);
        playerAttackMenu.GetComponent<PlayerAttack>().SetWords(trainingWord, false);

    }

    public void PlayerHealing()
    {
        waitingForTurn = false; // false so Update() does sets UIButtonHolder to inactive
        playerAttackMenu.SetActive(true);
        playerAttackMenu.GetComponent<PlayerAttack>().SetWords(GetRandomWord(false), true);
    }

    public void StartPlayerAttackImpact(double offence)
    {
        playerAttackMenu.SetActive(false);
        StartCoroutine(PlayerAttackCoroutine("Slash", offence));
    }

    public IEnumerator PlayerAttackCoroutine(string moveName, double offence)
    {
        enemyTargetPanel.SetActive(false);
        waitingForTurn = false;
        UIButtonHolder.SetActive(false);

        yield return StartCoroutine(MoveCharacter(playerPosition, 0.1f, 0.2f));

        int movePower = (int)(20 * offence);
        DealDamageToCharacters(selectEnemyTarget, movePower); // calculate damage to player and attack him, show damage number

        for(int i = 0; i < battleMovesList.Length; i++)
        {
            if(battleMovesList[i].moveName == moveName) // if battle manager has selected move
            {
                GettingMovePowerAndEffectInstantiation(selectEnemyTarget, i); // put damage effect on enemy
            }
        }

        //InstantiateEffectOnAttackingCharacter(); // put attack effect on player

        NextTurn();

        SetAttacks();
    }

    // run away
    public void RunAway() // on click on run away
    {
        StartCoroutine(RunAwayCoroutine());
    }

    private IEnumerator RunAwayCoroutine()
    {
        if(canRun) // if the battle allows you to run
        {
            SetAttacks();

            if(Random.value > chanceToRunAway)
            {
                // run and end battle
                runningAway = true;
                battleNotice.SetText("You managed to run away.");
                battleNotice.Activate();
                StartCoroutine(EndBattleCoroutine());
            }
            else
            {
                // you wasted your turn
                battleNotice.SetText("You failed to run away.");
                battleNotice.Activate();
                
                waitingForTurn = false; // false so Update() does sets UIButtonHolder to inactive
                
                yield return new WaitUntil(() => !battleNotice.gameObject.activeSelf); // wait until the notice disappears

                waitingForTurn = true;
                NextTurn();
            }
        }
    }

    // items
    public void SelectedItemToUse(ItemsManager itemToUse) // used in ItemsButton.Press() to select item
    {
        selectedItem = itemToUse;
        itemName.text = itemToUse.itemName;
        itemDescription.text = itemToUse.itemDescription;
    }

    public void UseItemButton() // on click on use item
    {
        if(selectedItem)
        {
            activeCharacters[0].UseItemInBattle(selectedItem); // make player use item
            Inventory.instance.RemoveItem(selectedItem); // remove item from inventory

            UpdatePlayerStats(); // update player stats UI

            CloseItemsMenu();
            UpdateItemsInInventory();

        }
        else{
            print("No item selected.");
        }
    }

    
    public void HealPlayer(double amountOfAffect)
    {
        StartCoroutine(HealPlayerCoroutine(amountOfAffect));
    }

    public IEnumerator HealPlayerCoroutine(double amountOfAffect)
    {
        AudioManager.instance.PlaySFX(8);

        int healingPower = (int)(50 * amountOfAffect);

        // put damage number on chara
        CharacterDamageGUI characterDamageText = Instantiate(
            damageText,
            activeCharacters[0].transform.position,
            activeCharacters[0].transform.rotation
        );
        characterDamageText.SetColor(Color.green);
        characterDamageText.SetDamage(healingPower);

        playerAttackMenu.SetActive(false);
        activeCharacters[0].AddHP(healingPower);
        UpdatePlayerStats(); // update player stats UI

        yield return new WaitForSeconds(1f);
        NextTurn();
    }

    public void CloseItemsMenu()
    {
        itemsToUseMenu.SetActive(false);
    }

    public void UpdateItemsInInventory()
    {
        itemsToUseMenu.SetActive(true);

        // same as in Menu Manager
        foreach(Transform itemSlot in itemSlotContainerParent)
        {
            Destroy(itemSlot.gameObject); //destroy all previous item slots to not have doubles
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

    // end of battle
    public IEnumerator EndBattleCoroutine()
    {
        // deactivate battle scene components
        isBattleActive = false;
        UIButtonHolder.SetActive(false);
        enemyTargetPanel.SetActive(false);
        magicChoicePanel.SetActive(false);

        yield return new WaitForSeconds(2f);

        // put HP and mana of battle player in overworld player
        foreach(BattleCharacters playerInBattle in activeCharacters)
        {
            /*
            if(playerInBattle.IsPlayer())
            {
                foreach(PlayerStats playerWithStats in GameManager.instance.GetPlayerStats())
                {
                    if(playerInBattle.characterName == playerWithStats.playerName)
                    {
                        playerWithStats.currentHP = playerInBattle.currentHP;
                        playerWithStats.currentMana = playerInBattle.currentMana;
                    }
                }
            }
            */

            Destroy(playerInBattle.gameObject); // destroy battle player
        }

        // deactivate battle scene components
        battleScene.SetActive(false);
        activeCharacters.Clear();

        if(runningAway)
        {
            GameManager.instance.battleIsActive = false; // make player movable again
            runningAway = false;
            MenuButton.instance.SetActiveState(true);
        }
        else
        {
            // give rewards (and make player movable again)
            xpSliders = FindObjectsOfType<KanjiXPSliderManager>(true);
            BattleRewardsHandler.instance.xpSliders = xpSliders;
            BattleRewardsHandler.instance.OpenRewardScreen(XPRewardAmount, itemsReward); 
        }

        currentTurn = 0;

        // turn xp rewards for all kanji to 0
        KanjiManager[] collectedKanji = GameManager.instance.GetCollectedKanji();

        foreach(KanjiManager kanji in collectedKanji)
        {
            kanji.xpReward = 0;
        }
    }

    public IEnumerator GameOverCoroutine()
    {
        // deactivate battle scene components
        isBattleActive = false;
        UIButtonHolder.SetActive(false);
        enemyTargetPanel.SetActive(false);
        magicChoicePanel.SetActive(false);

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(gameOverScene); // load game over scene
    }
}