using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// given to NPC
public class DialogHandler : MonoBehaviour
{
    public string[] sentences;
    private bool canActivateBox; 

    [SerializeField] bool shouldActivateQuest;
    [SerializeField] string questToMark;
    [SerializeField] bool markAsComplete;

    // box can only be activated when player is inside object's trigger collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            canActivateBox = true;
            ActionButton.instance.SetActiveState(true);
            ActionButton.instance.SetActionText("Talk");

            ActionButton.instance.GetComponent<Button>().onClick.RemoveAllListeners();
            ActionButton.instance.GetComponent<Button>().onClick.AddListener(OpenDialogBox);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            canActivateBox = false;
            if (ActionButton.instance != null)
            {
                ActionButton.instance.SetActiveState(false);
                ActionButton.instance.GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
    }

    private void OpenDialogBox()
    {
        ActionButton.instance.SetActiveState(false);

        if(canActivateBox && !DialogController.instance.IsDialogBoxActive() && GameManager.instance.gameMenuOpened != true) // only call if the box is not already active and menu is not open
        {
            DialogController.instance.ActivateDialog(sentences); // open box with first sentence

            if(shouldActivateQuest)
            {
                DialogController.instance.ActivateQuestAtEnd(questToMark, markAsComplete); // activate quest after dialog
            }
        }
    }
}
