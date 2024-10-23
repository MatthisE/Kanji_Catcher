using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogText, nameText;
    [SerializeField] GameObject dialogBox, nameBox;
    [SerializeField] string[] dialogSentences;
    [SerializeField] int currentSentence;

    public static DialogController instance;

    private bool dialogJustStarted;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        dialogText.text = dialogSentences[currentSentence];
    }

    // Update is called once per frame
    void Update()
    {
        if(dialogBox.activeInHierarchy) // box got activated by NPCs dialog handler
        {
            if(Input.GetButtonUp("Fire1")) // Fire1 --> left mouse button (Project Settings --> Input Manager)
            {
                if(!dialogJustStarted)
                {
                    // move along the dialog
                    currentSentence++;
                    if(currentSentence >= dialogSentences.Length)
                    {
                        dialogBox.SetActive(false);
                        GameManager.instance.dialogBoxOpened = false;
                    }
                    else
                    {
                        CheckForName();
                        dialogText.text = dialogSentences[currentSentence];
                    }
                }
                else
                {
                    dialogJustStarted = false;
                }
            }
        }
    }

    // gets activated by NPCs dialog handler
    public void ActivateDialog(string[] newSentencesToUse)
    {
        dialogSentences = newSentencesToUse;
        currentSentence = 0;

        CheckForName();
        dialogText.text = dialogSentences[currentSentence];
        dialogBox.SetActive(true);

        dialogJustStarted = true;
        GameManager.instance.dialogBoxOpened = true; //set condition for player to stop moving
    }

    // if a dialog line starts with hashtag, its a name. Check for name before going to next line in dialog
    void CheckForName()
    {
        if(dialogSentences[currentSentence].StartsWith("#"))
        {
            nameText.text = dialogSentences[currentSentence].Replace("#","");
            currentSentence++;
        }
    }

    // needed by NPCs dialog handler to only call the Box at the start of the dialog
    public bool IsDialogBoxActive()
    {
        return dialogBox.activeInHierarchy;
    }
}
