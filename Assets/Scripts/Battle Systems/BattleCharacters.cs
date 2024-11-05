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

    private void Update()
    {
        if(!isPlayer && isDead)
        {
            FadeOutEnemy();
        }
    }

    private void FadeOutEnemy()
    {
        GetComponent<SpriteRenderer>().color = new Color(
            Mathf.MoveTowards(GetComponent<SpriteRenderer>().color.r, 1f, 0.9f * Time.deltaTime), // red component 
            Mathf.MoveTowards(GetComponent<SpriteRenderer>().color.g, 0f, 0.9f * Time.deltaTime), // green component 
            Mathf.MoveTowards(GetComponent<SpriteRenderer>().color.b, 0f, 0.9f * Time.deltaTime), // blue component 
            Mathf.MoveTowards(GetComponent<SpriteRenderer>().color.a, 0f, 0.9f * Time.deltaTime) // alpha component 
        );

        if(GetComponent<SpriteRenderer>().color.a == 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void KillEnemy()
    {
        isDead = true;
    }
    
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
        currentHP -= damageToReceive;

        if(currentHP < 0)
        {
            currentHP = 0;
        }
    }

    public void UseItemInBattle(ItemsManager itemToUse)
    {
        if(itemToUse.itemType == ItemsManager.ItemType.Item)
        {
            if(itemToUse.affectType == ItemsManager.AffectType.HP)
            {
                AddHP(itemToUse.amountOfAffect);
            }
            else if(itemToUse.affectType == ItemsManager.AffectType.Mana)
            {
                AddMana(itemToUse.amountOfAffect);
            }
        }
    }

    private void AddHP(int amountOfAffect)
    {
        currentHP += amountOfAffect;
    }

    private void AddMana(int amountOfAffect)
    {
        currentMana += amountOfAffect;
    }

    public void KillPlayer()
    {
        isDead = true;
    }
}
