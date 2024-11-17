using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

// given to game manager object
public class QuestManager : MonoBehaviour
{
    [SerializeField] string[] questNames;
    [SerializeField] bool[] questMarkersCompleted; // set by code, each bool stands for one of the quests in questNames

    public static QuestManager instance;

    void Start()
    {
        instance = this;
        questMarkersCompleted = new bool[questNames.Length]; // set length of questMarkersCompleted
    }

    void Update()
    {
        // save quest data
        if(Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Data has been saved.");
            SaveQuestData();
        }

        // load quest data
        if(Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("Data has been loaded.");
            LoadQuestData();
        }

        // mark certain quests as complete
        if(Input.GetKeyDown(KeyCode.Q))
        {
            print(CheckIfComplete("Defeat Dragon"));
            MarkQuestComplete("Steal Gem");
            MarkQuestIncomplete("Take Monster Soul");
        }
    }

    // get index of quest in questNames
    public int GetQuestNumber(string questToFind)
    {
        for(int i = 0; i < questNames.Length; i++)
        {
            if(questNames[i] == questToFind)
            {
                return i;
            }
        }

        Debug.LogWarning("Quest: " + questToFind + " does not exist.");
        return 0; // always have first quest (index 0) empty
    }

    public bool CheckIfComplete(string questToCheck)
    {
        int questNumberToCheck = GetQuestNumber(questToCheck);

        if(questNumberToCheck != 0)
        {
            return questMarkersCompleted[questNumberToCheck]; // true or false
        }

        return false; // false because quest does not exist
    }

    // after (in)completing a quest, update all quest objects (to de-/activate them)
    public void UpdateQuestObjects()
    {
        QuestObject[] questObjects = FindObjectsOfType<QuestObject>();

        if(questObjects.Length > 0)
        {
            foreach(QuestObject questObject in questObjects)
            {
                questObject.CheckForCompletion();
            }
        }
    }

    // mark quest as in/complete and update quest objects

    public void MarkQuestComplete(string questToMark)
    {
        int questNumberToCheck = GetQuestNumber(questToMark);
        questMarkersCompleted[questNumberToCheck] = true;

        UpdateQuestObjects();
    }

    public void MarkQuestIncomplete(string questToMark)
    {
        int questNumberToCheck = GetQuestNumber(questToMark);
        questMarkersCompleted[questNumberToCheck] = false;

        UpdateQuestObjects();
    }

    // save completion data (1,0) of each quest in PlayerPrefs (using keyword "QuestMarker_") 
    public void SaveQuestData()
    {
        for(int i = 0; i < questNames.Length; i++)
        {
            if(questMarkersCompleted[i])
            {
                PlayerPrefs.SetInt("QuestMarker_" + questNames[i], 1);
            }
            else
            {
                PlayerPrefs.SetInt("QuestMarker_" + questNames[i], 0);
            }
        }
    }

    // look for QuestMarker of each quest in PlayerPrefs and set the completion statuses in questMarkersCompleted accordingly
    public void LoadQuestData()
    {
        for(int i = 0; i < questNames.Length; i++)
        {
            int valueToSet = 0;
            string keyToUse = "QuestMarker_" + questNames[i];

            if(PlayerPrefs.HasKey(keyToUse))
            {
                valueToSet = PlayerPrefs.GetInt(keyToUse);
            }

            if(valueToSet == 0)
            {
                questMarkersCompleted[i] = false;
            }
            else
            {
                questMarkersCompleted[i] = true;
            }
        }
    }
}
