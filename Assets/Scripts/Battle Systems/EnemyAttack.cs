using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// given to enemy attack menu
public class EnemyAttack : MonoBehaviour
{
    private TrainingWord trainingWord;
    [SerializeField] TextMeshProUGUI kanjiToRead;
    [SerializeField] TextMeshProUGUI kanjiMeaning1;
    [SerializeField] TextMeshProUGUI kanjiMeaning2;
    [SerializeField] TextMeshProUGUI kanjiMeaning3;
    [SerializeField] TextMeshProUGUI kanjiMeaning4;
    [SerializeField] BattleManager battleManager;

    [SerializeField] GameObject helpButton;
    [SerializeField] GameObject lessDefenceText;
    [SerializeField] GameObject hintText;

    private bool hintGiven = false;

    private bool exerciseType; // true --> give kana to kanji, false --> give english meaning to kanji

    public void SetWords()
    {
        // get random kanji
        trainingWord = GetRandomWord();
        kanjiToRead.text = trainingWord.inKanji;

        exerciseType = Random.value > 0.5f; // get random exercise type

        List<string> meanings = new List<string>();

        if(exerciseType)
        {
            SetKanaWords(meanings);
        }
        else{
            SetEnglishWords(meanings);
        }

        // randomize list order
        ShuffleList(meanings);

        // put meanings in menu fields
        kanjiMeaning1.text = meanings[0];
        kanjiMeaning2.text = meanings[1];
        kanjiMeaning3.text = meanings[2];
        kanjiMeaning4.text = meanings[3];
    }

    private void SetKanaWords(List<string> meanings)
    {
        // put meaning of kanji in list
        meanings.Add(trainingWord.inKana);

        // add 3 other random unique meanings
        for (int i = 0; i < 3; i++)
        {
            string meaning = GetRandomWord().inKana;
            while(meanings.Contains(meaning))
            {
                meaning = GetRandomWord().inKana;
            }
            meanings.Add(meaning);
        }
    }

    private void SetEnglishWords(List<string> meanings)
    {
        // put meaning of kanji in list
        meanings.Add(trainingWord.englishMeaning);

        // add 3 other random unique meanings
        for (int i = 0; i < 3; i++)
        {
            string meaning = GetRandomWord().englishMeaning;
            while(meanings.Contains(meaning))
            {
                meaning = GetRandomWord().englishMeaning;
            }
            meanings.Add(meaning);
        }
    }

    public TrainingWord GetRandomWord()
    {
        KanjiManager[] collectedKanji = GameManager.instance.GetCollectedKanji();

        // get random kanji
        int randomIndex = Mathf.FloorToInt(Random.value * collectedKanji.Length);
        TrainingWord[] randomTrainingWords = collectedKanji[randomIndex].trainingWords;
        // get random training word
        int randomIndex2 = Mathf.FloorToInt(Random.value * randomTrainingWords.Length);
        return randomTrainingWords[randomIndex2];
    }

    private void ShuffleList<T>(List<T> list)
    {
        // Fisher-Yates Algorithm
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1); // get a random index
            // swap elements
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void GiveHelp()
    {
        helpButton.SetActive(false);
        lessDefenceText.SetActive(true);

        ShowHint();
        hintGiven = true;
    }

    public void ShowHint()
    {
        hintText.SetActive(true);

        if(exerciseType)
        {
            hintText.GetComponent<TextMeshProUGUI>().text = trainingWord.englishMeaning;
        }
        else
        {
            hintText.GetComponent<TextMeshProUGUI>().text = trainingWord.inKana;
        }
    }

    public void CheckAnswer(TextMeshProUGUI pressedMeaning){
        helpButton.SetActive(false);
        lessDefenceText.SetActive(false);
        ShowHint();

        StartCoroutine(CheckAnswerCoroutine(pressedMeaning));
    }

    public IEnumerator CheckAnswerCoroutine(TextMeshProUGUI pressedMeaning){
        if(exerciseType)
        {
            ChangeColorKana();
        }
        else
        {
            ChangeColorEnglish();
        }
        
        yield return new WaitForSeconds(2f);

        if(exerciseType)
        {
            if(pressedMeaning.text == trainingWord.inKana)
            {
                if(hintGiven)
                {
                    battleManager.StartEnemyAttackImpact(0.66f);
                }
                else
                {
                    battleManager.StartEnemyAttackImpact(0.33f);
                }
            }else
            {
                battleManager.StartEnemyAttackImpact(1);
            }
        }
        else
        {
            if(pressedMeaning.text == trainingWord.englishMeaning)
            {
                if(hintGiven)
                {
                    battleManager.StartEnemyAttackImpact(0.66f);
                }
                else
                {
                    battleManager.StartEnemyAttackImpact(0.33f);
                }
            }else
            {
                battleManager.StartEnemyAttackImpact(1);
            }
        }

        helpButton.SetActive(true);
        hintText.SetActive(false);
        hintGiven = false;
        ChangeColorToBlack();
    }

    private void ChangeColorKana(){
        if(kanjiMeaning1.text == trainingWord.inKana)
        {
            kanjiMeaning1.color = Color.green;
        }else
        {
            kanjiMeaning1.color = Color.red;
        }

        if(kanjiMeaning2.text == trainingWord.inKana)
        {
            kanjiMeaning2.color = Color.green;
        }else
        {
            kanjiMeaning2.color = Color.red;
        }

        if(kanjiMeaning3.text == trainingWord.inKana)
        {
            kanjiMeaning3.color = Color.green;
        }else
        {
            kanjiMeaning3.color = Color.red;
        }

        if(kanjiMeaning4.text == trainingWord.inKana)
        {
            kanjiMeaning4.color = Color.green;
        }else
        {
            kanjiMeaning4.color = Color.red;
        }
    }

    private void ChangeColorEnglish(){
        if(kanjiMeaning1.text == trainingWord.englishMeaning)
        {
            kanjiMeaning1.color = Color.green;
        }else
        {
            kanjiMeaning1.color = Color.red;
        }

        if(kanjiMeaning2.text == trainingWord.englishMeaning)
        {
            kanjiMeaning2.color = Color.green;
        }else
        {
            kanjiMeaning2.color = Color.red;
        }

        if(kanjiMeaning3.text == trainingWord.englishMeaning)
        {
            kanjiMeaning3.color = Color.green;
        }else
        {
            kanjiMeaning3.color = Color.red;
        }

        if(kanjiMeaning4.text == trainingWord.englishMeaning)
        {
            kanjiMeaning4.color = Color.green;
        }else
        {
            kanjiMeaning4.color = Color.red;
        }
    }

    private void ChangeColorToBlack(){
        kanjiMeaning1.color = Color.black;
        kanjiMeaning2.color = Color.black;
        kanjiMeaning3.color = Color.black;
        kanjiMeaning4.color = Color.black;
    }
}
