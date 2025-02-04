using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// given to item objects
public class ItemsManager : MonoBehaviour
{
    public enum ItemType { Item, Weapon, Armor }
    public ItemType itemType;

    public string itemName, itemDescription;
    public int valueInCoins;
    public Sprite itemsImage;

    public enum AffectType { HP, Mana }
    public AffectType affectType;
    public int amountOfAffect;

    public int weaponDexterity;
    public int armorDefence;

    public bool isStackable;
    public int amount;

    public void UseItem()
    {
        if(itemType == ItemType.Item) // if item is of type item (not weapon or armor)
        {
            if(affectType == AffectType.HP)
            {
                // heal HP
                PlayerStats.instance.AddHP(amountOfAffect);
            }
            else if(affectType == AffectType.Mana)
            {
                // heal Mana
                PlayerStats.instance.AddMana(amountOfAffect);
            }
        }
    }

    // collecting an item on the map
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            AudioManager.instance.PlaySFX(5);
            Inventory.instance.AddItems(this);
            SelfDestroy();
        }
    }

    public void SelfDestroy()
    {
        gameObject.SetActive(false);
    }
}
