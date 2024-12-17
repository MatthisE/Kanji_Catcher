using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// given to NPC
public class DialogHandler : MonoBehaviour
{
    public string[] sentences;
    private bool canActivateBox; 

    [SerializeField] bool shouldActivateQuest;
    [SerializeField] string questToMark;
    [SerializeField] bool markAsComplete;

    void Update()
    {
        if(canActivateBox && Input.GetButtonDown("Jump") && !DialogController.instance.IsDialogBoxActive()) // only call if the box is not already active
        {
            DialogController.instance.ActivateDialog(sentences); // open box with first sentence

            if(shouldActivateQuest)
            {
                DialogController.instance.ActivateQuestAtEnd(questToMark, markAsComplete); // activate quest after dialog
            }
        }
    }

    // box can only be activated when player is inside object's trigger collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            canActivateBox = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            canActivateBox = false;
        }
    }
}
