using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
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

    public void setWords()
    {
        // get random kanji
        trainingWord = GetRandomWord();
        kanjiToRead.text = trainingWord.inKanji;

        // put meaning of kanji in list
        List<string> meanings = new List<string> {trainingWord.inKana};

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

        // randomize list order
        ShuffleList(meanings);

        // put meanings in menu fields
        kanjiMeaning1.text = meanings[0];
        kanjiMeaning2.text = meanings[1];
        kanjiMeaning3.text = meanings[2];
        kanjiMeaning4.text = meanings[3];
    }

    public TrainingWord GetRandomWord()
    {
        PlayerStats[] playerStats = GameManager.instance.GetPlayerStats();
        KanjiManager[] collectedKanji = playerStats[0].collectedKanji;

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

    public void CheckAnswer(TextMeshProUGUI pressedMeaning){
        StartCoroutine(CheckAnswerCoroutine(pressedMeaning));
    }

    public IEnumerator CheckAnswerCoroutine(TextMeshProUGUI pressedMeaning){
        ChangeColor();
        yield return new WaitForSeconds(2f);

        if(pressedMeaning.text == trainingWord.inKana)
        {
            battleManager.StartEnemyAttackImpact(0);
        }else
        {
            battleManager.StartEnemyAttackImpact(1);
        }
        ChangeColorToBlack();
    }

    private void ChangeColor(){
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

    private void ChangeColorToBlack(){
        kanjiMeaning1.color = Color.black;
        kanjiMeaning2.color = Color.black;
        kanjiMeaning3.color = Color.black;
        kanjiMeaning4.color = Color.black;
    }
}
