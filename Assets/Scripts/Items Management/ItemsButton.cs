using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

// given to buttons of individual items in normal menu or battle menu
public class ItemButton : MonoBehaviour
{
    public ItemsManager itemOnButton;
    public GameObject itemsDescription;

    public void Press()
    {
        if(BattleManager.instance.itemsToUseMenu.activeInHierarchy) // --> pressed button on battle menu
        {
            BattleManager.instance.SelectedItemToUse(itemOnButton); // mark item as selected (so you can use it)
        }
        else // if(MenuManager.instance.menu.activeInHierarchy) --> pressed button on normal menu
        {
            itemsDescription.SetActive(true); // show item description field

            MenuManager.instance.itemName.text = itemOnButton.itemName;
            MenuManager.instance.itemDescription.text = itemOnButton.itemDescription;

            MenuManager.instance.activeItem = itemOnButton; // mark item as active (so you can use or drop it)
        }
    }
}
