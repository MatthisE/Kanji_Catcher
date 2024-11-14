using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// given to NPC
public class DialogHandler : MonoBehaviour
{
    public string[] sentences;
    private bool canActivateBox; // can only be activated inside objects trigger box

    [SerializeField] bool shouldActivateQuest;
    [SerializeField] string questToMark;
    [SerializeField] bool markAsComplete;

    void Update()
    {
        if(canActivateBox && Input.GetButtonDown("Fire1") && !DialogController.instance.IsDialogBoxActive()) // only call if the box is not already active
        {
            DialogController.instance.ActivateDialog(sentences);

            if(shouldActivateQuest)
            {
                DialogController.instance.ActivateQuestAtEnd(questToMark, markAsComplete);
            }
        }
    }

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
