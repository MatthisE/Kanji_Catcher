using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// handels list of items
public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    private List<ItemsManager> itemsList;

    void Start()
    {
        instance = this;

        itemsList = new List<ItemsManager>();

        /*
        ItemsManager Book1 = new ItemsManager();
        ItemsManager Book2 = new ItemsManager();
        ItemsManager Book3 = new ItemsManager();

        Book1.itemName = "Book";
        Book2.itemName = "Book";
        Book3.itemName = "Book";

        AddItems(Book1);
        AddItems(Book2);
        AddItems(Book3);
        */
    }

    public void AddItems(ItemsManager item)
    {
        // for stackable items, just increase the counter
        if(item.isStackable)
        {
            bool itemAlreadyInInventory = false;

            // look for item in inventory
            foreach(ItemsManager itemInInventory in itemsList)
            {
                if(itemInInventory.itemName == item.itemName)
                {
                    itemInInventory.amount += item.amount;
                    itemAlreadyInInventory = true;
                }
            }

            // only add item if its not already in inventory
            if(!itemAlreadyInInventory)
            {
                itemsList.Add(item);
            }
        }
        else
        {
            itemsList.Add(item);
        }
    }

    // gets called by MenuManager
    public void RemoveItem(ItemsManager item)
    {
        if(item.isStackable)
        {
            ItemsManager inventoryItem = null; // later: set it to item to be removed

            // look for item to be removed
            foreach(ItemsManager itemInInventory in itemsList)
            {
                if(itemInInventory.itemName == item.itemName)
                {
                    itemInInventory.amount--; // decrease counter
                    inventoryItem = itemInInventory;
                }
            }

            // only remove stackable items if their amount is below 1
            if(inventoryItem != null && inventoryItem.amount <= 0)
            {
                itemsList.Remove(inventoryItem);
            }
        }
        else
        {
            itemsList.Remove(item);
        }
    }

    public void RemoveAllBooks()
    {
        itemsList.RemoveAll(item => item.itemName == "Book");
    }

    public List<ItemsManager> GetItemsList()
    {
        return itemsList;
    }
}
