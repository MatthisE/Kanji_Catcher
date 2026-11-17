using UnityEngine;

public class DamageCalculator : MonoBehaviour
{
    [Header("Stroke Analysis")]
    [SerializeField, Min(0f)]
    private float ramerEpsilon = 5f;

    public void AnalyzeStroke(int characterIndex, Vector2[] points, SymbolInStrokes referenceStrokes)
    {
        // break down input stroke into few feature points
        Vector2[] inputFeaturePoints = StrokeSimplifier.Simplify(points, ramerEpsilon);

        // pair input stroke feature points to reference stroke feature points (for all reference strokes)
        var pairer = new FeaturePointPairer(positionalWeight: 0.5f, directionalWeight: 0.5f, normalizedCharacterSize: 1f);

        foreach (Stroke referenceStroke in referenceStrokes.strokes)
        {
            FeaturePointPairing pairing = pairer.Pair(inputFeaturePoints, referenceStroke.points);

            Debug.Log($"Reference stroke has {pairing.PrincipalPairs.Count} principal pairs.");

            foreach (FeaturePointPair pair in pairing.PrincipalPairs)
            {
                Vector2 inputPoint = inputFeaturePoints[pair.InputIndex];
                Vector2 referencePoint = referenceStroke.points[pair.ReferenceIndex];

                Debug.Log(
                    $"Input {pair.InputIndex} ({inputPoint.x}, {inputPoint.y}) " +
                    $"<-> Reference {pair.ReferenceIndex} " +
                    $"({referencePoint.x}, {referencePoint.y})");
            }
        }
    }

    public Vector2[] GetFeaturePoints(int characterIndex, Vector2[] points)
    {
        // break down input stroke into few feature points
        Vector2[] inputFeaturePoints = StrokeSimplifier.Simplify(points, ramerEpsilon);

        Debug.Log(
            $"Zeichenfeld {characterIndex}: " +
            $"{points.Length} Eingabepunkte → " +
            $"{inputFeaturePoints.Length} Feature-Punkte."
        );

        for (int i = 0; i < inputFeaturePoints.Length; i++)
        {
            Debug.Log($"Punkt {i}: X = {inputFeaturePoints[i].x}, Y = {inputFeaturePoints[i].y}");
        }

        return inputFeaturePoints;
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