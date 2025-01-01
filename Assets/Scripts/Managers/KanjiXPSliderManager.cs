using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// given to Kanji XP Slider Objects
public class KanjiXPSliderManager : MonoBehaviour
{
    [SerializeField] Image kanjiImage;
    [SerializeField] Slider XPSlider;
    [SerializeField] TextMeshProUGUI XPSliderText;
    [SerializeField] TextMeshProUGUI newXPText;

    public void SetSlider(KanjiManager kanji, int newXP)
    {
        // set slider in reward menu
        kanjiImage.sprite = kanji.kanjiImage;
        XPSlider.maxValue = 100;
        XPSlider.value = kanji.currentXP;
        XPSliderText.text = kanji.currentXP + "/ 100"; 

        if(newXP == -10)
        {
            newXPText.text = "";
        }
        else
        {
            newXPText.text = "+" + newXP;
        }
    }
}
