using UnityEngine;

// given to a quest object (its child is the object that should dis-/appear after a certain quest is completed)
public class QuestObject : MonoBehaviour
{
    [SerializeField] private GameObject objectToActivate; // the child object
    [SerializeField] private string questToCheck;
    [SerializeField] private bool activateIfComplete; // tue --> object appears, false --> object disappears (after questToCheck is completed)

    public void CheckForCompletion() // gets called for every quest object after any quest got completed (by QuestManager)
    {
        if (QuestManager.instance.CheckIfComplete(questToCheck)) // if the completed quest is questToCheck
        {
            // de/-activate object
            objectToActivate.SetActive(activateIfComplete);
        }
    }
}