using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacters : MonoBehaviour
{
    [SerializeField] bool isPlayer;
    [SerializeField] string[] attacksAvailable;

    public string characterName;
    public int currentHP, maxHP, currentMana, maxMana, dexterity, defence;
    public bool isDead;
    
    public bool IsPlayer()
    {
        return isPlayer;
    }

    public string[] AttackMovesAvailable()
    {
        return attacksAvailable;
    }

    public void TakeHPDamage(int damageToReceive)
    {
        Debug.Log("From " + currentHP);

        currentHP -= damageToReceive;

        if(currentHP < 0)
        {
            currentHP = 0;
        }

        Debug.Log(" to " + currentHP);
    }
}
