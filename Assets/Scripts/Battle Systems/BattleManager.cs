using System;
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
        bool allEnemiesAreDead = false;
        bool playerIsDead = false;

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
        
    }
}
