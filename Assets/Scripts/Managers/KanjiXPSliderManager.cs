using TMPro;
using UnityEngine;
using UnityEngine.UI;

// given to Kanji XP Slider Objects
public class KanjiXPSliderManager : MonoBehaviour
{
    [SerializeField] private Image kanjiImage;
    [SerializeField] private Slider XPSlider;
    [SerializeField] private TextMeshProUGUI XPSliderText;
    [SerializeField] private TextMeshProUGUI newXPText;

    public void SetSlider(KanjiManager kanji, int newXP)
    {
        // set slider in reward menu
        kanjiImage.sprite = kanji.kanjiImage;
        XPSlider.maxValue = 100;
        XPSlider.value = kanji.currentXP;
        XPSliderText.text = kanji.currentXP + "/ 100";

        newXPText.text = newXP == -10 ? "" : "+" + newXP;
    }
}