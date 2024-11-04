using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleMagicButtons : MonoBehaviour
{
    public string spellName;
    public int spellCost;

    public TextMeshProUGUI spellNameText, spellCostText;

    public void Press()
    {
        if(BattleManager.instance.GetCurrentActiveCharacter().currentMana >= spellCost)
        {
            BattleManager.instance.magicChoicePanel.SetActive(false);
            BattleManager.instance.OpenTargetMenu(spellName);

            BattleManager.instance.GetCurrentActiveCharacter().currentMana -= spellCost;
        }
        else
        {
            BattleManager.instance.battleNotice.SetText("You don't have enough mana.");
            BattleManager.instance.battleNotice.Activate();
            BattleManager.instance.magicChoicePanel.SetActive(false);
        }
    }
}
