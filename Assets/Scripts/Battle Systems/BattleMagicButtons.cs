using UnityEngine;
using TMPro;

// given to spell buttons in battle menu
public class BattleMagicButtons : MonoBehaviour
{
    public string spellName;
    public int spellCost;

    public TextMeshProUGUI spellNameText, spellCostText;

    public TrainingWord trainingWord;

    // after pressing a spell button in battle
    public void Press()
    {
        // if player has enough mana
        if (BattleManager.Instance.GetCurrentActiveCharacter().currentMana >= spellCost)
        {
            // make enemy target selectable
            BattleManager.Instance.magicChoicePanel.SetActive(false);
            BattleManager.Instance.OpenTargetMenu(trainingWord);

            // remove mana
            BattleManager.Instance.GetCurrentActiveCharacter().currentMana -= spellCost;
        }
        else
        {
            // display message
            BattleManager.Instance.battleNotice.SetText("You don't have enough mana.");
            BattleManager.Instance.battleNotice.Activate();
            BattleManager.Instance.magicChoicePanel.SetActive(false);
        }
    }
}