using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObject : MonoBehaviour
{
    [SerializeField] GameObject objectToActivate;
    [SerializeField] string questToCheck;
    [SerializeField] bool activateIfComplete;

    // after completing a quest:
    public void CheckForCompletion()
    {
        if(QuestManager.instance.CheckIfComplete(questToCheck))
        {
            // only activate object if a certain quest in complete
            objectToActivate.SetActive(activateIfComplete);
        }
    }
}
