using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// given to enemy attack menu
public class EnemyAttack : MonoBehaviour
{
    private TrainingWord trainingWord;
    [SerializeField] private TextMeshProUGUI kanjiToRead;
    [SerializeField] private TextMeshProUGUI kanjiMeaning1;
    [SerializeField] private TextMeshProUGUI kanjiMeaning2;
    [SerializeField] private TextMeshProUGUI kanjiMeaning3;
    [SerializeField] private TextMeshProUGUI kanjiMeaning4;
    [SerializeField] private BattleManager battleManager;

    [SerializeField] private GameObject helpButton;
    [SerializeField] private GameObject lessDefenceText;
    [SerializeField] private GameObject hintText;

    private bool hintGiven = false;

    private bool exerciseType; // true --> give kana to kanji, false --> give english meaning to kanji

    public void SetWords()
    {
        // get random kanji
        trainingWord = GetRandomWord();
        kanjiToRead.text = trainingWord.inKanji;

        exerciseType = Random.value > 0.5f; // get random exercise type

        List<string> meanings = new();

        if (exerciseType)
        {
            SetKanaWords(meanings);
        }
        else
        {
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
            while (meanings.Contains(meaning))
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
            while (meanings.Contains(meaning))
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
            (list[randomIndex], list[i]) = (list[i], list[randomIndex]);
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

        hintText.GetComponent<TextMeshProUGUI>().text = exerciseType ? trainingWord.englishMeaning : trainingWord.inKana;
    }

    public void CheckAnswer(TextMeshProUGUI pressedMeaning)
    {
        helpButton.SetActive(false);
        lessDefenceText.SetActive(false);
        ShowHint();

        _ = StartCoroutine(CheckAnswerCoroutine(pressedMeaning));
    }

    public IEnumerator CheckAnswerCoroutine(TextMeshProUGUI pressedMeaning)
    {
        if (exerciseType)
        {
            ChangeColorKana();
        }
        else
        {
            ChangeColorEnglish();
        }

        yield return new WaitForSeconds(2f);

        bool isCorrect = exerciseType
            ? pressedMeaning.text == trainingWord.inKana
            : pressedMeaning.text == trainingWord.englishMeaning;

        float damage = GetDamageImpact(isCorrect, hintGiven);
        battleManager.StartEnemyAttackImpact(damage);

        ResetHintUI();
    }

    private static float GetDamageImpact(bool isCorrect, bool hintUsed)
    {
        if (!isCorrect)
        {
            return 1f;
        }
        else
        {
            return hintUsed ? 0.66f : 0.33f;
        }
    }

    private void ResetHintUI()
    {
        helpButton.SetActive(true);
        hintText.SetActive(false);
        hintGiven = false;
        ChangeColorToBlack();
    }

    private void ChangeColorKana()
    {
        kanjiMeaning1.color = kanjiMeaning1.text == trainingWord.inKana ? Color.green : Color.red;

        kanjiMeaning2.color = kanjiMeaning2.text == trainingWord.inKana ? Color.green : Color.red;

        kanjiMeaning3.color = kanjiMeaning3.text == trainingWord.inKana ? Color.green : Color.red;

        kanjiMeaning4.color = kanjiMeaning4.text == trainingWord.inKana ? Color.green : Color.red;
    }

    private void ChangeColorEnglish()
    {
        kanjiMeaning1.color = kanjiMeaning1.text == trainingWord.englishMeaning ? Color.green : Color.red;

        kanjiMeaning2.color = kanjiMeaning2.text == trainingWord.englishMeaning ? Color.green : Color.red;

        kanjiMeaning3.color = kanjiMeaning3.text == trainingWord.englishMeaning ? Color.green : Color.red;

        kanjiMeaning4.color = kanjiMeaning4.text == trainingWord.englishMeaning ? Color.green : Color.red;
    }

    private void ChangeColorToBlack()
    {
        kanjiMeaning1.color = Color.black;
        kanjiMeaning2.color = Color.black;
        kanjiMeaning3.color = Color.black;
        kanjiMeaning4.color = Color.black;
    }
}