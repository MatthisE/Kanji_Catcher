using UnityEngine;

// given to game manager, defines available items in game
public class ItemsAssets : MonoBehaviour
{
    public static ItemsAssets instance;
    [SerializeField] private ItemsManager[] itemsAvailable;


    private void Start()
    {
        //singelton pattern --> avoid duplicate in new scenes
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // get an item (if it is available)
    public ItemsManager GetItemAsset(string itemToGetName)
    {
        foreach (ItemsManager item in itemsAvailable)
        {
            if (item.itemName == itemToGetName)
            {
                return item;
            }
        }

        return null;
    }
}