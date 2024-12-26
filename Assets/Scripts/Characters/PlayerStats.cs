using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// given to player object, handels all his stats
public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;
    public string playerName;
    public Sprite characterImage;

    [SerializeField] int maxLevel = 50;
    public int playerLevel = 1;
    public int currentXP;
    public int[] xpForNextLevel;
    [SerializeField] int baseLevelXP = 100;

    public int maxHP = 100;
    public int currentHP;

    public int maxMana = 30;
    public int currentMana;

    public int dexterity;
    public int defence;

    public KanjiManager[] collectedKanji;

    void Start()
    {
        instance = this;

        // fill the xpForEachLevel array
        xpForNextLevel = new int[maxLevel];
        xpForNextLevel[1] = baseLevelXP;

        for(int i = 2; i < xpForNextLevel.Length; i++)
        {
            xpForNextLevel[i] = (int)(0.02f * i * i * i + 3.06f * i * i + 105.6f * i); // needed XP increases for each level (formula from dark souls)
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            AddXP(100);
        }
    }

    public void AddXP(int amountOfXp)
    {
        currentXP += amountOfXp;
        while(currentXP > xpForNextLevel[playerLevel]) // level up
        {
            currentXP -= xpForNextLevel[playerLevel]; // reset player XP 0 + left over XP from the level up 
            playerLevel++;

            // for each level up switc between increasing dexterity and defence
            if(playerLevel % 2 == 0)
            {
                dexterity++;
            }
            else
            {
                defence++;
            }

            // increase maxHP and maxMana and set current values to them
            maxHP = Mathf.FloorToInt(maxHP * 1.06f);
            currentHP = maxHP;

            maxMana = Mathf.FloorToInt(maxMana * 1.06f);
            currentMana = maxMana;
        }
    }

    // add HP or mana, cannot go above max values, used by items
    public void AddHP(int amountHPToAdd)
    {
        currentHP += amountHPToAdd;
        if(currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }

    public void AddMana(int amountManaToAdd)
    {
        currentMana += amountManaToAdd;
        if(currentMana > maxMana)
        {
            currentMana = maxMana;
        }
    }
}
