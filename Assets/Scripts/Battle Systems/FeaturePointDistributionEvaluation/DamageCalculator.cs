using UnityEngine;

public class DamageCalculator : MonoBehaviour
{
    [Header("Stroke Analysis")]
    [SerializeField, Min(0f)]
    private float ramerEpsilon = 5f;

    public void AnalyzeStroke( int characterIndex, Vector2[] points, SymbolInStrokes referenceStrokes)
    {
        Vector2[] featurePoints = StrokeSimplifier.Simplify(points, ramerEpsilon);

        foreach (Stroke stroke in referenceStrokes.strokes)
        {
            foreach (Vector2 refPoints in stroke.points)
            {
                Debug.Log($"Punkt: X = {refPoints.x}, Y = {refPoints.y}");
            }
        }

        /*
        Debug.Log(
            $"Zeichenfeld {characterIndex}: " +
            $"{points.Length} Eingabepunkte → " +
            $"{featurePoints.Length} Feature-Punkte."
        );

        for (int i = 0; i < featurePoints.Length; i++)
        {
            Debug.Log($"Punkt {i}: X = {featurePoints[i].x}, Y = {featurePoints[i].y}");
        }
        */

        //return featurePoints;
    }

    // compute similarity of two sprites
    public float ComputeSimilarityForTwoSprites(Texture2D tex1, Texture2D tex2)
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
    private bool IsBlack(Color color)
    {
        return color.r <= 0.0f && color.g <= 0.0f && color.b <= 0.0f;
    }

}