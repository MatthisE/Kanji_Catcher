using TMPro;
using UnityEngine;

// given to a button object that has the name of the target as child
public class BattleTargetButtons : MonoBehaviour
{
    public TrainingWord trainingWord;
    public int activeBattleTarget;
    public TextMeshProUGUI targetName; // referenced by BattleManager

    private void Start()
    {
        targetName = GetComponentInChildren<TextMeshProUGUI>(); // set targetName to the TextMeshProUGUI of the button (its child)
    }

    public void Press()
    {
        BattleManager.Instance.PlayerAttack(trainingWord, activeBattleTarget); // activate set attack on set target
    }
}