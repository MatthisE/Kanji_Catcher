using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// given to objects that can activate quests (on enter or on enter + Jump)
public class QuestZone : MonoBehaviour
{

    [SerializeField] string questToMark;
    [SerializeField] bool markAsComplete; // false --> mark as incomplete

    [SerializeField] bool markOnEnter; // when to mark quest (true --> when hitting collider, false --> when in collider and pressing Jump)
    private bool canMark; // true when in collider of zone, otherwise false

    public bool deactivateOnMarking; // turn off zone (or destroy trigger item) after marking quest

    private void Update()
    {
        // check for second mark condition (after hitting collider)
        if(canMark && Input.GetButtonDown("Jump"))
        {
            canMark = false;
            MarkTheQuest();
        }
    }

    // mark either as complete or incomplete
    public void MarkTheQuest()
    {
        if(markAsComplete)
        {
            QuestManager.instance.MarkQuestComplete(questToMark);
        }
        else
        {
            QuestManager.instance.MarkQuestIncomplete(questToMark); // could be useful
        }

        gameObject.SetActive(!deactivateOnMarking); // deactivate zone (if deactivateOnMarking == true)
    }

    // either immediately mark quest or just set canMark to true
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(markOnEnter)
            {
                MarkTheQuest();
            }
            else
            {
                canMark = true;
            }
        }
    }
}
