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

    [SerializeField] GameObject helpButton;
    [SerializeField] GameObject doneButton;
    [SerializeField] GameObject lessDamageText;

    private int imageAmount;
    private GameObject overlayObject;
    private bool strokeOrderDisplayed = false;

    public void SetWords(TrainingWord thisTrainingWord)
    {
        // get random kanji
        trainingWord = thisTrainingWord;
        wordInKana.text = trainingWord.inKana;

        imageAmount = trainingWord.wordImage.Length;
        for(int i=0; i<imageAmount; i++)
        {
            rawImages[i].SetActive(true);
        }
    }

    public void GiveHelp()
    {
        helpButton.SetActive(false);
        lessDamageText.SetActive(true);

        showStrokeOrder();
        strokeOrderDisplayed = true;
    }
    
    private void showStrokeOrder()
    {
        RawImage rawImage = rawImages[0].GetComponent<RawImage>(); 
        Sprite  overlaySprite = trainingWord.strokeOrder[0];
        
        // Create a new GameObject for the overlay
        overlayObject = new GameObject("SpriteOverlay");
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

    public void CheckAnswer()
    {
        StartCoroutine(CheckAnswerCoroutine());
    }

    public IEnumerator CheckAnswerCoroutine()
    {
        if(!strokeOrderDisplayed){
            showStrokeOrder();
        }

        helpButton.SetActive(false);
        doneButton.SetActive(false);
        lessDamageText.SetActive(false);
        
        yield return new WaitForSeconds(2f);

        for(int i=0; i<imageAmount; i++)
        {
            rawImages[i].GetComponent<DrawOnRawImage>().Repaint();
        }

        helpButton.SetActive(true);
        doneButton.SetActive(true);

        strokeOrderDisplayed = false;

        // find all objects in the scene
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            // check if the object's name matches
            if (obj.name == "SpriteOverlay")
            {
                Destroy(obj); // destroy the object
            }
        }

        battleManager.StartPlayerAttackImpact();
    }
}
