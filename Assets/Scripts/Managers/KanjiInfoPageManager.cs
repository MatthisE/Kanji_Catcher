using System.Collections;
using System.Collections.Generic;
//using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KanjiInfoPageManager : MonoBehaviour
{
    [SerializeField] Image kanjiSymbol;
    [SerializeField] TextMeshProUGUI onyomi;
    [SerializeField] TextMeshProUGUI kunyomi;
    [SerializeField] TextMeshProUGUI meanings;

    [SerializeField] GameObject[] exampleWords;

    public void SetPage(string buttonKanjiSymbol)
    {
        KanjiManager[] collectedKanjiList = GameManager.instance.GetCollectedKanji();
        KanjiManager kanji = null;

        foreach(KanjiManager collectedKanji in collectedKanjiList)
        {
            if(collectedKanji.kanjiSymbol == buttonKanjiSymbol)
            {
                kanji = collectedKanji;
            }
        }

        kanjiSymbol.sprite = kanji.kanjiImage;
        onyomi.text = string.Join(", ", kanji.onyomi);
        kunyomi.text = string.Join(", ", kanji.kunyomi);
        meanings.text = string.Join(", ", kanji.meanings);

        for(int i = 0; i < exampleWords.Length; i++){
            if(i < kanji.trainingWords.Length)
            {
                exampleWords[i].SetActive(true);
                
                Transform childTransform = exampleWords[i].transform.GetChild(0);
                GameObject childObject = childTransform.gameObject;

                childObject.GetComponent<TextMeshProUGUI>().text = kanji.trainingWords[i].englishMeaning + " - " + kanji.trainingWords[i].inKanji + " - " + kanji.trainingWords[i].inKana;
            }
            else
            {
                exampleWords[i].SetActive(false);
            }
        }
    }
}
