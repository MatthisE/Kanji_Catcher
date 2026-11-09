using UnityEngine;

public class DamageCalculator : MonoBehaviour
{
    // compute similarity of two sprites
    public static float ComputeSimilarityForTwoSprites(Texture2D tex1, Texture2D tex2)
    {
        if (tex1.width != tex2.width || tex1.height != tex2.height)
        {
            Debug.LogError("Textures must have the same dimensions for comparison!");
            return 0f;
        }

        // get size of the images (same size)
        int width = tex1.width;
        int height = tex1.height;

        // check if a pixel is black
        bool isBlack = IsBlack(tex1.GetPixel(0, 0));

        float similarity = 1.0f;
        return similarity;
    }

    // helper method to determine if a pixel is "black"
    private static bool IsBlack(Color color)
    {
        return color.r <= 0.0f && color.g <= 0.0f && color.b <= 0.0f;
    }

}