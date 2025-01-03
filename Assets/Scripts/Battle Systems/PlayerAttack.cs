using System.Collections;
using System.Collections.Generic;
using System;
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
    [SerializeField] GameObject clearButton;
    [SerializeField] GameObject doneButton;
    [SerializeField] GameObject lessDamageText;

    [SerializeField] GameObject similarityText;

    private int imageAmount;
    private GameObject overlayObject;
    private bool strokeOrderDisplayed = false;

    private bool forHealing; // false --> for attack

    public void SetWords(TrainingWord thisTrainingWord, bool healing)
    {
        // define it move is for healing or attack
        forHealing = healing;

        // get random kanji
        trainingWord = thisTrainingWord;
        wordInKana.text = trainingWord.inKana;

        imageAmount = trainingWord.wordImage.Length;
        for(int i=0; i<imageAmount; i++)
        {
            rawImages[i].SetActive(true);
        }
    }

    public void ClearImages()
    {
        for(int i=0; i<imageAmount; i++)
        {
            rawImages[i].GetComponent<DrawOnRawImage>().Repaint();
        }
    }

    public void GiveHelp()
    {
        helpButton.SetActive(false);
        lessDamageText.SetActive(true);

        ShowStrokeOrder();
        strokeOrderDisplayed = true;
    }
    
    private void ShowStrokeOrder()
    {
        for(int i = 0; i < imageAmount; i++)
        {
            if(rawImages[i].activeInHierarchy)
            {
                RawImage rawImage = rawImages[i].GetComponent<RawImage>(); 
                Sprite  overlaySprite = trainingWord.strokeOrder[i];
                
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
        }
    }

    public void CheckAnswer()
    {
        StartCoroutine(CheckAnswerCoroutine());
    }

    private double GetFinalAttackDamage(double attackDamage)
    {
        double decrement = 1;
        if(strokeOrderDisplayed)
        {
            decrement = 2;
        }

        if(attackDamage <= 75)
        {
            if(attackDamage <= 50)
            {
                if(attackDamage <= 25)
                {
                    return 0;
                }
                else
                {
                    return 0.5/decrement;
                }
            }
            else
            {
                return 0.75/decrement;
            }
        }
        else
        {
            return 1/decrement;
        }
    }

    public IEnumerator CheckAnswerCoroutine()
    {
        if(!strokeOrderDisplayed){
            ShowStrokeOrder();
        }

        helpButton.SetActive(false);
        clearButton.SetActive(false);
        doneButton.SetActive(false);
        lessDamageText.SetActive(false);

        double attackDamage = CompareImages();
        double finalAttackDamage = GetFinalAttackDamage(attackDamage);
        
        yield return new WaitForSeconds(3f);

        for(int i=0; i<imageAmount; i++)
        {
            rawImages[i].GetComponent<DrawOnRawImage>().Repaint();
        }

        helpButton.SetActive(true);
        clearButton.SetActive(true);
        doneButton.SetActive(true);

        strokeOrderDisplayed = false;
        similarityText.SetActive(false);

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

        for(int i=0; i<imageAmount; i++)
        {
            rawImages[i].SetActive(false);
        }

        // turn final damage into xp for kanji in the training word which you will get at end of battle
        KanjiManager[] collectedKanji = GameManager.instance.GetCollectedKanji();

        foreach(KanjiManager kanjiInWord in trainingWord.kanjiInWord)
        {
            foreach(KanjiManager kanji in collectedKanji)
            {
                if(kanjiInWord.kanjiSymbol == kanji.kanjiSymbol)
                {
                    kanji.xpReward += (int)(finalAttackDamage*10);
                }
            }
        }

        if(forHealing)
        {
            battleManager.HealPlayer(finalAttackDamage);
        }
        else
        {
            battleManager.StartPlayerAttackImpact(finalAttackDamage);
        }
    }

    // compare RawImage and Sprite of correct Kanji
    public double CompareImages()
    {
        float totalSimmilarity = 0.0f;

        for(int i=0; i<imageAmount; i++)
        {
            Texture2D rawTexture = rawImages[i].GetComponent<RawImage>().texture as Texture2D;
            Texture2D spriteTexture = SpriteToTexture(trainingWord.wordImage[i]); 

            if (rawTexture == null || spriteTexture == null)
            {
                Debug.LogError("One or both textures are null!");
                return 0;
            }

            // Compare the textures
            float similarity = ComputeSimilarityForBlackPixels(rawTexture, spriteTexture);
            totalSimmilarity += similarity;
        }

        double finalValue = Math.Round(totalSimmilarity / imageAmount * 100, 0);

        similarityText.GetComponent<TextMeshProUGUI>().text = finalValue + "%";
        similarityText.SetActive(true);

        return finalValue;

    }

    // convert Sprite to Texture2D
    private Texture2D SpriteToTexture(Sprite sprite)
    {
        if (sprite == null) return null;

        Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        Color[] pixels = sprite.texture.GetPixels(
            (int)sprite.rect.x,
            (int)sprite.rect.y,
            (int)sprite.rect.width,
            (int)sprite.rect.height
        );

        // iterate through the pixels and replace transparency with white
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a < 1f) // if pixel is transparent
            {
                pixels[i] = Color.white; // set it to white
            }
        }
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    // compute similarity of black pixels
    private float ComputeSimilarityForBlackPixels(Texture2D tex1, Texture2D tex2, float threshold = 0.0f, int toleranceRadius = 10)
    {
        Color[] pixels1 = tex1.GetPixels();
        Color[] pixels2 = tex2.GetPixels();
        int width = tex1.width;
        int height = tex1.height;

        if (pixels1.Length != pixels2.Length)
        {
            Debug.LogError("Textures must have the same dimensions for comparison!");
            return 0f;
        }

        int totalBlackPixels1 = 0;
        int totalBlackPixels2 = 0;
        int matchingBlackPixels = 0;
        int unmatchedBlackPixels1 = 0;

        // Iterate through every pixel in both textures
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;

                // Count black pixels in tex2 and check if they are matched in tex1
                if (IsBlack(pixels2[index], threshold))
                {
                    totalBlackPixels2++;
                    if (IsBlackPixelInNeighborhood(tex1, x, y, threshold, toleranceRadius))
                    {
                        matchingBlackPixels++;
                    }
                }

                // Count black pixels in tex1 and check if they are unmatched in tex2
                if (IsBlack(pixels1[index], threshold))
                {
                    totalBlackPixels1++;
                    if (!IsBlackPixelInNeighborhood(tex2, x, y, threshold, toleranceRadius))
                    {
                        unmatchedBlackPixels1++;
                    }
                }
            }
        }

        if (totalBlackPixels1 == 0 && totalBlackPixels2 == 0) return 1f; // Both textures are completely empty of black pixels.

        // Similarity calculation: penalize unmatched black pixels in tex1
        float similarity = (float)matchingBlackPixels / (totalBlackPixels2 + unmatchedBlackPixels1);
        return similarity;
    }

    // Check if there is a black pixel within the tolerance radius
    private bool IsBlackPixelInNeighborhood(Texture2D tex, int centerX, int centerY, float threshold, int radius)
    {
        int width = tex.width;
        int height = tex.height;

        for (int offsetY = -radius; offsetY <= radius; offsetY++)
        {
            for (int offsetX = -radius; offsetX <= radius; offsetX++)
            {
                int neighborX = centerX + offsetX;
                int neighborY = centerY + offsetY;

                // Skip out-of-bounds pixels
                if (neighborX < 0 || neighborX >= width || neighborY < 0 || neighborY >= height)
                    continue;

                // Check if this neighbor pixel is black
                if (IsBlack(tex.GetPixel(neighborX, neighborY), threshold))
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Helper method to determine if a pixel is "black"
    private bool IsBlack(Color color, float threshold)
    {
        return color.r <= threshold && color.g <= threshold && color.b <= threshold;
    }

}
