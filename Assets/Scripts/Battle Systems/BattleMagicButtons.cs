using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// given to spell buttons in battle menu
public class BattleMagicButtons : MonoBehaviour
{
    public string spellName;
    public int spellCost;

    public TextMeshProUGUI spellNameText, spellCostText;

    // after pressing a spell button in battle
    public void Press()
    {
        // if player has enough mana
        if(BattleManager.instance.GetCurrentActiveCharacter().currentMana >= spellCost)
        {
            // make enemy target selectable
            BattleManager.instance.magicChoicePanel.SetActive(false);
            BattleManager.instance.OpenTargetMenu(spellName);

            // remove mana
            BattleManager.instance.GetCurrentActiveCharacter().currentMana -= spellCost;
        }
        else
        {
            // display message
            BattleManager.instance.battleNotice.SetText("You don't have enough mana.");
            BattleManager.instance.battleNotice.Activate();
            BattleManager.instance.magicChoicePanel.SetActive(false);
        }
    }
}
