using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class ItemButton : MonoBehaviour
{
    public ItemsManager itemOnButton;

    public void Press()
    {
        // put button on battle menu
        if(BattleManager.instance.itemsToUseMenu.activeInHierarchy)
        {
            BattleManager.instance.SelectedItemToUse(itemOnButton);
        }
        else // if(MenuManager.instance.menu.activeInHierarchy) --> put button on normal menu
        {
            MenuManager.instance.itemName.text = itemOnButton.itemName;
            MenuManager.instance.itemDescription.text = itemOnButton.itemDescription;

            MenuManager.instance.activeItem = itemOnButton; // mark item as active (so you can drop it)
        }
    }
}
