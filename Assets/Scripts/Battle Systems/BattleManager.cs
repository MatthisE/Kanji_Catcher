//using System;  --> comment System out so you can use Random
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            StartBattle(new string[] {"Mage", "Warlock"});
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

    public void StartBattle(string[] enemiesToSpawn)
    {
        if(!isBattleActive)
        {
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

            waitingForTurn = true;
            currentTurn = 0; // Random.Range(0, activeCharacters.Count);
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
                // kill character
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
                print("WE WON!!!");
            }
            else if(playerIsDead)
            {
                print("WE LOST!!!");
            }

            // exit battle mode
            battleScene.SetActive(false);
            GameManager.instance.battleIsActive = false;
            isBattleActive = false;
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
                Instantiate(
                    battleMovesList[i].effectToUse,
                    activeCharacters[0].transform.position, // position of player
                    activeCharacters[0].transform.rotation
                );

                movePower = battleMovesList[i].movePower; // set power of attack
            }
        }

        // instantiating the particle effect on the attacking character
        Instantiate(
            characterAttackEffect,
            activeCharacters[currentTurn].transform.position,
            activeCharacters[currentTurn].transform.rotation
        );

        DealDamageToCharacters(0, movePower); // attack player (at position 0 of activeCharacters)
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
}
