using System.Collections;
using System.Collections.Generic;
//using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KanjiInfoPageManager : MonoBehaviour
{
    public static KanjiInfoPageManager instance;

    [SerializeField] Image kanjiSymbol;
    [SerializeField] TextMeshProUGUI onyomi;
    [SerializeField] TextMeshProUGUI kunyomi;
    [SerializeField] TextMeshProUGUI meanings;
    [SerializeField] GameObject practiceImage;

    [SerializeField] GameObject[] exampleWords;
    private KanjiManager kanji;

    private bool strokeOrderDisplayed;
    private string kanjiSymbolString;

    private void Awake()
    {
        instance = this; // no singelton pattern needed
    }

    public void SetActiveState(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void SetPage(string buttonKanjiSymbol)
    {
        kanjiSymbolString = buttonKanjiSymbol;

        KanjiManager[] collectedKanjiList = GameManager.instance.GetCollectedKanji();

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

        foreach (Transform child in practiceImage.transform)
        {
            Destroy(child.gameObject); // destroy the SpriteOverlay
        }

        practiceImage.GetComponent<DrawOnRawImage>().Setup();
        practiceImage.GetComponent<DrawOnRawImage>().Repaint();

        strokeOrderDisplayed = false;
        ShowStrokeOrder();
    }

    public void NextPage()
    {
        KanjiManager[] collectedKanjiList = GameManager.instance.GetCollectedKanji();

        // find the index of the object with the matching Name
        int index = 0;
        for (int i = 0; i < collectedKanjiList.Length; i++)
        {
            if (collectedKanjiList[i].kanjiSymbol == kanjiSymbolString)
            {
                index = i;
                break;
            }
        }

        string nextSymbol = null;
        if(index == collectedKanjiList.Length-1)
        {
            nextSymbol = collectedKanjiList[0].kanjiSymbol;
        }
        else
        {
            nextSymbol = collectedKanjiList[index+1].kanjiSymbol;
        }
        SetPage(nextSymbol);
    }

    public void PreviousPage()
    {
        KanjiManager[] collectedKanjiList = GameManager.instance.GetCollectedKanji();

        // find the index of the object with the matching Name
        int index = 0;
        for (int i = 0; i < collectedKanjiList.Length; i++)
        {
            if (collectedKanjiList[i].kanjiSymbol == kanjiSymbolString)
            {
                index = i;
                break;
            }
        }

        string previousSymbol = null;
        if(index == 0)
        {
            previousSymbol = collectedKanjiList[collectedKanjiList.Length-1].kanjiSymbol;
        }
        else
        {
            previousSymbol = collectedKanjiList[index-1].kanjiSymbol;
        }
        SetPage(previousSymbol);
    }

    public void ClearImage()
    {
        practiceImage.GetComponent<DrawOnRawImage>().Repaint();
    }

    public void ShowStrokeOrder()
    {
        if(!strokeOrderDisplayed)
        {
            strokeOrderDisplayed = true;

            RawImage rawImage = practiceImage.GetComponent<RawImage>(); 
            Sprite  overlaySprite = kanji.strokeOrder;
            
            // Create a new GameObject for the overlay
            GameObject overlayObject = new GameObject("SpriteOverlay");
            overlayObject.transform.SetParent(rawImage.transform, false); // Make it a child of the RawImage

            // Add an Image component to the overlay GameObject
            Image overlayImage = overlayObject.AddComponent<Image>();

            // Set the sprite and transparency
            overlayImage.sprite = overlaySprite;
            overlayImage.color = new Color(1, 1, 1, 0.5f); // RGB stays the same; modify alpha for transparency

            // Match the size and position of the RawImage
            RectTransform overlayTransform = overlayObject.GetComponent<RectTransform>();
            overlayTransform.anchorMin = new Vector2(0, 0);
            overlayTransform.anchorMax = new Vector2(1, 1);
            overlayTransform.offsetMin = Vector2.zero;
            overlayTransform.offsetMax = Vector2.zero;
        }
        else
        {
            strokeOrderDisplayed = false;

            foreach (Transform child in practiceImage.transform)
            {
                Destroy(child.gameObject); // destroy the SpriteOverlay
            }
        }
    }
}
