using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// given to a battle character object
public class BattleCharacters : MonoBehaviour
{
    [SerializeField] bool isPlayer;
    [SerializeField] string[] attacksAvailable;

    public string characterName;
    public int currentHP, maxHP, currentMana, maxMana, dexterity, defence;
    public bool isDead;

    private void Update()
    {
        // make enemies disappear after they die
        if(!isPlayer && isDead)
        {
            FadeOutEnemy();
        }
    }

    private void FadeOutEnemy()
    {
        // make all the color values of the sprite of the object go to transparent
        GetComponent<SpriteRenderer>().color = new Color(
            Mathf.MoveTowards(GetComponent<SpriteRenderer>().color.r, 1f, 0.9f * Time.deltaTime), // red component 
            Mathf.MoveTowards(GetComponent<SpriteRenderer>().color.g, 0f, 0.9f * Time.deltaTime), // green component 
            Mathf.MoveTowards(GetComponent<SpriteRenderer>().color.b, 0f, 0.9f * Time.deltaTime), // blue component 
            Mathf.MoveTowards(GetComponent<SpriteRenderer>().color.a, 0f, 0.9f * Time.deltaTime) // alpha component 
        );

        // once fully transparent deactivate object
        if(GetComponent<SpriteRenderer>().color.a == 0)
        {
            gameObject.SetActive(false);
        }
    }

    // substract damage from hp, cannot go below 0
    public void TakeHPDamage(int damageToReceive)
    {
        currentHP -= damageToReceive;

        if(currentHP < 0)
        {
            currentHP = 0;
        }
    }

    // set battle character to dead
    public void KillEnemy()
    {
        isDead = true;
    }

    public void KillPlayer()
    {
        isDead = true;
    }

    // use item to either heal hp or mana depending on its affect type
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

    public void AddHP(int amountOfAffect)
    {   
        if(currentHP + amountOfAffect > 100)
        {
            amountOfAffect = 100 - currentHP;
        }

        currentHP += amountOfAffect;
    }

    private void AddMana(int amountOfAffect)
    {
        currentMana += amountOfAffect;
    }

    // get info about character
    public bool IsPlayer()
    {
        return isPlayer;
    }

    public string[] AttackMovesAvailable()
    {
        return attacksAvailable;
    }
}
