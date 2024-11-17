using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// given to game manager, defines available items in game
public class ItemsAssets : MonoBehaviour
{
    public static ItemsAssets instance;
    [SerializeField] ItemsManager[] itemsAvailable;


    void Start()
    {
        //singelton pattern --> avoid duplicate in new scenes
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // get an item (if it is available)
    public ItemsManager GetItemAsset(string itemToGetName)
    {
        foreach(ItemsManager item in itemsAvailable)
        {
            if(item.itemName == itemToGetName)
            {
                return item;
            }
        }

        return null;
    }
}
