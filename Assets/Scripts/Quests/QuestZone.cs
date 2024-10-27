using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    [SerializeField] string questToMark; // what quest to mark
    [SerializeField] bool markAsComplete; // how to mark it

    [SerializeField] bool markOnEnter; // when to mark (true --> when hitting collider, false --> after hitting collider and doing smt (eg pressing Fire1))
    private bool canMark; // set true when hitting collider (and not immediately mark on enter)

    public bool deactivateOnMarking; // if you want to turn off the zone (or destroy trigger item) after marking quest

    private void Update()
    {
        // check for second mark condition (after hitting collider)
        if(canMark && Input.GetButtonDown("Fire1"))
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

        gameObject.SetActive(!deactivateOnMarking); // if deactivateOnMarking --> set active to false
    }

    // either immediately mark quest or just setting canMark to true
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
