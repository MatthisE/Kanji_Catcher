using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    private TrainingWord trainingWord;
    [SerializeField] TextMeshProUGUI wordInKana;
    [SerializeField] BattleManager battleManager;
    [SerializeField] GameObject[] rawImages;
    private int imageAmount;

    public void SetWords()
    {
        // get random kanji
        trainingWord = GetRandomWord();
        wordInKana.text = trainingWord.inKana;

        imageAmount = trainingWord.wordImage.Length;
        for(int i=0; i<imageAmount; i++)
        {
            rawImages[i].SetActive(true);
        }
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

    public void CheckAnswer(){
        StartCoroutine(CheckAnswerCoroutine());
    }

    public IEnumerator CheckAnswerCoroutine(){
        yield return new WaitForSeconds(2f);

        for(int i=0; i<imageAmount; i++)
        {
            rawImages[i].GetComponent<DrawOnRawImage>().Repaint();
        }

        battleManager.StartPlayerAttackImpact();
    }
}
