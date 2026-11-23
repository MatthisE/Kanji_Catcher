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

        for (int i = 0; i < referenceStrokes.strokes.Length; i++)
        {
            // pair input stroke feature points to reference stroke feature points
            Stroke referenceStroke = referenceStrokes.strokes[i];
            FeaturePointPairing pairing = FeaturePointPairer.Pair(inputFeaturePoints, referenceStroke.points);

            Vector2[] inputPrincipalFeaturePoints = new Vector2[pairing.PrincipalPairs.Count];
            Vector2[] referencePrincipalFeaturePoints = new Vector2[pairing.PrincipalPairs.Count];

            for (int j = 0; j < pairing.PrincipalPairs.Count; j++)
            {
                FeaturePointPair pair = pairing.PrincipalPairs[j];

                inputPrincipalFeaturePoints[j] = pairing.InputFeaturePoints[pair.InputIndex];
                referencePrincipalFeaturePoints[j] = pairing.ReferenceFeaturePoints[pair.ReferenceIndex];
            }

            // add auxiliary feature points
            ResampledStrokePair resampled = FeaturePointResampler.Resample(inputPrincipalFeaturePoints, referencePrincipalFeaturePoints);
        }
    }

    public Vector2[] GetFeaturePoints(int characterIndex, Vector2[] points)
    {
        // break down input stroke into few feature points
        Vector2[] inputFeaturePoints = StrokeSimplifier.Simplify(points, ramerEpsilon);

        Debug.Log(
            $"RawImage {characterIndex}: " +
            $"{points.Length} Input points → " +
            $"{inputFeaturePoints.Length} Feature points."
        );

        for (int i = 0; i < inputFeaturePoints.Length; i++)
        {
            Debug.Log($"Point {i}: X = {inputFeaturePoints[i].x}, Y = {inputFeaturePoints[i].y}");
        }

        return inputFeaturePoints;
    }

    public FeaturePointPairing GetFeaturePointPairing(Vector2[] points, SymbolInStrokes referenceStrokes)
    {
        // break down input stroke into few feature points
        Vector2[] inputFeaturePoints = StrokeSimplifier.Simplify(points, ramerEpsilon);

        Stroke referenceStroke = referenceStrokes.strokes[0];

        // pair input stroke feature points to reference stroke feature points (for all reference strokes)
        FeaturePointPairing pairing = FeaturePointPairer.Pair(inputFeaturePoints, referenceStroke.points);

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

        return pairing;
    }

    public ResampledStrokePair GetResampledAuxiliaryFeaturePoints(Vector2[] points, SymbolInStrokes referenceStrokes)
    {
        // break down input stroke into few feature points
        Vector2[] inputFeaturePoints = StrokeSimplifier.Simplify(points, ramerEpsilon);

        // pair input stroke feature points to reference stroke feature points
        Stroke referenceStroke = referenceStrokes.strokes[0];
        FeaturePointPairing pairing = FeaturePointPairer.Pair(inputFeaturePoints, referenceStroke.points);

        Vector2[] inputPrincipalFeaturePoints = new Vector2[pairing.PrincipalPairs.Count];
        Vector2[] referencePrincipalFeaturePoints = new Vector2[pairing.PrincipalPairs.Count];

        for (int j = 0; j < pairing.PrincipalPairs.Count; j++)
        {
            FeaturePointPair pair = pairing.PrincipalPairs[j];

            inputPrincipalFeaturePoints[j] = pairing.InputFeaturePoints[pair.InputIndex];
            referencePrincipalFeaturePoints[j] = pairing.ReferenceFeaturePoints[pair.ReferenceIndex];
        }

        // add auxiliary feature points
        ResampledStrokePair resampled = FeaturePointResampler.Resample(inputPrincipalFeaturePoints, referencePrincipalFeaturePoints);

        return resampled;
    }

    public Vector2[] GetResampledPrincipalFeaturePoints(Vector2[] points, SymbolInStrokes referenceStrokes)
    {
        // break down input stroke into few feature points
        Vector2[] inputFeaturePoints = StrokeSimplifier.Simplify(points, ramerEpsilon);

        // pair input stroke feature points to reference stroke feature points
        Stroke referenceStroke = referenceStrokes.strokes[0];
        FeaturePointPairing pairing = FeaturePointPairer.Pair(inputFeaturePoints, referenceStroke.points);

        Vector2[] inputPrincipalFeaturePoints = new Vector2[pairing.PrincipalPairs.Count];
        Vector2[] referencePrincipalFeaturePoints = new Vector2[pairing.PrincipalPairs.Count];

        for (int j = 0; j < pairing.PrincipalPairs.Count; j++)
        {
            FeaturePointPair pair = pairing.PrincipalPairs[j];

            inputPrincipalFeaturePoints[j] = pairing.InputFeaturePoints[pair.InputIndex];
            referencePrincipalFeaturePoints[j] = pairing.ReferenceFeaturePoints[pair.ReferenceIndex];
        }
        
        return inputPrincipalFeaturePoints;
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