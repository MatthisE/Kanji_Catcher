using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// given to a button object that has the name of the target as child
public class BattleTargetButtons : MonoBehaviour
{
    public TrainingWord trainingWord;
    public int activeBattleTarget;
    public TextMeshProUGUI targetName; // referenced by BattleManager

    void Start()
    {
        targetName = GetComponentInChildren<TextMeshProUGUI>(); // set targetName to the TextMeshProUGUI of the button (its child)
    }

    public void Press()
    {
        BattleManager.instance.PlayerAttack(trainingWord, activeBattleTarget); // activate set attack on set target
    }
}
