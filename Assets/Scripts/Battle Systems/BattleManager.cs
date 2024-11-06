//using System;  --> comment System out so you can use Random
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    private bool isBattleActive;

    [SerializeField] GameObject battleScene;
    [SerializeField] List<BattleCharacters> activeCharacters = new List<BattleCharacters>();

    [SerializeField] Transform playerPosition;
    [SerializeField] BattleCharacters player;

    [SerializeField] Transform[] enemiesPositions;
    [SerializeField] BattleCharacters[] enemiesPrefabs;

    [SerializeField] int currentTurn; // does not need to be serialized field
    [SerializeField] bool waitingForTurn; // does not need to be serialized field
    [SerializeField] GameObject UIButtonHolder;

    [SerializeField] BattleMoves[] battleMovesList;

    [SerializeField] ParticleSystem characterAttackEffect;
    [SerializeField] CharacterDamageGUI damageText;

    [SerializeField] GameObject[] playersBattleStats;
    [SerializeField] TextMeshProUGUI[] playersNameText; // just has 1 entry
    [SerializeField] Slider[] playerHealthSlider, playerManaSlider; // just has 1 entry

    [SerializeField] GameObject enemyTargetPanel;
    [SerializeField] BattleTargetButtons[] targetButtons;

    public GameObject magicChoicePanel;
    [SerializeField] BattleMagicButtons[] magicButtons;

    public BattleNotifications battleNotice;

    [SerializeField] float chanceToRunAway = 0.5f;

    public GameObject itemsToUseMenu;
    [SerializeField] ItemsManager selectedItem;
    [SerializeField] GameObject itemSlotContainer;
    [SerializeField] Transform itemSlotContainerParent;
    [SerializeField] TextMeshProUGUI itemName, itemDescription;

    [SerializeField] string gameOverScene;
    private bool runningAway;
    public int XPRewardAmount;
    public ItemsManager[] itemsReward;

    private bool canRun;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            StartBattle(new string[] {"Mage", "Warlock"}, true);
        }

        if(Input.GetKeyDown(KeyCode.N))
        {
            NextTurn();
        }

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
                    UIButtonHolder.SetActive(true);
                }
                else
                {
                    UIButtonHolder.SetActive(false);
                    StartCoroutine(EnemyMoveCoroutine());
                }
            }
        }
    }

    public void StartBattle(string[] enemiesToSpawn, bool canRunAway)
    {
        if(!isBattleActive)
        {
            canRun = canRunAway; // define if you can or cannot run away
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
        GameManager.instance.battleIsActive = true;

        transform.position = new Vector3(
            Camera.main.transform.position.x,
            Camera.main.transform.position.y,
            transform.position.z
        );
        battleScene.SetActive(true);
    }

    private void ImportPlayerStats(int i)
    {
        PlayerStats playerStats = GameManager.instance.GetPlayerStats()[0]; // get player stats of player (first (AND ONLY) in array)

        activeCharacters[i].currentHP = playerStats.currentHP; // set stats in players activeCharacters slot (0)
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
                        BattleCharacters newEnemy = Instantiate( // make enemy a battle character
                            enemiesPrefabs[j],
                            enemiesPositions[i].position,
                            enemiesPositions[i].rotation,
                            enemiesPositions[i]
                        );

                        activeCharacters.Add(newEnemy);
                    }
                }
            }
        }
    }

    private void NextTurn()
    {
        // loop through turn counter
        currentTurn++;
        if(currentTurn >= activeCharacters.Count)
        {
            currentTurn = 0;
        }

        waitingForTurn = true;
        UpdateBattle();

        UpdatePlayerStats(); // the UI
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
                if(activeCharacters[i].IsPlayer() && !activeCharacters[i].isDead)
                {
                    activeCharacters[i].KillPlayer();
                }

                if(!activeCharacters[i].IsPlayer() && !activeCharacters[i].isDead)
                {
                    activeCharacters[i].KillEnemy(); // so far same as kill player because I dont have death particles
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

        if(allEnemiesAreDead || playerIsDead)
        {
            if(allEnemiesAreDead)
            {
                StartCoroutine(EndBattleCoroutine());
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

    public IEnumerator EnemyMoveCoroutine()
    {
        waitingForTurn = false;

        yield return new WaitForSeconds(1f);
        EnemyAttack();

        yield return new WaitForSeconds(1f);
        NextTurn();
    }

    private void EnemyAttack()
    {
        int selectedAttack = Random.Range(0, activeCharacters[currentTurn].AttackMovesAvailable().Length); // select random move of enemy
        int movePower = 0;

        for(int i = 0; i < battleMovesList.Length; i++)
        {
            if(battleMovesList[i].moveName == activeCharacters[currentTurn].AttackMovesAvailable()[selectedAttack]) // if battle manager has move of active enemy
            {
                movePower = GettingMovePowerAndEffectInstantiation(0, i); // 0 --> player as target
            }
        }

        InstantiateEffectOnAttackingCharacter();

        DealDamageToCharacters(0, movePower); // attack player (at position 0 of activeCharacters)

        UpdatePlayerStats(); // the UI
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
        float attackPower = activeCharacters[currentTurn].dexterity; // power of current chara
        float defenceAmount = activeCharacters[selectedCharacterToAttack].dexterity; // defence of chara to be attacked

        float damageAmount = (attackPower / defenceAmount) * movePower * Random.Range(0.9f, 1.1f);
        int damageToGive = (int)damageAmount;

        damageToGive = CalculateCritical(damageToGive);

        Debug.Log(activeCharacters[currentTurn].characterName + " just dealt " + damageAmount + "(" + damageToGive + ") to " + activeCharacters[selectedCharacterToAttack]);

        activeCharacters[selectedCharacterToAttack].TakeHPDamage(damageToGive); // give chara damage

        CharacterDamageGUI characterDamageText = Instantiate(
            damageText,
            activeCharacters[selectedCharacterToAttack].transform.position,
            activeCharacters[selectedCharacterToAttack].transform.rotation
        );

        characterDamageText.SetDamage(damageToGive);
    }

    private int CalculateCritical(int damageToGive)
    {
        if(Random.value <= 0.8f) // this needs to change to 0.1f
        {
            Debug.Log("CRITICAL HIT!! instead of " + damageToGive + " points " + (damageToGive * 2) + " was dealt.");

            return (damageToGive * 2);
        }

        return damageToGive;
    }

    public void UpdatePlayerStats()
    {
        for(int i = 0; i < playersNameText.Length; i++)
        {
            if(activeCharacters.Count > i)
            {
                if(activeCharacters[i].IsPlayer())
                {
                    BattleCharacters playerData = activeCharacters[i]; // get player chara

                    // put player info in stats UI
                    playersNameText[i].text = playerData.characterName;

                    playerHealthSlider[i].maxValue = playerData.maxHP;
                    playerHealthSlider[i].value = playerData.currentHP;

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

    // Player Attacking Methods
    public void PlayerAttack(string moveName, int selectEnemyTarget)
    {
        //int selectEnemyTarget = 1;
        int movePower = 0;

        for(int i = 0; i < battleMovesList.Length; i++)
        {
            if(battleMovesList[i].moveName == moveName) // if battle manager has move of player
            {
                movePower = GettingMovePowerAndEffectInstantiation(selectEnemyTarget, i);
            }
        }

        InstantiateEffectOnAttackingCharacter();

        DealDamageToCharacters(selectEnemyTarget, movePower);

        NextTurn();

        enemyTargetPanel.SetActive(false);
    }

    public void OpenTargetMenu(string moveName)
    {
        enemyTargetPanel.SetActive(true); // activate enemy target select panel

        // create list of all the enemies 
        List<int> Enemies = new List<int>();

        for(int i = 0; i < activeCharacters.Count; i++)
        {
            if(!activeCharacters[i].IsPlayer())
            {
                Enemies.Add(i);
            }
        }

        //Debug.Log(Enemies.Count);

        for(int i = 0; i < targetButtons.Length; i++)
        {
            if(Enemies.Count > i && activeCharacters[Enemies[i]].currentHP > 0) // make sure you have (alive) enemies for the target buttons
            {
                targetButtons[i].gameObject.SetActive(true);
                targetButtons[i].moveName = moveName; // set move name for the player attack
                targetButtons[i].activeBattleTarget = Enemies[i];
                targetButtons[i].targetName.text = activeCharacters[Enemies[i]].characterName;
            }
            else
            {
                targetButtons[i].gameObject.SetActive(false); // turn off enemy target button
            }
        }
    }

    private int GettingMovePowerAndEffectInstantiation(int selectedCharacterTarget, int i)
    {
        int movePower;

        Instantiate(
            battleMovesList[i].effectToUse,
            activeCharacters[selectedCharacterTarget].transform.position, // position of selected target
            activeCharacters[selectedCharacterTarget].transform.rotation
        );

        movePower = battleMovesList[i].movePower; // set power of attack
        return movePower;
    }

    public void OpenMagicPanel()
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
                        magicButtons[i].spellCost = battleMovesList[j].manaCost;
                        magicButtons[i].spellCostText.text = magicButtons[i].spellCost.ToString();
                    }
                }
            }
            else
            {
                magicButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public BattleCharacters GetCurrentActiveCharacter()
    {
        return activeCharacters[currentTurn];
    }

    public void RunAway()
    {
        if(canRun)
        {
            if(Random.value > chanceToRunAway)
            {
                runningAway = true;
                StartCoroutine(EndBattleCoroutine());
            }
            else
            {
                NextTurn();
                battleNotice.SetText("You failed to run away.");
                battleNotice.Activate();
            }
        }
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

            TextMeshProUGUI itemsAmountText = itemSlot.Find("Amount Text").GetComponent<TextMeshProUGUI>();
            if(item.amount > 1)
            {
                itemsAmountText.text = item.amount.ToString();
            }
            else
            {
                itemsAmountText.text = "";
            }

            itemSlot.GetComponent<ItemButton>().itemOnButton = item;
        }
    }

    public void SelectedItemToUse(ItemsManager itemToUse)
    {
        selectedItem = itemToUse;
        itemName.text = itemToUse.itemName;
        itemDescription.text = itemToUse.itemDescription;
    }

    public void UseItemButton()
    {
        if(selectedItem)
        {
            activeCharacters[0].UseItemInBattle(selectedItem);
            Inventory.instance.RemoveItem(selectedItem);

            UpdatePlayerStats();

            CloseItemsMenu();
            UpdateItemsInInventory();

        }
        else{
            print("No item selected.");
        }
    }

    public void CloseItemsMenu()
    {
        itemsToUseMenu.SetActive(false);
    }

    public IEnumerator EndBattleCoroutine()
    {
        isBattleActive = false;
        UIButtonHolder.SetActive(false);
        enemyTargetPanel.SetActive(false);
        magicChoicePanel.SetActive(false);

        if(!runningAway)
        {
            battleNotice.SetText("WE WON!!");
            battleNotice.Activate();
        }

        yield return new WaitForSeconds(3f);

        // put stats of battle characters in overworld characters (?)
        foreach(BattleCharacters playerInBattle in activeCharacters)
        {
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

            Destroy(playerInBattle.gameObject);
        }

        battleScene.SetActive(false);
        activeCharacters.Clear();

        if(runningAway)
        {
            GameManager.instance.battleIsActive = false; // deactivate battle scene (also handeled by BattleRewardsHandler)
            runningAway = false;
        }
        else
        {
            BattleRewardsHandler.instance.OpenRewardScreen(XPRewardAmount, itemsReward);
        }

        currentTurn = 0;
    }

    public IEnumerator GameOverCoroutine()
    {
        battleNotice.SetText("WE LOST!");
        battleNotice.Activate();

        yield return new WaitForSeconds(3f);

        isBattleActive = false;
        SceneManager.LoadScene(gameOverScene);
    }
}