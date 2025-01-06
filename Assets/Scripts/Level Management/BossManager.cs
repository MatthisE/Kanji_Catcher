using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossManager : MonoBehaviour
{
    public string[] sentences;
    private bool canActivateBox; 


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
            List<string> sentencesList = new List<string> { "#Darkness", "I am an amalgamation of the cursed energy in this place...", "If you want to cleanse this place you must find and train all the cursed kanji and then defeat me..." };

            KanjiManager[] collectedKanji = GameManager.instance.GetCollectedKanji();
            int kanjiLeft = 9 - collectedKanji.Length;

            if(kanjiLeft > 0)
            {
                sentencesList.Add("There are still " + kanjiLeft + " cursed kanji you need to find...");
            }
            else
            {
                sentencesList.Add("You found all the cursed kanji in this place...");

                bool allKanjiAreFullyTrained = true;
                foreach(KanjiManager kanji in collectedKanji)
                {
                    if(kanji.currentXP != 100)
                    {
                        allKanjiAreFullyTrained = false;
                        break;
                    }
                }

                if(allKanjiAreFullyTrained)
                {
                    sentencesList.Add("Now let us battle...");
                }
                else
                {
                    sentencesList.Add("But you must train them all fully before you can challenge me...");
                }
            }

            sentences = sentencesList.ToArray();
            DialogController.instance.ActivateDialog(sentences, ""); // open box with first sentence
        }
    }
}
