using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// controls dialog boxes and their text, can mark a quest, used by DialogHandler
public class DialogController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogText, nameText;
    [SerializeField] GameObject dialogBox, nameBox;
    [SerializeField] string[] dialogSentences;
    [SerializeField] int currentSentence;

    public static DialogController instance;

    public bool dialogJustStarted = false;

    private bool shouldMarkQuest;
    private string questToMark;
    private bool markQuestComplete; // false --> mark as incomplete

    private bool stillTyping = false;

    void Start()
    {
        instance = this;
        //dialogText.text = dialogSentences[currentSentence]; I dont think I need this
    }

    void Update()
    {
        if(nameText.text == "")
        {
            nameBox.SetActive(false);
        }
        else
        {
            nameBox.SetActive(true);
        }

        if(dialogBox.activeInHierarchy) // box got activated by NPCs dialog handler
        {
            if(Input.GetButtonUp("Fire1") && !stillTyping) // Fire1 --> left click (Project Settings --> Input Manager)
            {
                if(!dialogJustStarted)
                {
                    // move along the dialog
                    currentSentence++;
                    if(currentSentence >= dialogSentences.Length)
                    {
                        // after end of dialog
                        dialogBox.SetActive(false);
                        GameManager.instance.dialogBoxOpened = false;
                        nameText.text = "";
                        MenuButton.instance.SetActiveState(true);

                        // activate quest after opening dialog for the first time
                        if(shouldMarkQuest)
                        {
                            shouldMarkQuest = false;
                            // mark quest complete or incomplete
                            if(markQuestComplete)
                            {
                                QuestManager.instance.MarkQuestComplete(questToMark);
                            }
                            else
                            {
                                QuestManager.instance.MarkQuestIncomplete(questToMark);
                            }
                        }
                    }
                    else // not end of dialog
                    {
                        CheckForName(); // display current name
                        //dialogText.text = dialogSentences[currentSentence]; // display current sentence
                        stillTyping = true;
                        StartCoroutine(RevealText(dialogText, dialogSentences[currentSentence], 0.01f));
                    }
                }
                else
                {
                    // show first sentence
                    dialogJustStarted = false; // then go to code that progresses the dialog
                }
            }
        }
    }

    IEnumerator RevealText(TextMeshProUGUI revealText, string fullText, float typingSpeed)
    {
        AudioManager.instance.PlaySFX(9);

        revealText.text = ""; // clear existing text
        foreach (char letter in fullText)
        {
            revealText.text += letter; // add one letter at a time
            yield return new WaitForSeconds(0.01f); // wait before next letter
        }
        stillTyping = false;
    }

    // set values for marking quest after dialog (called by DialogHandler alongside ActivateDialog)
    public void ActivateQuestAtEnd(string questName, bool markComplete)
    {
        questToMark = questName;
        markQuestComplete = markComplete;
        shouldMarkQuest = true;
    }

    // gets activated by NPCs dialog handler
    public void ActivateDialog(string[] newSentencesToUse)
    {
        // set values for start of dialog
        dialogSentences = newSentencesToUse;
        currentSentence = 0;

        MenuButton.instance.SetActiveState(false);

        CheckForName();
        //dialogText.text = dialogSentences[currentSentence];
        stillTyping = true;
        StartCoroutine(RevealText(dialogText, dialogSentences[currentSentence], 0.01f));
        dialogBox.SetActive(true);

        GameManager.instance.dialogBoxOpened = true; //set condition for player to stop moving
    }

    // if a dialog line starts with hashtag, its a name. Check for name before going to next line in dialog
    void CheckForName()
    {
        if(dialogSentences[currentSentence].StartsWith("#"))
        {
            nameText.text = dialogSentences[currentSentence].Replace("#",""); // remove hashtag from line and set it as name
            currentSentence++; // move to next line
        }
    }

    // needed by NPCs dialog handler to only call the box at the start of the dialog
    public bool IsDialogBoxActive()
    {
        return dialogBox.activeInHierarchy;
    }
}
